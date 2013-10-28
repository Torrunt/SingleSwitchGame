using System;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;

namespace SingleSwitchGame
{
    class CannonWeapon : ProjectileWeapon
    {

        public float ExplosionRadius = 40;

        // Sounds
        private SoundBuffer ShootSoundBuffer1;
        private SoundBuffer ShootSoundBuffer2;
        private Sound ShootSound1;
        private Sound ShootSound2;

        private SoundBuffer ExplosionSoundBuffer1;
        private SoundBuffer ExplosionSoundBuffer2;
        private Sound ExplosionSound1;
        private Sound ExplosionSound2;

        private SoundBuffer SplashSoundBuffer1;
        private SoundBuffer SplashSoundBuffer2;
        private Sound SplashSound1;
        private Sound SplashSound2;

        public CannonWeapon(Game game, Entity sourceObject)
            : base(game, sourceObject)
        {
            ProjectileSpeed = 600.0f;
            Damage = 4000;

            ProjectileRotateSpeed = 10;

            ShootSoundBuffer1 = new SoundBuffer("assets/audio/shotgun1.ogg");
            ShootSoundBuffer2 = new SoundBuffer("assets/audio/shotgun2.ogg");
            ShootSound1 = new Sound(ShootSoundBuffer1);
            ShootSound2 = new Sound(ShootSoundBuffer2);

            ExplosionSoundBuffer1 = new SoundBuffer("assets/audio/explode1.ogg");
            ExplosionSoundBuffer2 = new SoundBuffer("assets/audio/explode2.ogg");
            ExplosionSound1 = new Sound(ExplosionSoundBuffer1);
            ExplosionSound2 = new Sound(ExplosionSoundBuffer2);

            SplashSoundBuffer1 = new SoundBuffer("assets/audio/splash1.ogg");
            SplashSoundBuffer2 = new SoundBuffer("assets/audio/splash2.ogg");
            SplashSound1 = new Sound(SplashSoundBuffer1);
            SplashSound2 = new Sound(SplashSoundBuffer2);
        }

        public override void Fire(Vector2f pos, float direction, Vector2f? targetPos = null)
        {
            base.Fire(pos, direction, targetPos);

            // Sound
            if (Utils.RandomInt() == 0)
                ShootSound1.Play();
            else
                ShootSound2.Play();
        }

        /// <param name="radius">If left at -1, will use ExplosionRadius.</param>
        /// <param name="directCall">If true, will not credit kill or activate pick-ups.</param>
        public override void Explode(Vector2f pos, float radius = -1, bool directCall = false)
        {
            if (radius == -1)
                radius = ExplosionRadius;
            // Collision
            bool hitSomething = false;
            int numChildren = Game.Layer_Objects.NumChildren;
            for (int i = 0; i < numChildren; i++)
            {
                dynamic obj = Game.Layer_Objects.GetChildAt(i);

                if (obj is PhysicalEntity && !obj.CanTakeDamage)
                    continue;
                if ((!(obj is CollisionEntity) || obj.Collision == null) && !Utils.InCircle(pos, radius, obj.Position)) // Collide with Position if obj is not a CollisionEntity or has no Collision
                    continue;
                if (obj.Collision is CircleShape && !Utils.CircleCircleCollision(pos, radius, obj.Position, obj.Collision.Radius))
                    continue;
                if (obj.Collision is RectangleShape && !Utils.CircleRectangleCollision(pos, radius, obj.Collision, obj.Rotation, obj.Position))
                    continue;

                if (obj is PhysicalEntity)
                {
                    // Damage
                    obj.Damage(Damage, 0, !directCall ? SourceObject : null);
                }
                else if (!directCall && obj is Pickup)
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

            if (hitSomething || Utils.InCircle(Game.Island, pos))
            {
                // Explosion
                Explosion explosion = new Explosion(Game, radius);
                explosion.Position = pos;
                Game.Layer_OtherAbove.AddChild(explosion);

                // Sound
                if (Utils.RandomInt() == 0)
                    ExplosionSound1.Play();
                else
                    ExplosionSound2.Play();
            }
            else
            {
                // Splash
                Explosion explosion = new Explosion(Game, 15);
                explosion.Position = pos;
                Game.Layer_OtherAbove.AddChild(explosion);

                // Sound
                if (Utils.RandomInt() == 0)
                    SplashSound1.Play();
                else
                    SplashSound2.Play();
            }


            if (directCall || Game.Player == null)
                return;
            if (hitSomething)
                Game.Player.IncreaseScoreMultiplier();
            else if (!Game.Player.HasPowerup(Powerup.TRIPLE_CANNON) && !Game.Player.HasPowerup(Powerup.OCTUPLE_CANNON))
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
