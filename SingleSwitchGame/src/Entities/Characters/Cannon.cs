using System;
using SFML.Window;
using SingleSwitchGame.GUI;
using System.Timers;

namespace SingleSwitchGame
{
    class Cannon : Character
    {
        public float AimSpeed = 300;
        public float RotateSpeedMax = 35;
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

        public Cannon(Game Game)
            : base(Game, Graphics.GetSprite("assets/sprites/cannon.png"))
        {
            SetScale(0.5f, 0.5f);
            Origin = new Vector2f(26, 30);
            CanMove = false;
            RemoveOnDeath = false;

            HealthMax = 2;
            Health = HealthMax;
        }

        public override void Init()
        {
            base.Init();

            RotationDelayTimer = new Timer(300); // Delay in ms before the Cannon starts rotating after firing
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
            Game.Layer_GUI.AddChild(AimAssistance);
        }
        public override void OnRemoved()
        {
            base.OnRemoved();

            Game.Layer_GUI.RemoveChild(AimAssistance);
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
            if (KeyDown || IsDead() || e.Code == Keyboard.Key.Escape || e.Code == Keyboard.Key.F11)
                return;

            KeyDown = true;

            CanRotate = false;
            RotationDelayTimer.Stop();

            AimAssistance.AimStart();
        }

        protected override void OnKeyReleased(object sender, KeyEventArgs e)
        {
            if (IsDead() || e.Code == Keyboard.Key.Escape || e.Code == Keyboard.Key.F11)
                return;

            KeyDown = false;

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
