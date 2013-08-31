using System;

namespace SingleSwitchGame
{
    class Weapon
    {
        protected Game game;
        public Entity SourceObject;

        public uint Damage = 2000;
        public uint DamageType = 0;
        public float KnockBack = 0;
        /// <summary>How many things projectiles can hit before they remove (ie: penetration).</summary>
        public uint HitMax = 1;

        public string DisplayName = "";

        public Weapon(Game game, Entity sourceObject)
        {
            this.game = game;
            this.SourceObject = sourceObject;

            Init();
        }
        public void Init() { }
        public void Deinit() { }
    }
}
