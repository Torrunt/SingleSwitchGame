using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class PhysicalEntity : CollisionEntity
    {
        public float SpeedMax;
        public float Acc;
        public float Friction;
        protected Vector2f Velocity;

        private uint _Health;
        private uint _HealthMax;
        
        public PhysicalEntity(Game game, Sprite Model)
            : base(game, Model)
        {
            HealthMax = 10000;
            Health = HealthMax;

            SpeedMax = 100.0f; // 100px a second
            Acc = 400.0f; // hits full speed in 0.25 seconds
            Friction = 400.0f; // loses all speed in 0.25 seconds
            Velocity = new Vector2f(0, 0);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            
            // Slow Down
            if (!IsMovingHorizontally())
                Velocity.X = Utils.StepTo(Velocity.X, 0.0f, Friction * dt);
            if (!IsMovingVertically())
                Velocity.Y = Utils.StepTo(Velocity.Y, 0.0f, Friction * dt);
            
            // Apply Velocity
            Move(Velocity.X * dt, Velocity.Y * dt);
        }

        public virtual bool IsMoving() { return false; }
        public virtual bool IsMovingHorizontally() { return false; }
        public virtual bool IsMovingVertically() { return false; }

        // Health
        public uint Health { get { return _Health; } set { _Health = value > HealthMax ? HealthMax : value; } }
        public uint HealthMax { get { return _HealthMax; } set { _HealthMax = value; } }
        /// <param name="sourceObject">Who caused the damage (eg: who get's the credit?).</param>
        /// <param name="hitObject">What caused the damage (eg: a projectile?).</param>
        public uint Damage(uint amount, uint damageType = 0, Object sourceObject = null, Object hitObject = null)
        {
            Health -= amount;
            return Health;
        }
        public uint Heal(uint amount)
        {
            Health += amount;
            return Health;
        }
        public bool IsDead() { return Health == 0; }

    }
}
