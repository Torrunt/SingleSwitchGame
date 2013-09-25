using SFML.Window;
using SFML.Graphics;
using System;

namespace SingleSwitchGame
{
    class Infantryman : Character
    {
        public Infantryman(Game game)
            : base(game, null)
        {
            Model = new CircleShape(4, 12);
            if (Game.GraphicsMode == Game.GRAPHICSMODE_NORMAL)
            {
                Model.FillColor = new Color(200, 0, 0);
                Model.OutlineColor = new Color(0, 0, 0);
                Model.OutlineThickness = 3;
            }
            else if (Game.GraphicsMode == Game.GRAPHICSMODE_BLUEPRINT)
            {
                Model.FillColor = new Color(0, 0, 0, 0);
                Model.OutlineColor = new Color(255, 255, 255);
                Model.OutlineThickness = 2;
            }
            AddChild(Model);
            Origin = new Vector2f(Model.Radius, Model.Radius);

            Collision = Model;

            HealthMax = 4000;
            Health = HealthMax;

            //SpeedMax = 100.0f;
            //Acc = 400.0f;
            SpeedMax = 20.0f;
            Acc = 80.0f;
            Friction = 1000.0f;

            SetAI(new InfantrymanAI(Game));
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            // Animations
        }

        protected override void OnDeath(dynamic sourceObject = null)
        {
            base.OnDeath((object)sourceObject);

            if (sourceObject is Cannon)
                sourceObject.IncreaseScore(AIManager.POINTS_INFANTRY);
        }
    }
}
