using SFML.Window;
using SFML.Graphics;

namespace SingleSwitchGame
{
    class Bat : Character
    {
        public Bat(Game Game)
            : base(Game, Graphics.GetSprite("assets/sprites/testing/bat.png"))
        {
            Origin = new Vector2f(75, 90);
            Collision = new RectangleShape(new Vector2f(Model.TextureRect.Width, Model.TextureRect.Height));

            TurnAroundOnMove = true;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            // Animations
        }
    }
}
