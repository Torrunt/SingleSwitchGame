using SFML.Window;
using SFML.Graphics;
using System;

namespace SingleSwitchGame
{
    class Ship : Character
    {
        public Ship(Game game)
            : base(game, Graphics.GetSprite("assets/sprites/ship.png"))
        {
            Model.Scale = new Vector2f(0.5f, 0.5f);
            Model.Origin = new Vector2f(265, 244);

            Collision = new RectangleShape(new Vector2f(265, 105));
            Collision.Position = new Vector2f(-132.5f, -52.5f);

            SpeedMax = 50.0f;
            Acc = 200.0f;

            SetAI(new ShipAI(Game));
        }

        public override void Update(float dt)
        {
            // Freeze Time?
            if (Game.Player != null && Game.Player.CurrentPowerup == Cannon.POWERUP_FREEZE_TIME)
                return;

            base.Update(dt);
        }
    }
}