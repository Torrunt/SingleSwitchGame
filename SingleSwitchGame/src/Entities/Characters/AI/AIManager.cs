﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace SingleSwitchGame
{
    class AIManager
    {
        protected Game Game;

        public const int POINTS_INFANTRY = 1;

        private Timer TestInfantrySpawnTimer;

        public AIManager(Game Game)
        {
            this.Game = Game;
        }

        public void StartTestInfantryTimer()
        {
            TestInfantrySpawnTimer = new Timer(2500);
            TestInfantrySpawnTimer.Elapsed += TestInfantrySpawnTimerHandler;
            TestInfantrySpawnTimer.Start();
        }
        public void StopTestInfantryTimer()
        {
            if (TestInfantrySpawnTimer == null)
                return;

            TestInfantrySpawnTimer.Stop();
            TestInfantrySpawnTimer.Elapsed -= TestInfantrySpawnTimerHandler;
            TestInfantrySpawnTimer = null;
        }
        private void TestInfantrySpawnTimerHandler(Object source, ElapsedEventArgs e)
        {
            if (Game.Player == null)
            {
                StopTestInfantryTimer();
                return;
            }

            Infantryman infantryman = new Infantryman(Game);
            infantryman.SetPosition(Utils.GetPointInDirection(Game.Island.Position, Utils.RandomInt(0, 359), Game.Island.Radius-5));
            Game.Layer_Objects.AddChild(infantryman);
        }
    }
}
