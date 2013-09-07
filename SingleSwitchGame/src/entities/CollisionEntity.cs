using SFML.Graphics;

namespace SingleSwitchGame
{
    class CollisionEntity : Entity
    {
        public Shape Collision;
        protected bool IgnoreEntityCollision;

        public CollisionEntity(Game Game, dynamic model = null)
            : base(Game, (object)model)
        {
            IgnoreEntityCollision = false;
        }


        public void Debug_ShowCollision(bool value = true)
        {
            if (Collision == null)
                return;

            if (value)
            {
                Collision.FillColor = new Color(0, 255, 0, 100);
                AddChild(Collision);
            }
            else
            {
                RemoveChild(Collision);
            }
        }

    }
}
