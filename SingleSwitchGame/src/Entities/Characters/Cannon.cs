using System;
using System.Collections.Generic;
using SFML.Window;
using SingleSwitchGame.GUI;
using System.Timers;

namespace SingleSwitchGame
{
    class Cannon : Character
    {
        public float AimSpeed = 300;
        public float RotateSpeedMax = 40;
        public float RotateAcc = 10;
        private float RotateVelocity;
        private bool CanRotate = true;
        private Timer RotationDelayTimer;

        private int Score;
        private int ScoreMultiplier = 1;
        public int ScoreMultiplierBase = 1;

        private bool KeyDown;
        
        public CannonWeapon Weapon;
        public AimAssistance AimAssistance;

        public List<uint> UpgradeLevels = new List<uint>() { 1, 0, 0, 0 };
        public uint LandmineExplosionChance = 0;

        public Cannon(Game Game)
            : base(Game, Game.GraphicsMode == Game.GRAPHICSMODE_NORMAL ? Graphics.GetSprite("assets/sprites/cannon.png") : Graphics.GetSprite("assets/sprites/blueprint/cannon.png"))
        {
            SetScale(0.5f, 0.5f);
            Origin = new Vector2f(26, 30);
            CanMove = false;
            RemoveOnDeath = false;

            HealthMax = 1000;
            Health = UpgradeLevels[0];
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
            Game.Layer_OtherAbove.AddChildAt(AimAssistance, 0);
        }
        public override void OnRemoved()
        {
            base.OnRemoved();

            if (AimAssistance.Parent != null)
                AimAssistance.Parent.RemoveChild(AimAssistance);
        }

        public override void Update(float dt)
        {
            if (IsDead())
                return;

            base.Update(dt);

            if (CanRotate)
            {
                if (RotateVelocity < RotateSpeedMax)
                    RotateVelocity = Math.Min(RotateVelocity + (RotateAcc * dt), RotateSpeedMax);
                Rotate(RotateVelocity * dt);
            }
        }

        protected override void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (KeyDown || IsDead() || Game.KeyIsNotAllowed(e.Code))
                return;

            KeyDown = true;

            if (!Game.IsRunning())
                return;

            CanRotate = false;
            RotationDelayTimer.Stop();

            AimAssistance.AimStart();
        }

        protected override void OnKeyReleased(object sender, KeyEventArgs e)
        {
            if (IsDead() || Game.KeyIsNotAllowed(e.Code))
                return;

            KeyDown = false;

            if (!Game.IsRunning())
            {
                // Interrupt aiming if the player releases the key while paused
                if (!CanRotate)
                    AimAssistance.AimEnd();
                return;
            }

            if (CanRotate)
                return;

            Weapon.Fire(Utils.GetPointInDirection(Position, Rotation, 40), Rotation, Utils.GetPointInDirection(Position, Rotation, AimAssistance.Reticle.Y));
            StartRotationDelay();

            AimAssistance.AimEnd();
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
                Game.HUD.SetHealth(Health);

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
}
