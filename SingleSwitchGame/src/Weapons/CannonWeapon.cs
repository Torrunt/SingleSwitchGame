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
        }

        public override void Explode(Vector2f pos)
        {
            Explosion explosion = new Explosion(Game, ExplosionRadius);
            explosion.Position = pos;
            Game.Layer_Objects.AddChild(explosion);
        }

    }
}
