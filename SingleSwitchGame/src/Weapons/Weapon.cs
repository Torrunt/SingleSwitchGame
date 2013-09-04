using System;

namespace SingleSwitchGame
{
    class Weapon
    {
        protected Game Game;
        public Entity SourceObject;

        public uint Damage = 2000;
        public uint DamageType = 0;
        public float KnockBack = 0;
        /// <summary>How many things projectiles can hit before they remove (ie: penetration).</summary>
        public uint HitMax = 1;

        public string DisplayName = "";

        public Weapon(Game Game, Entity sourceObject)
        {
            this.Game = Game;
            this.SourceObject = sourceObject;

            Init();
        }
        public void Init() { }
        public void Deinit() { }
    }
}
