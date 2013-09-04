using SFML.Graphics;

namespace SingleSwitchGame
{
    class CollisionEntity : Entity
    {
        protected bool IgnoreEntityCollision;

        public CollisionEntity(Game Game, dynamic model = null)
            : base(Game, (object)model)
        {
            IgnoreEntityCollision = false;
        }
    }
}
