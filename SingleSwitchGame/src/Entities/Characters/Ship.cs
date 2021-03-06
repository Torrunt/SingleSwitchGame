﻿using SFML.Window;
using SFML.Graphics;
using System;

namespace SingleSwitchGame
{
    class Ship : Character
    {
        public int AmountOfInfantry;

        public Ship(Game game)
            : base(game)
        {
            if (Game.GraphicsMode == Game.GRAPHICSMODE_NORMAL)
            {
                Model = Graphics.GetAnimatedSprite(game, "assets/sprites/ship.xml");
            }
            else
            {
                Model = Graphics.GetSprite("assets/sprites/blueprint/ship.png");
                Model.Scale = new Vector2f(0.5f, 0.5f);
                Model.Origin = new Vector2f(265, 244);
            }
            AddChild(Model);

            Collision = new RectangleShape(new Vector2f(265, 105));
            Collision.Position = new Vector2f(-132.5f, -52.5f);

            SpeedMax = 50.0f + Math.Min(0.5f * Game.AIManager.Difficulty, 25.0f);
            Acc = 200.0f;
            
            AmountOfInfantry = Utils.RandomInt(12, 18);

            SetAI(new ShipAI(Game));
        }

        public override void Update(float dt)
        {
            // Freeze Time?
            if (Game.Player != null && Game.Player.HasPowerup(Powerup.FREEZE_TIME))
            {
                if (Model is AnimatedSprite)
                    Model.Stop();
                return;
            }

            if (Model != null && Model is AnimatedSprite && !Model.Playing)
                Model.Play();

            base.Update(dt);
        }

        protected override void OnDeath(dynamic sourceObject = null)
        {
            // Explosions
            Explosion explosion = new Explosion(Game, 100);
            explosion.Position = Position;
            Game.Layer_OtherAbove.AddChild(explosion);

            explosion = new Explosion(Game, 60);
            explosion.Position = Utils.GetPointInDirection(Position, Rotation, 40);
            Game.Layer_OtherAbove.AddChild(explosion);

            explosion = new Explosion(Game, 60);
            explosion.Position = Utils.GetPointInDirection(Position, Rotation, -40);
            Game.Layer_OtherAbove.AddChild(explosion);

            // Deploy Rowboats
            if (AmountOfInfantry <= 6 || (AI != null && ((ShipAI)AI).ReachedBeach))
            {
                base.OnDeath((object)sourceObject);
                return;
            }

            int amountOfRowboats = Utils.RandomInt(0, 2);
            for (int i = 0; i < amountOfRowboats; i++)
            {
                Rowboat rowboat = (Rowboat)Game.AIManager.SpawnEnemy(AIManager.TYPE_ROWBOAT, Utils.GetPointInDirection(Position, i == 0 ? 90 : -90, 100));
                rowboat.Rotation = Rotation;
            }

            base.OnDeath((object)sourceObject);
        }

        public void RemoveInfantryman()
        {
            AmountOfInfantry--;
        }
    }
}