using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using SFML.Window;

namespace SingleSwitchGame
{
    class AIManager
    {
        protected Game Game;

        public uint Wave = 1;
        public uint EnemyCount;

        public bool SpawningOverTime;
        private Timer SpawnOverTimeTimer;
        private uint SpawnOverTimeCount;
        private uint SpawnOverTimeAmount;
        private uint SpawnOverTimeType;

        // Consts
        public const uint TYPE_INFANTRYMAN = 0;
        public const uint TYPE_SHIP = 1;
        public const uint TYPE_ROWBOAT = 2;

        public const int POINTS_INFANTRY = 1;
        public const int POINTS_SHIP = 10;
        public const int POINTS_ROWBOAT = 4;

        public AIManager(Game Game)
        {
            this.Game = Game;
        }

        public void StopAll()
        {
            StopSpawnEnemiesOverTime();
        }

        public void Pause()
        {
            if (SpawnOverTimeTimer != null)
                SpawnOverTimeTimer.Enabled = false;
        }

        public void Resume()
        {
            if (SpawnOverTimeTimer != null)
                SpawnOverTimeTimer.Enabled = true;
        }

        // Wave Spawning

        public void StartWave(uint no = 1)
        {
            uint amount = 2 + (2 * (no-1));
            double interval = 2500;

            // TODO: Insert dynamically adjusting difficulty here

            SpawnEnemiesOverTime(TYPE_INFANTRYMAN, amount, interval);

            // Message
            MessageFade msg = new MessageFade(Game, "Wave " + no, 200, new Vector2f(Game.Size.X/2, Game.Size.Y/2));
            Game.Layer_GUI.AddChild(msg);
        }
        public void NextWave()
        {
            StartWave(++Wave);
        }


        public void OnEnemyDeath(object sender, EventArgs e)
        {
            if (EnemyCount > 0)
                EnemyCount--;

            // Powerup drops
            if (Utils.RandomInt(0, 3) == 0)
            {
                PowerupPickup powerup = new PowerupPickup(Game, Utils.RandomInt(1, Cannon.POWERUP_MAX));
                powerup.Position = ((DisplayObject)sender).Position;
                Game.Layer_Objects.AddChild(powerup);
            }

            // Wave Finished
            if (EnemyCount == 0 && !SpawningOverTime && Game.Player != null)
            {
                Game.Player.StopPowerup();

                Game.UpgradeMenu = new UpgradeMenuGUI(Game);
                Game.Layer_GUI.AddChild(Game.UpgradeMenu);
                Game.UpgradeMenu.Removed += OnUpgradeMenuClosed;
            }
        }
        private void OnUpgradeMenuClosed(object source, EventArgs e)
        {
            Game.UpgradeMenu.Removed -= OnUpgradeMenuClosed;
            Game.UpgradeMenu = null;

            NextWave();
        }

        // Spawning over time

        public void SpawnEnemiesOverTime(uint type, uint amount, double interval)
        {
            if (SpawnOverTimeTimer != null)
                StopSpawnEnemiesOverTime();

            SpawningOverTime = true;

            SpawnOverTimeType = type;
            SpawnOverTimeAmount = amount;
            SpawnOverTimeCount = 0;

            SpawnOverTimeTimer = new Timer(interval);
            SpawnOverTimeTimer.Elapsed += SpawnOverTimeTimerHandler;
            SpawnOverTimeTimer.Start();
        }
        public void StopSpawnEnemiesOverTime()
        {
            if (SpawnOverTimeTimer == null)
                return;
            SpawningOverTime = false;
            SpawnOverTimeTimer.Stop();
            SpawnOverTimeTimer.Elapsed -= SpawnOverTimeTimerHandler;
            SpawnOverTimeTimer = null;
        }

        private void SpawnOverTimeTimerHandler(Object source, ElapsedEventArgs e)
        {
            if (Game.Player == null)
            {
                StopSpawnEnemiesOverTime();
                return;
            }
            if (Game.Player.CurrentPowerup == Cannon.POWERUP_FREEZE_TIME)
                return;

            // Spawn enemy
            Character enemy;
            switch (SpawnOverTimeType)
            {
                case TYPE_SHIP:
                {
                    enemy = new Character(Game);
                    //enemy.SetPosition(Utils.GetPointInDirection(Game.Island.Position, Utils.RandomInt(0, 359), Game.Size.X + 100));
                    break;
                }
                default:
                {
                    enemy = new Infantryman(Game);
                    enemy.SetPosition(Utils.GetPointInDirection(Game.Island.Position, Utils.RandomInt(0, 359), Game.Island.Radius - 5));
                    break;
                }
            }

            enemy.Death += OnEnemyDeath;
            EnemyCount++;
            Game.Layer_Objects.AddChild(enemy);

            SpawnOverTimeCount++;

            if (SpawnOverTimeCount >= SpawnOverTimeAmount)
            {
                // Finish spawning enemies over time
                StopSpawnEnemiesOverTime();
            }
        }

    }
}
