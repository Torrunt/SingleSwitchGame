using System;
using System.Timers;
using SFML.Window;
using SFML.Graphics;

namespace SingleSwitchGame
{
    class ProjectileWeapon : Weapon
    {

        public float ProjectileSpeed = 400.0f;
        public uint ProjectileAmount = 1;
        /// <summary>If ProjectileAmount > 1, additional projetiles will be offset by this value (angle). </summary>
        public float ProjectileOffset = 10;
        public uint ProjectileLifeSpan = 3000;
        /// <summary>If not 0, projectiles will rotate at this speed while moving (in either direction)</summary>
        public float ProjectileRotateSpeed = 0;

        public bool CanShoot = true;


        public ProjectileWeapon(Game game, Entity sourceObject)
            : base(game, sourceObject) { }

        public virtual void Fire(Vector2f pos, float direction, Vector2f? targetPos = null)
        {
            if (!CanShoot)
                return;

            SpawnProjectiles(pos, direction, targetPos);
        }
        public void SpawnProjectiles(Vector2f pos, float direction, Vector2f? targetPos = null)
        {
            for (uint i = 0; i < ProjectileAmount; i++)
                SpawnProjectile(pos, direction, ProjectileAmount == 1 ? 0 : ProjectileOffset * ((float)i - (float)Math.Floor((float)ProjectileAmount / 2)), targetPos);
        }
        public virtual Projectile SpawnProjectile(Vector2f pos, float direction, float offset, Vector2f? targetPos = null)
        {
            Projectile proj = new Projectile(Game, GetProjectileModel(), this);

            // Position and Velocity
            proj.SetPosition(pos);
            proj.Rotate(direction);
            
            float angle = offset == 0 ? (float)Utils.ToRadians(direction) : (float)Utils.ToRadians(direction + offset);
            proj.Velocity = new Vector2f((float)Math.Cos(angle) * ProjectileSpeed, (float)Math.Sin(angle) * ProjectileSpeed);

            // Stats
            proj.Damage = Damage;
            proj.SetLifeSpan(ProjectileLifeSpan);
            if (targetPos.HasValue)
                proj.SetTargetPosition(targetPos.Value);
            if (ProjectileRotateSpeed != 0)
            {
                proj.RotateSpeed = ProjectileRotateSpeed*(Utils.RandomInt() == 1 ? 1 : -1);
                proj.Rotate(Utils.RandomInt(0, 359));
            }

            Game.Layer_Other.AddChild(proj);

            return proj;
        }

        public virtual void OnProjectileCollision(Projectile proj, dynamic hitTarget = null)
        {
            Explode(proj.Position);
        }
        /// <summary>The Explosion caused by the projectiles if there is one.</summary>
        public virtual void Explode(Vector2f pos)
        {
        }

        public virtual void OnProjectileLifeEnd()
        {

        }

        /// <summary>
        /// Drawing of the Project Model. Can simply override and return a Sprite if need be.
        /// </summary>
        /// <returns></returns>
        public virtual dynamic GetProjectileModel()
        {
            float radius = 10;
            CircleShape proj = new CircleShape(radius);
            proj.Origin = new Vector2f(radius, radius);
            proj.FillColor = new Color(0, 0, 0, 0);
            proj.OutlineThickness = 2;
            proj.OutlineColor = new Color(250, 250, 250);
            proj.SetPointCount(20);

            return proj;
        }

    }
}
