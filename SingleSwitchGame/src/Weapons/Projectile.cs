using System;
using SFML.Graphics;
using SFML.Window;
using System.Timers;

namespace SingleSwitchGame
{
    class Projectile : Entity
    {
        private ProjectileWeapon Weapon;
        public Entity SourceObject;

        public Vector2f Velocity;

        public uint Damage = 0;
        public uint HitCount = 0;
        private Timer LifeSpanTimer;
        private bool HasTargetPosition = false;
        private Vector2f TargetPosition;

        /// <summary>Sets to true when LifeSpanTimer ends. This exists because System.Timers run in different threads.</summary>
        private bool RemoveNextTick = false;
        private Vector2f LastPos;

        public Projectile(Game Game, dynamic model, ProjectileWeapon weapon)
            : base(Game, (object)model)
        {
            this.Weapon = weapon;
            this.SourceObject = weapon.SourceObject;

            Velocity = new Vector2f();
            LastPos = new Vector2f();
        }
        public override void Deinit()
        {
            base.Deinit();

            if (LifeSpanTimer != null)
            {
                LifeSpanTimer.Stop();
                LifeSpanTimer.Dispose();
                LifeSpanTimer = null;
            }
        }

        public override void Update(float dt)
        {
            if (RemoveNextTick)
            {
                Parent.RemoveChild(this);
                return;
            }

            LastPos = Position;

            // Apply Velocity
            Move(Velocity.X * dt, Velocity.Y * dt);

            // Check Collisions

            if (HasTargetPosition && Utils.Distance(this, TargetPosition) < 10)
            {
                // Hit Target Position
                OnProjectileCollision();
                return;
            }
        }

        private void OnProjectileCollision(dynamic HitTarget = null)
        {
            Weapon.OnProjectileCollision(HitTarget);

            Parent.RemoveChild(this);
        }

        public void SetTargetPosition(Vector2f TargetPosition)
        {
            this.TargetPosition = TargetPosition;
            HasTargetPosition = true;
        }

        public void SetLifeSpan(uint LifeSpan)
        {
            LifeSpanTimer = new Timer(LifeSpan);
            LifeSpanTimer.Start();
            LifeSpanTimer.Elapsed += new ElapsedEventHandler(OnLifeSpanEnd);
        }
        private void OnLifeSpanEnd(Object source, ElapsedEventArgs e)
        {
            LifeSpanTimer.Stop();
            LifeSpanTimer.Dispose();
            LifeSpanTimer = null;

            RemoveNextTick = true;
        }
    }
}
