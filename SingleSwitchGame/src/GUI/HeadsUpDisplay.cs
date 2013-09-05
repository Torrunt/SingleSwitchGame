using System;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using System.Timers;

namespace SingleSwitchGame
{
    class HeadsUpDisplay : Entity
    {

        public bool DisplayFPS = true;
        private Text FPS;
        private Timer FPSUpdateTimer;
        private float DT;

        public HeadsUpDisplay(Game Game)
            : base(Game, null)
        {
            if (DisplayFPS)
            {
                FPS = new Text("00.0", Game.TidyHand, 30);
                FPS.Position = new Vector2f(Game.Size.X - 70, Game.Size.Y - 40);
                Game.Layer_GUI.AddChild(FPS);

                FPSUpdateTimer = new Timer(500); // Update every 0.5 seconds
                FPSUpdateTimer.Elapsed += new ElapsedEventHandler(FPSUpdate);
                FPSUpdateTimer.Start();
            }
        }

        public override void Update(float dt)
        {
            if (DisplayFPS)
                DT = dt;
        }
        protected virtual void FPSUpdate(Object source = null, ElapsedEventArgs e = null)
        {
            FPS.DisplayedString = (1 / DT).ToString("00.0");
        }
    }
}
