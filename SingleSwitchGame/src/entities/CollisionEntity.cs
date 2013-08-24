using SFML.Graphics;

namespace SingleSwitchGame
{
    class CollisionEntity : Entity
    {
        protected bool ignoreEntityCollision;

        public CollisionEntity(Game game, Sprite model)
            : base(game, model)
        {
            ignoreEntityCollision = false;
        }
    }
}
