using System;
using System.Collections.Generic;
using SingleSwitchGame.GUI;
using System.Timers;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class Cannon : Character
    {
        public float AimSpeed = 300;
        public float RotateSpeedMax = 40;
        public float RotateAcc = 20;
        private float RotateVelocity;
        private bool CanRotate = true;
        public Timer RotationDelayTimer;

        private int Score;
        private int ScoreMultiplier = 1;
        public int ScoreMultiplierBase = 1;

        private bool KeyDown;
        
        public CannonWeapon Weapon;
        public AimAssistance AimAssistance;
        /// <summary>Extra AimAssistances for use with powerups.</summary>
        public List<AimAssistance> AimAssistanceExtras = new List<AimAssistance>();

        public List<uint> UpgradeLevels = new List<uint>() { 1, 0, 0, 0 };
        public uint LandmineExplosionChance = 0;

        public List<Powerup> Powerups = new List<Powerup>();
        public DisplayObject PowerupEffect;

        public Cannon(Game game)
            : base(game, game.GraphicsMode == Game.GRAPHICSMODE_NORMAL ? Graphics.GetSprite("assets/sprites/cannon.png") : Graphics.GetSprite("assets/sprites/blueprint/cannon.png"))
        {
            Model.Scale = new Vector2f(0.5f, 0.5f);
            Model.Origin = new Vector2f(26, 30);
            CanMove = false;
            RemoveOnDeath = false;

            HealthMax = 1000;
            Health = UpgradeLevels[0];

            PowerupEffect = new DisplayObject();
            AddChildAt(PowerupEffect, 0);
        }

        public override void Init()
        {
            base.Init();

            RotationDelayTimer = new Timer(140); // Delay in ms before the Cannon starts rotating after firing
            RotationDelayTimer.Elapsed += OnRotationDelayEnd;

            Weapon = new CannonWeapon(Game, this);
        }
        public override void Deinit()
        {
            base.Deinit();

            if (RotationDelayTimer != null)
            {
                RotationDelayTimer.Stop();
                RotationDelayTimer.Dispose();
                RotationDelayTimer = null;
            }

            Weapon.Deinit();
            Weapon = null;
        }
        public override void OnAdded()
        {
            base.OnAdded();

            AimAssistance = new AimAssistance(Game, this);
            AimAssistance.SetPosition(Position);
            Game.Layer_OtherAbove.AddChildAt(AimAssistance, 0);
        }
        public override void OnRemoved()
        {
            StopPowerups();

            base.OnRemoved();

            if (AimAssistance.Parent != null)
                AimAssistance.Parent.RemoveChild(AimAssistance);
        }

        public override void Update(float dt)
        {
            if (IsDead())
                return;

            base.Update(dt);

            if (Game.DEBUG_MOUSE_CONTROLS)
            {
                Rotation = (float)Utils.GetAngle(Position, 
                    new Vector2f((Mouse.GetPosition(Game.Window).X * Game.ResScale) + ((VideoMode.DesktopMode.Width - Game.Window.GetView().Size.X)/2), 
                        Mouse.GetPosition(Game.Window).Y * Game.ResScale));
                return;
            }

            if (CanRotate)
            {
                if (RotateVelocity < RotateSpeedMax)
                    RotateVelocity = Math.Min(RotateVelocity + (RotateAcc * dt), RotateSpeedMax);
                Rotate(RotateVelocity * dt);
            }
        }

        protected override void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (KeyDown || IsDead() || (e != null && Game.KeyIsNotAllowed(e.Code)))
                return;

            KeyDown = true;

            if (!Game.IsRunning())
                return;

            CanRotate = false;
            RotationDelayTimer.Stop();

            AimAssistance.AimStart();
            foreach (AimAssistance t in AimAssistanceExtras)
                t.AimStart();
        }

        protected override void OnKeyReleased(object sender, KeyEventArgs e)
        {
            if (IsDead() || !KeyDown || (e != null && Game.KeyIsNotAllowed(e.Code)))
                return;

            KeyDown = false;

            if (!Game.IsRunning())
            {
                // Interrupt aiming if the player releases the key while paused
                if (!CanRotate)
                {
                    AimAssistance.AimEnd();
                    foreach (AimAssistance a in AimAssistanceExtras)
                        a.AimEnd();
                }
                return;
            }

            if (CanRotate)
                return;

            Weapon.Fire(Utils.GetPointInDirection(Position, Rotation, 40), Rotation, Utils.GetPointInDirection(Position, Rotation, AimAssistance.Reticle.Y));
            foreach (AimAssistance a in AimAssistanceExtras)
                Weapon.Fire(Utils.GetPointInDirection(Position, Rotation + a.RotationOffset, 40), Rotation + a.RotationOffset, Utils.GetPointInDirection(Position, Rotation + a.RotationOffset, a.Reticle.Y));

            StartRotationDelay();

            AimAssistance.AimEnd();
            foreach (AimAssistance a in AimAssistanceExtras)
                a.AimEnd();
        }

        public void StartRotationDelay()
        {
            CanRotate = false;
            RotateVelocity = 0;
            RotationDelayTimer.Start();
        }
        private void OnRotationDelayEnd(Object source, ElapsedEventArgs e)
        {
            RotationDelayTimer.Stop();
            CanRotate = true;
        }


        public override uint Damage(uint amount, uint damageType = 0, object sourceObject = null, object hitObject = null)
        {
            if (!CanTakeDamage || IsDead())
                return Health;

            ResetScoreMultiplier();

            base.Damage(amount, damageType, sourceObject, hitObject);

            // Health UpgradeLevel needs to reflect current health
            UpgradeLevels[0] = Health;

            // Update HUD
            if (Player)
            {
                Game.AIManager.TimesHitThisWave++;
                Game.HUD.SetHealth(Health);
            }

            return Health;
        }

        public override uint Heal(uint amount)
        {
            base.Heal(amount);

            // Update HUD
            if (Player)
                Game.HUD.SetHealth(Health);

            return Health;
        }

        protected override void OnDeath(dynamic sourceObject = null)
        {
            base.OnDeath((object)sourceObject);

            Game.Pause();
            Game.Layer_GUI.AddChild(new GameOverGUI(Game));
            if (Player)
                Game.Player = null;
        }


        // Upgrades

        public void LevelUpUpgrade(int upgrade)
        {
            UpgradeLevels[upgrade]++;
            UpdateUpgrade(upgrade);
        }

        public void UpdateUpgrade(int upgrade)
        {
            uint level = UpgradeLevels[upgrade];

            switch (upgrade)
            {
                case 0:
                {
                    // Health
                    Heal(1);
                    break;
                }
                case 1:
                {
                    // Explosion Blast Radius (40 - 80)
                    Weapon.ExplosionRadius = 40 + (2 * level);
                    AimAssistance.UpdateReticle();
                    break;
                }
                case 2:
                {
                    // Landmines
                    LandmineExplosionChance = level;
                    break;
                }
                case 3:
                {
                    // Base Score Multiplier
                    ScoreMultiplierBase = 1 + (2 * (int)level);
                    if (ScoreMultiplier < ScoreMultiplierBase)
                        ScoreMultiplier = ScoreMultiplierBase;
                        // Update HUD
                    Game.HUD.SetScoreMultiplier(ScoreMultiplier);
                    break;
                }
            }
        }

        public string GetUpgradeValue(int upgrade, uint level)
        {
            switch (upgrade)
            {
                case 0: return level.ToString("D");
                case 1: return (40 + (2 * level)).ToString("D");
                case 2: return level.ToString("D") + "%";
                case 3: return "x" + (1 + (2 * level)).ToString("D");
            }

            return "";
        }


        // Power-ups
        public void StartPowerup(int type)
        {
            for (int i = 0; i < Powerups.Count; i++)
            {
                if (Powerups[i].Type == type || (type == Powerup.TRIPLE_CANNON && Powerups[i].Type == Powerup.OCTUPLE_CANNON))
                {
                    // Restart Timer if Cannon already has powerup (triple cannon refreshes octuple cannon)
                    Powerups[i].Restart();
                    return;
                }
                else if (type == Powerup.OCTUPLE_CANNON && Powerups[i].Type == Powerup.TRIPLE_CANNON)
                {
                    // Octuple Cannon replaces Triple Cannon
                    Powerups[i].Stop();
                    break;
                }
            }

            Powerup powerup = new Powerup(Game, this, type);
            Powerups.Add(powerup);
            powerup.Start();
        }
        public void StopPowerups()
        {

            while (Powerups.Count > 0)
                Powerups[0].Stop();

            Game.HUD.RemovePowerups();
        }

        public bool HasPowerup(int type)
        {
            for (int i = 0; i < Powerups.Count; i++)
            {
                if (Powerups[i].Type == type)
                    return true;
            }
            return false;
        }

        // Score

        public int GetScore() { return Score; }

        /// <summary>Increases score by points * ScoreMultiplier.</summary>
        public void IncreaseScore(int points)
        {
            Score += points * ScoreMultiplier;
            Game.HUD.SetScore(Score);
        }
        public void IncreaseScoreMultiplier(int amount = 1)
        {
            ScoreMultiplier += amount;
            Game.HUD.SetScoreMultiplier(ScoreMultiplier);
        }
        /// <summary>Resets ScoreMultiplier to ScoreMultiplierBase.</summary>
        public void ResetScoreMultiplier()
        {
            ScoreMultiplier = ScoreMultiplierBase;
            Game.HUD.SetScoreMultiplier(ScoreMultiplier);
        }

    }

    class Powerup
    {
        private Game Game;
        private Cannon Cannon;

        public int Type;
        private Timer Timer;

        public const int DOUBLE_EXPLOSION_RADIUS = 1;
        public const int AIM_SPEED_INCREASE = 2;
        public const int FREEZE_TIME = 3;
        public const int TRIPLE_CANNON = 4;
        public const int OCTUPLE_CANNON = 5;
        public const int RED_HOT_BEACH = 6;
        public const int MAX = 6;

        public Powerup(Game game, Cannon cannon, int type)
        {
            Game = game;
            Cannon = cannon;
            Type = type;
        }

        public void Start()
        {
            Timer = new Timer();
            Timer.AutoReset = false;
            Timer.Elapsed += OnEnd;

            Timer.Interval = 10000;
            switch (Type)
            {
                case DOUBLE_EXPLOSION_RADIUS:
                {
                    Cannon.Weapon.ExplosionRadius *= 2;
                    Cannon.AimAssistance.UpdateReticle();
                    Cannon.AimAssistance.UpdateReticlePosition();
                    foreach (AimAssistance a in Cannon.AimAssistanceExtras)
                    {
                        a.UpdateReticle();
                        a.UpdateReticlePosition();
                    }
                    break;
                }
                case AIM_SPEED_INCREASE:
                {
                    Cannon.AimSpeed *= 1.5f;
                    Cannon.RotateSpeedMax *= 1.5f;
                    Cannon.RotateAcc *= 3f;
                    Cannon.RotationDelayTimer.Interval /= 2f;

                    Timer.Interval = 20000;
                    break;
                }
                case TRIPLE_CANNON:
                case OCTUPLE_CANNON:
                {
                    int amount = Type == TRIPLE_CANNON ? 3 : 8;
                    for (int b = 1; b < amount; b++)
                    {
                        var barrel = new Sprite(((Sprite)Cannon.Model).Texture);
                        barrel.Scale = new Vector2f(0.5f, 0.5f);
                        barrel.Origin = new Vector2f(26, 30);
                        if (amount == 3)
                            barrel.Rotation = b == 1 ? 45 : -45;
                        else
                            barrel.Rotation = 45 * b;
                        Cannon.PowerupEffect.AddChild(barrel);

                        var aa = new AimAssistance(Game, Cannon);
                        aa.Position = Cannon.Position;
                        aa.RotationOffset = barrel.Rotation;
                        Game.Layer_OtherAbove.AddChildAt(aa, 0);
                        Cannon.AimAssistanceExtras.Add(aa);

                        if (Cannon.AimAssistance.IsAiming())
                        {
                            aa.AimStart();
                            aa.SetReticlePosition(Cannon.AimAssistance.GetReticlePosition());
                        }
                    }
                    break;
                }
            }

            Game.HUD.AddPowerup(Type, Timer.Interval);

            Timer.Start();
        }
        public void Stop()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer.Dispose();
                Timer = null;
            }

            switch (Type)
            {
                case DOUBLE_EXPLOSION_RADIUS:
                {
                    Cannon.Weapon.ExplosionRadius /= 2;
                    Cannon.AimAssistance.UpdateReticle();
                    Cannon.AimAssistance.UpdateReticlePosition();
                    foreach (AimAssistance a in Cannon.AimAssistanceExtras)
                    {
                        a.UpdateReticle();
                        a.UpdateReticlePosition();
                    }
                }
                break;
                case AIM_SPEED_INCREASE:
                {
                    Cannon.AimSpeed /= 1.5f;
                    Cannon.RotateSpeedMax /= 1.5f;
                    Cannon.RotateAcc /= 3f;
                    Cannon.RotationDelayTimer.Interval *= 2f;
                }
                break;
                case TRIPLE_CANNON:
                case OCTUPLE_CANNON:
                {
                    Cannon.PowerupEffect.Clear();
                    foreach (AimAssistance a in Cannon.AimAssistanceExtras)
                    {
                        if (a.Parent != null)
                            a.Parent.RemoveChild(a);
                    }
                    Cannon.AimAssistanceExtras.Clear();
                }
                break;
            }

            Game.HUD.RemovePowerup(Type);

            Cannon.Powerups.Remove(this);
        }
        public void Restart()
        {
            Timer.Stop();
            Timer.Start();

            Game.HUD.RestartPowerup(Type, Timer.Interval);
        }

        private void OnEnd(Object source, ElapsedEventArgs e)
        {
            Stop();
        }

        public static string GetPowerupName(int powerup)
        {
            switch (powerup)
            {
                case DOUBLE_EXPLOSION_RADIUS:
                    return "Double Explosion Radius";
                case AIM_SPEED_INCREASE:
                    return "Aim Speed Increase";
                case FREEZE_TIME:
                    return "Freeze Time";
                case TRIPLE_CANNON:
                    return "Triple Cannon";
                case OCTUPLE_CANNON:
                    return "Octuple Cannon";
                case RED_HOT_BEACH:
                    return "Red Hot Beach";
            }
            return "";
        }
    }
}
