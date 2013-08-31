using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class Projectile : Entity
    {
        private Weapon Weapon;
        public Entity SourceObject;

        public Vector2f Velocity;

        public uint Damage = 0;
        public uint HitCount = 0;
        // INSERT LIFE TIMER

        private Vector2f LastPos;

        public Projectile(Game game, Sprite model, Weapon weapon)
            : base(game, model)
        {
            this.Weapon = weapon;
            this.SourceObject = weapon.SourceObject;

            Velocity = new Vector2f();
            LastPos = new Vector2f();
        }

        public override void Update(float dt)
        {
            LastPos = Position;

            // Apply Velocity
            Move(Velocity.X * dt, Velocity.Y * dt);
        }

    }
}
