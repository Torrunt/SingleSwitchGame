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
        public uint Difficulty = 1;
        public uint EnemyCount = 0;

        /// <summary>The amount of times the player has been hit this wave.</summary>
        public int TimesHitThisWave = 0;

        public bool SpawningOverTime;
        private Timer SpawnOverTimeTimer;
        private uint SpawnOverTimeCount;
        private uint SpawnOverTimeAmount;
        private uint SpawnOverTimeType;

        // Consts
        public const uint TYPE_INFANTRYMAN = 0;
        public const uint TYPE_SHIP = 1;
        public const uint TYPE_ROWBOAT = 2;

        public const int POINTS_INFANTRYMAN = 1;
        public const int POINTS_SHIP = 15;
        public const int POINTS_SHIP_EMPTY = 5;
        public const int POINTS_ROWBOAT = 8;
        public const int POINTS_ROWBOAT_EMPTY = 0;

        public AIManager(Game Game)
        {
            this.Game = Game;
        }

        public void StopAll()
        {
            Wave = 1;
            Difficulty = 1;

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

        public void StartWave()
        {
            uint amount;
            switch (Difficulty)
            {
                case  1: amount = 1; break;
                case  2: amount = 3; break;
                default: amount = 1 + Difficulty - (uint)Math.Floor((Difficulty-1) / 2f); break;
            }
            double interval = 8000 - Math.Min(40 * Difficulty, 4000);

            SpawnEnemiesOverTime(TYPE_SHIP, amount, interval);
        }
        public void NextWave()
        {
            Wave++;
            if (TimesHitThisWave == 0)
                Difficulty++;
            else
                Difficulty -= (uint)((float)Difficulty * ((float)TimesHitThisWave / 10f));

            TimesHitThisWave = 0;

            StartWaveCountdown();
        }


        private int WaveStartCountDownNo = 0;
        public void StartWaveCountdown()
        {
            // Countdown
            WaveStartCountDownNo = 4;
            MessageFade msg = new MessageFade(Game, "Wave " + Wave, 200, new Vector2f(Game.Size.X / 2, Game.Size.Y / 2), WaveStartCountDown, 800);
            Game.Layer_GUI.AddChild(msg);
        }
        private void WaveStartCountDown()
        {
            WaveStartCountDownNo--;

            MessageFade msg;
            if (WaveStartCountDownNo == 1)
                msg = new MessageFade(Game, WaveStartCountDownNo.ToString(), 200, new Vector2f(Game.Size.X / 2, Game.Size.Y / 2), null, 100);
            else
                msg = new MessageFade(Game, WaveStartCountDownNo.ToString(), 200, new Vector2f(Game.Size.X / 2, Game.Size.Y / 2), WaveStartCountDown, 100);
            Game.Layer_GUI.AddChild(msg);

            if (WaveStartCountDownNo == 1)
                StartWave();
        }


        public void OnEnemyDeath(object sender, EventArgs e)
        {
            EnemyRemoved(sender, true);
        }
        public void EnemyRemoved(object enemy, bool death)
        {
            if (EnemyCount > 0)
                EnemyCount--;
            if (Game.Player == null)
                return;

            if (death)
            {
                int powerupDropChance = 3;

                // Increase Score
                if (enemy is Ship)
                {
                    Game.Player.IncreaseScore(((Ship)enemy).AmountOfInfantry == 0 ? POINTS_SHIP_EMPTY : POINTS_SHIP);
                    powerupDropChance = 2;
                }
                else if (enemy is Infantryman)
                {
                    Game.Player.IncreaseScore(POINTS_INFANTRYMAN);
                    powerupDropChance = 20;
                }
                else if (enemy is Rowboat)
                {
                    Game.Player.IncreaseScore(((Rowboat)enemy).AmountOfInfantry == 0 ? POINTS_ROWBOAT_EMPTY : POINTS_ROWBOAT);
                    powerupDropChance = 3;
                }

                // Powerup drops
                if (Utils.RandomInt(1, powerupDropChance) == 1)
                {
                    PowerupPickup powerup = new PowerupPickup(Game, Utils.RandomInt(1, Powerup.MAX));
                    powerup.Position = ((DisplayObject)enemy).Position;
                    Game.Layer_Objects.AddChild(powerup);
                }
            }

            // Wave Finished
            if (EnemyCount == 0 && !SpawningOverTime)
            {
                Game.Player.StopPowerups();

                Game.UpgradeMenu = new UpgradeMenu(Game);
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
                SpawnOverTimeTimer.Stop();

            SpawningOverTime = true;

            SpawnOverTimeType = type;
            SpawnOverTimeAmount = amount;
            SpawnOverTimeCount = 0;

            if (SpawnOverTimeTimer == null)
            {
                SpawnOverTimeTimer = new Timer(interval);
                SpawnOverTimeTimer.Elapsed += SpawnOverTimeTimerHandler;
            }
            else
                SpawnOverTimeTimer.Interval = interval;
            SpawnOverTimeTimer.Start();

            // spawn one straight away
            SpawnOverTimeTimerHandler();
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

        private void SpawnOverTimeTimerHandler(Object source = null, ElapsedEventArgs e = null)
        {
            if (Game.Player == null)
            {
                StopSpawnEnemiesOverTime();
                return;
            }
            if (Game.Player.HasPowerup(Powerup.FREEZE_TIME))
                return;

            // Spawn enemy
            Character enemy;
            switch (SpawnOverTimeType)
            {
                case TYPE_SHIP:
                {
                    enemy = new Ship(Game);
                    int angle = Utils.RandomInt(0, 359);
                    Vector2f intersectPoint = Utils.RaycastAgainstBounds(Game.Bounds, Game.Center, Utils.GetPointInDirection(Game.Center, angle, Game.Size.X));
                    enemy.SetPosition(Utils.GetPointInDirection(intersectPoint, angle, 200));
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

        // Enemy Spawning

        public object SpawnEnemy(uint type, Vector2f position)
        {
            Character enemy;
            switch (type)
            {
                case TYPE_SHIP: enemy = new Ship(Game); break;
                case TYPE_ROWBOAT: enemy = new Rowboat(Game); break;
                default: enemy = new Infantryman(Game); break;
            }

            enemy.Position = position;

            enemy.Death += OnEnemyDeath;
            EnemyCount++;
            Game.Layer_Objects.AddChild(enemy);

            return enemy;
        }

        public void OnShipReachedBeach(Ship ship)
        {
            const float gapX = 4;

            for (int i = 0; i < ship.AmountOfInfantry; i++)
            {
                Infantryman enemy = new Infantryman(Game, Utils.RandomInt(0, 5000), ship);
                enemy.SetPosition(Utils.GetPointInDirection(
                    Game.Island.Position, 
                    (float)Utils.GetAngle(Game.Island.Position, ship.Position) - ((ship.AmountOfInfantry / 2) * gapX) + (i * gapX), 
                    Game.Island.Radius - 5)
                    );

                enemy.Death += OnEnemyDeath;
                EnemyCount++;
                Game.Layer_Objects.AddChild(enemy);
            }
        }
        public void OnRowboatReachedBeach(Rowboat rowboat)
        {
            const float gapX = 4;

            for (int i = 0; i < rowboat.AmountOfInfantry; i++)
            {
                Infantryman enemy = new Infantryman(Game);
                enemy.SetPosition(Utils.GetPointInDirection(
                    Game.Island.Position,
                    (float)Utils.GetAngle(Game.Island.Position, rowboat.Position) - ((rowboat.AmountOfInfantry / 2) * gapX) + (i * gapX),
                    Game.Island.Radius - 5)
                    );

                enemy.Death += OnEnemyDeath;
                EnemyCount++;
                Game.Layer_Objects.AddChild(enemy);
            }
        }

    }
}
