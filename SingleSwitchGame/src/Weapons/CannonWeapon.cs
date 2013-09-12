using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class CannonWeapon : ProjectileWeapon
    {

        public float ExplosionRadius = 40;

        public CannonWeapon(Game Game, Entity sourceObject)
            : base(Game, sourceObject)
        {
            ProjectileSpeed = 500.0f;
            Damage = 4000;
        }

        public override void Explode(Vector2f pos)
        {
            Explosion explosion = new Explosion(Game, ExplosionRadius);
            explosion.Position = pos;
            Game.Layer_Other.AddChild(explosion);
            
            // Collision
            bool hitSomething = false;
            for (int i = 0; i < Game.Layer_Objects.NumChildren; i++)
            {
                dynamic obj = Game.Layer_Objects.GetChildAt(i);

                if (!(obj is PhysicalEntity) || !obj.CanTakeDamage)
                    continue;
                if ((!(obj is CollisionEntity) || obj.Collision == null) && !Utils.InCircle(pos, ExplosionRadius, obj.Position)) // Collide with Position if obj is not a CollisionEntity or has no Collision
                    continue;
                if (obj.Collision is CircleShape && !Utils.CircleCircleCollision(pos, ExplosionRadius, obj.Position, obj.Model.Radius))
                    continue;
                if (obj.Collision is RectangleShape && !Utils.CircleRectangleCollision(pos, ExplosionRadius, obj.Collision, obj.Position))
                    continue;

                obj.Damage(Damage, 0, SourceObject);
                if (obj.Parent != Game.Layer_Objects)
                    i--;

                hitSomething = true;
            }

            if (Game.Player == null)
                return;
            if (hitSomething)
                Game.Player.IncreaseScoreMultiplier(1);
            else
                Game.Player.ResetScoreMultiplier();
        }

        public override void OnProjectileLifeEnd()
        {
            if (Game.Player != null)
                Game.Player.ResetScoreMultiplier();
        }
    }
}
