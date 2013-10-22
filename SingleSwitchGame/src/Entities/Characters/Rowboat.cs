using SFML.Window;
using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace SingleSwitchGame
{
    class Rowboat : Character
    {
        private List<Infantryman> InfantryPassengers = new List<Infantryman>();

        public Rowboat(Game game)
            : base(game, game.GraphicsMode == Game.GRAPHICSMODE_NORMAL ? Graphics.GetAnimatedSprite(game, "assets/sprites/rowboat.xml") : Graphics.GetAnimatedSprite(game, "assets/sprites/blueprint/rowboat.xml"))
        {
            Collision = new RectangleShape(new Vector2f(82f, 38f));
            Collision.Position = new Vector2f(-41f, -19f);

            HealthMax = 8000;
            Health = HealthMax;

            SpeedMax = 50.0f + Math.Min(0.5f * Game.AIManager.Difficulty, 25.0f);
            Acc = 200.0f;

            // Add Infantry Passengers
            AddInfantryman(new Vector2f(0, -10));
            AddInfantryman(new Vector2f(0, 10));

            AddInfantryman(new Vector2f(25, -10));
            AddInfantryman(new Vector2f(25, 10));

            AddInfantryman(new Vector2f(-25, -10));
            AddInfantryman(new Vector2f(-25, 10));

            SetAI(new RowboatAI(Game));
        }

        public override void Update(float dt)
        {
            // Freeze Time?
            if (Game.Player != null && Game.Player.HasPowerup(Powerup.FREEZE_TIME))
            {
                Model.Stop();
                return;
            }

            if ((AI == null || !((RowboatAI)AI).ReachedBeach) && !Model.Playing)
                Model.Play();

            base.Update(dt);
        }

        protected override void OnDeath(dynamic sourceObject = null)
        {
            base.OnDeath((object)sourceObject);

            // Explosions
            Explosion explosion = new Explosion(Game, 60);
            explosion.Position = Position;
            Game.Layer_OtherAbove.AddChild(explosion);
        }

        public void RemoveInfantry()
        {
            while (InfantryPassengers.Count > 0)
            {
                if (InfantryPassengers[0].Parent != null)
                    InfantryPassengers[0].Parent.RemoveChild(InfantryPassengers[0]);
                InfantryPassengers.RemoveAt(0);
            }
        }
        private void AddInfantryman(Vector2f position)
        {
            Infantryman infantryman = new Infantryman(Game, 1, null);
            infantryman.Visible = true;
            infantryman.SetPosition(position);
            AddChild(infantryman);
            InfantryPassengers.Add(infantryman);
        }
        public int AmountOfInfantry { get { return InfantryPassengers.Count; } set { } }
    }
}