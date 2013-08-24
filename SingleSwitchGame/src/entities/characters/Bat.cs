using SFML.Window;


namespace SingleSwitchGame
{
    class Bat : Character
    {
        public Bat(Game game)
            : base(game, Graphics.getSprite("assets/sprites/bat.png"))
        {
            Origin = new Vector2f(75, 90);
        }

        public override void update(float dt)
        {
            base.update(dt);

            // Animations
        }
    }
}
