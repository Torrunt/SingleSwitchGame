using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class PhysicalEntity : CollisionEntity
    {
        public float speedMax;
        public float acc;
        public float friction;
        protected Vector2f velocity;

        private uint _health;
        private uint _healthMax;
        
        public PhysicalEntity(Game game, Sprite model)
            : base(game, model)
        {
            healthMax = 10000;
            health = healthMax;

            speedMax = 100.0f; // 100px a second
            acc = 400.0f; // hits full speed in 0.25 seconds
            friction = 400.0f; // loses all speed in 0.25 seconds
            velocity = new Vector2f(0, 0);
        }

        public override void update(float dt)
        {
            base.update(dt);
            
            // Slow Down
            if (!isMovingHorizontally())
                velocity.X = Utils.stepTo(velocity.X, 0.0f, friction * dt);
            if (!isMovingVertically())
                velocity.Y = Utils.stepTo(velocity.Y, 0.0f, friction * dt);
            
            // Apply Velocity
            move(velocity.X * dt, velocity.Y * dt);
        }

        public virtual bool isMoving() { return false; }
        public virtual bool isMovingHorizontally() { return false; }
        public virtual bool isMovingVertically() { return false; }

        // Health
        public uint health { get { return _health; } set { _health = value > healthMax ? healthMax : value; } }
        public uint healthMax { get { return _healthMax; } set { _healthMax = value; } }
        /// <param name="sourceObject">Who caused the damage (eg: who get's the credit?).</param>
        /// <param name="hitObject">What caused the damage (eg: a projectile?).</param>
        public uint damage(uint amount, uint damageType = 0, object sourceObject = null, object hitObject = null)
        {
            health -= amount;
            return health;
        }
        public uint heal(uint amount)
        {
            health += amount;
            return health;
        }
        public bool isDead() { return health == 0; }

    }
}
