using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class CannonWeapon : ProjectileWeapon
    {

        public float ExplosionRadius = 40;

        public CannonWeapon(Game game, Entity sourceObject)
            : base(game, sourceObject)
        {
            ProjectileSpeed = 600.0f;
            Damage = 4000;

            ProjectileRotateSpeed = 10;
        }

        public override void Explode(Vector2f pos)
        {
            Explosion explosion = new Explosion(Game, ExplosionRadius);
            explosion.Position = pos;
            Game.Layer_OtherAbove.AddChild(explosion);
            
            // Collision
            bool hitSomething = false;
            int numChildren = Game.Layer_Objects.NumChildren;
            for (int i = 0; i < numChildren; i++)
            {
                dynamic obj = Game.Layer_Objects.GetChildAt(i);

                if (obj is PhysicalEntity && !obj.CanTakeDamage)
                    continue;
                if ((!(obj is CollisionEntity) || obj.Collision == null) && !Utils.InCircle(pos, ExplosionRadius, obj.Position)) // Collide with Position if obj is not a CollisionEntity or has no Collision
                    continue;
                if (obj.Collision is CircleShape && !Utils.CircleCircleCollision(pos, ExplosionRadius, obj.Position, obj.Collision.Radius))
                    continue;
                if (obj.Collision is RectangleShape && !Utils.CircleRectangleCollision(pos, ExplosionRadius, obj.Collision, obj.Rotation, obj.Position))
                    continue;

                if (obj is PhysicalEntity)
                {
                    // Damage
                    obj.Damage(Damage, 0, SourceObject);
                }
                else if (obj is Pickup)
                {
                    // Pickup
                    obj.Activate(SourceObject);
                }

                // Removed?
                if (obj.Parent != Game.Layer_Objects && i <= numChildren - 1)
                {
                    i--;
                    numChildren--;
                }

                hitSomething = true;
            }

            if (Game.Player == null)
                return;
            if (hitSomething)
                Game.Player.IncreaseScoreMultiplier();
            else if (Game.Player.CurrentPowerup != Cannon.POWERUP_TRIPLE_CANNON && Game.Player.CurrentPowerup != Cannon.POWERUP_OCTUPLE_CANNON)
                Game.Player.ResetScoreMultiplier();
        }

        public override void OnProjectileLifeEnd()
        {
            if (Game.Player != null)
                Game.Player.ResetScoreMultiplier();
        }

        public override dynamic GetProjectileModel()
        {
            if (Game.GraphicsMode == Game.GRAPHICSMODE_NORMAL)
            {
                Sprite proj = Graphics.GetSprite("assets/sprites/cannon_ball.png");
                proj.Origin = new Vector2f(20, 20);
                proj.Scale = new Vector2f(0.5f, 0.5f);

                return proj;
            }

            return base.GetProjectileModel();
        }
    }
}
