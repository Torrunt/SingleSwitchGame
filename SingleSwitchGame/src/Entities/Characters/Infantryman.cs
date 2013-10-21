using SFML.Window;
using SFML.Graphics;
using System;
using System.Timers;

namespace SingleSwitchGame
{
    class Infantryman : Character
    {

        /// <summary>The ship this infantry is/will be spawned from (used only if spawnDelay is not 0).</summary>
        private Ship Ship;
        private Timer SpawnDelayTimer;

        /// <param name="spawnDelay">In ms. Used to easily allow infantry to gradually move out of a ship.</param>
        /// <param name="ship">The ship this infantry is/will be spawned from (used only if spawnDelay is not 0).</param>
        public Infantryman(Game game, int spawnDelay = 0, Ship ship = null)
            : base(game, null)
        {
            Model = new CircleShape(4, 12);
            if (Game.GraphicsMode == Game.GRAPHICSMODE_NORMAL)
            {
                Model.FillColor = new Color((byte)Utils.RandomInt(0, 255), (byte)Utils.RandomInt(0, 255), (byte)Utils.RandomInt(0, 255));
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

            if (spawnDelay == 0)
            {
                SetAI(new InfantrymanAI(Game));
            }
            else
            {
                CanTakeDamage = false;
                Visible = false;
                if (ship == null)
                    return;
                Ship = ship;
                SpawnDelayTimer = new Timer(spawnDelay);
                SpawnDelayTimer.AutoReset = false;
                SpawnDelayTimer.Elapsed += OnSpawn;
                SpawnDelayTimer.Start();
            }
        }
        public override void Deinit()
        {
            base.Deinit();

            if (SpawnDelayTimer != null)
            {
                SpawnDelayTimer.Stop();
                SpawnDelayTimer = null;
            }
        }
        private void OnSpawn(Object source, ElapsedEventArgs e)
        {
            SpawnDelayTimer.Stop();
            SpawnDelayTimer = null;

            if (Ship != null && !Ship.IsDead())
            {
                // Spawn
                Ship.RemoveInfantryman();
                CanTakeDamage = true;
                Visible = true;
                SetAI(new InfantrymanAI(Game));
            }
            else
            {
                if (Parent != null)
                    Parent.RemoveChild(this);
                Game.AIManager.EnemyRemoved(this, false);
            }

            Ship = null;
        }

        public override void Update(float dt)
        {
            // Ship blew-up before infantryman got off?
            if (SpawnDelayTimer != null && (Ship == null || Ship.IsDead()))
            {
                if (Parent != null)
                    Parent.RemoveChild(this);
                Game.AIManager.EnemyRemoved(this, false);
            }

            if (Game.Player != null)
            {
                // Freeze Time?
                if (Game.Player.HasPowerup(Powerup.FREEZE_TIME))
                    return;
                // Red Hot Beach?
                if (Game.Player.HasPowerup(Powerup.RED_HOT_BEACH))
                    Damage(10, DamageType.FIRE);
            }
            base.Update(dt);
        }
    }
}
