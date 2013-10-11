using SFML.Window;
using SFML.Graphics;

namespace SingleSwitchGame
{
    class Bat : Character
    {
        public Bat(Game game)
            : base(game, Graphics.GetAnimatedSprite(game, "assets/sprites/characters/rupert/rupert.xml"))
        {
            Origin = new Vector2f(75, 90);
            //Collision = new RectangleShape(new Vector2f(Model.TextureRect.Width, Model.TextureRect.Height));

            Model.GotoAndStop(0);

            TurnAroundOnMove = true;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            // Animations
        }
    }
}
