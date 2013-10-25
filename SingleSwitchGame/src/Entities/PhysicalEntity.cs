using System;
using SFML.Graphics;
using SFML.Window;
using System.Timers;

namespace SingleSwitchGame
{
    class PhysicalEntity : CollisionEntity
    {
        public float SpeedMax;
        public float Acc;
        public float Friction;
        public Vector2f Velocity;
        /// <summary>If true, uses Velocity for movement. If false, uses MoveAngle and MoveAngleVelocity for movement.</summary>
        public bool DefaultVelocityApplication = true;
        public float MoveAngleVelocity = 0;
        public float MoveAngle = 0;

        private uint _Health;
        private uint _HealthMax;
        public bool CanTakeDamage = true;
        protected bool RemoveOnDeath = true;
        public event EventHandler Death;

        public bool FlashOnDamageBright = false;
        private bool _FlashOnDamage = false;
        private Timer FlashOnDamageTimer;
        private Color FlashOnDamageColor;
        public Color FlashOnDamageOriginalColor;
        private Color FlashOnDamageOriginalColor2;

        public PhysicalEntity(Game Game, dynamic model = null)
            : base(Game, (object)model)
        {
            HealthMax = 10000;
            Health = HealthMax;

            FlashOnDamage = true;

            SpeedMax = 100.0f; // 100px a second
            Acc = 400.0f; // hits full speed in 0.25 seconds
            Friction = 400.0f; // loses all speed in 0.25 seconds
            Velocity = new Vector2f(0, 0);
        }
        public override void Deinit()
        {
            base.Deinit();

            if (FlashOnDamageTimer != null)
            {
                FlashOnDamageTimer.Stop();
                FlashOnDamageTimer.Dispose();
                FlashOnDamageTimer = null;
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            
            // Slow Down
            if (DefaultVelocityApplication)
            {
                if (Velocity.X != 0.0f && !IsMovingHorizontally())
                    Velocity.X = Utils.StepTo(Velocity.X, 0.0f, Friction * dt);
                else if (Velocity.X > SpeedMax)
                    Velocity.X = Utils.StepTo(Velocity.X, SpeedMax, Friction * dt);
                else if (Velocity.X < -SpeedMax)
                    Velocity.X = Utils.StepTo(Velocity.X, -SpeedMax, Friction * dt);

                if (Velocity.Y != 0.0f && !IsMovingVertically())
                    Velocity.Y = Utils.StepTo(Velocity.Y, 0.0f, Friction * dt);
                else if (Velocity.Y > SpeedMax)
                    Velocity.Y = Utils.StepTo(Velocity.Y, SpeedMax, Friction * dt);
                else if (Velocity.X < -SpeedMax)
                    Velocity.Y = Utils.StepTo(Velocity.Y, -SpeedMax, Friction * dt);
            }
            else
            {
                if (Velocity.X != 0 || Velocity.Y != 0)
                {
                    Velocity.X = Utils.StepTo(Velocity.X, 0.0f, Friction * dt);
                    Velocity.Y = Utils.StepTo(Velocity.Y, 0.0f, Friction * dt);
                }
                else
                {
                    if (!IsMoving())
                        MoveAngleVelocity = Utils.StepTo(MoveAngleVelocity, 0, Friction * dt);
                    else if (MoveAngleVelocity > SpeedMax)
                        MoveAngleVelocity = Utils.StepTo(MoveAngleVelocity, SpeedMax, Friction * dt);
                }
            }

            // Apply Velocity
            if (DefaultVelocityApplication || Velocity.X != 0 || Velocity.Y != 0)
                Move(Velocity.X * dt, Velocity.Y * dt);
            else
                Move((float)Math.Cos(MoveAngle) * (MoveAngleVelocity * dt), (float)Math.Sin(MoveAngle) * (MoveAngleVelocity * dt));
        }

        public virtual bool IsMoving() { return false; }
        public virtual bool IsMovingHorizontally() { return false; }
        public virtual bool IsMovingVertically() { return false; }

        // Health
        public uint Health { get { return _Health; } set { _Health = value > HealthMax ? HealthMax : value; } }
        public uint HealthMax { get { return _HealthMax; } set { _HealthMax = value; } }
        /// <param name="sourceObject">Who caused the damage (eg: who get's the credit?).</param>
        /// <param name="hitObject">What caused the damage (eg: a projectile?).</param>
        public virtual uint Damage(uint amount, uint damageType = 0, object sourceObject = null, Object hitObject = null)
        {
            if (!CanTakeDamage || IsDead())
                return Health;

            if (amount >= Health)
                Health = 0;
            else
                Health -= amount;

            if (Health == 0)
            {
                // Dead
                OnDeath(sourceObject);

                if (RemoveOnDeath)
                    Parent.RemoveChild(this);
            }
            else if (FlashOnDamage)
                FlashOnDamageStart(damageType);

            return Health;
        }
        public virtual uint Heal(uint amount)
        {
            Health += amount;
            return Health;
        }
        public bool IsDead() { return Health == 0; }
        protected virtual void OnDeath(dynamic sourceObject = null)
        {
            if (Death != null)
                Death(this, EventArgs.Empty);
        }

        public bool FlashOnDamage
        {
            get { return _FlashOnDamage; }
            set
            {
                _FlashOnDamage = value;
                if (FlashOnDamage && FlashOnDamageTimer == null)
                {
                    FlashOnDamageTimer = new Timer(100);
                    FlashOnDamageTimer.Elapsed += FlashOnDamageHandler;
                }
                else if (!FlashOnDamage && FlashOnDamageTimer != null)
                {
                    FlashOnDamageTimer.Stop();
                    FlashOnDamageTimer.Dispose();
                    FlashOnDamageTimer = null;
                }
            }
        }
        protected void FlashOnDamageStart(uint damageType = 0)
        {
            if (damageType == DamageType.FIRE)
                FlashOnDamageColor = new Color(255, 0, 0, 200);
            else
                FlashOnDamageColor = new Color(255, 255, 255, 200);

            FlashOnDamageHandler();
            FlashOnDamageTimer.Start();
        }
        private void FlashOnDamageHandler(Object source = null, ElapsedEventArgs e = null)
        {
            if (Model == null)
                return;

            FlashOnDamageBright = !FlashOnDamageBright;

            if (Model is Sprite)
            {
                if (FlashOnDamageBright)
                {
                    FlashOnDamageOriginalColor = Model.Color;
                    Model.Color = FlashOnDamageColor;
                }
                else
                    Model.Color = FlashOnDamageOriginalColor;
            }
            else if (Model is AnimatedSprite)
            {
                if (FlashOnDamageBright)
                {
                    FlashOnDamageOriginalColor = Model.Sprite.Color;
                    Model.Sprite.Color = FlashOnDamageColor;
                }
                else
                    Model.Sprite.Color = FlashOnDamageOriginalColor;
            }
            else if (Model is Shape)
            {
                if (FlashOnDamageBright)
                {
                    FlashOnDamageOriginalColor = Model.FillColor;
                    FlashOnDamageOriginalColor2 = Model.OutlineColor;
                    Model.FillColor = FlashOnDamageColor;
                    Model.OutlineColor = FlashOnDamageColor;
                }
                else
                {
                    Model.FillColor = FlashOnDamageOriginalColor;
                    Model.OutlineColor = FlashOnDamageOriginalColor2;
                }
            }

            // Finish
            if (!FlashOnDamageBright)
                FlashOnDamageTimer.Stop();
        }

    }
}
