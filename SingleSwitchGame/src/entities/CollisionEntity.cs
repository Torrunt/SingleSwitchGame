using SFML.Graphics;

namespace SingleSwitchGame
{
    class CollisionEntity : Entity
    {
        protected bool IgnoreEntityCollision;

        public CollisionEntity(Game game, Sprite model)
            : base(game, model)
        {
            IgnoreEntityCollision = false;
        }
    }
}
