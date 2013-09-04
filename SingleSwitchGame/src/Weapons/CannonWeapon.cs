using System;
using SFML.Graphics;

namespace SingleSwitchGame
{
    class CannonWeapon : ProjectileWeapon
    {
        public CannonWeapon(Game Game, Entity sourceObject)
            : base(Game, sourceObject)
        {
            ProjectileSpeed = 500.0f;
            //ProjectileAmount = 1;
            //ProjectileOffset = 10;
        }

    }
}
