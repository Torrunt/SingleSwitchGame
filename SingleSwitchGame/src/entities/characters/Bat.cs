using SFML.Window;


namespace SingleSwitchGame
{
    class Bat : Character
    {
        public Bat(Game game)
            : base(game, Graphics.GetSprite("assets/sprites/bat.png"))
        {
            Origin = new Vector2f(75, 90);
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            // Animations
        }
    }
}
