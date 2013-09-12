using System;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using System.Timers;

namespace SingleSwitchGame
{
    class HeadsUpDisplay : Entity
    {

        private Text Score;
        private Text ScoreMultiplier;
        private Text Health;

        public bool DisplayFPS = true;
        private Text FPS;
        private Timer FPSUpdateTimer;
        private float DT;

        public HeadsUpDisplay(Game Game)
            : base(Game, null)
        {
            Score = new Text("00000000", Game.TidyHand, 50);
            Score.Color = new Color(255, 255, 255, 180);
            Score.Position = new Vector2f(Game.Size.X - 270, 2);
            AddChild(Score);

            ScoreMultiplier = new Text("x001", Game.TidyHand, 35);
            ScoreMultiplier.Color = new Color(200, 200, 200, 180);
            ScoreMultiplier.Position = new Vector2f(Game.Size.X - 100, 50);
            AddChild(ScoreMultiplier);
            
            Health = new Text("0", Game.TidyHand, 50);
            Health.Color = new Color(255, 20, 20, 180);
            Health.Position = new Vector2f(2, 2);
            AddChild(Health);

            if (DisplayFPS)
            {
                FPS = new Text("00.0", Game.TidyHand, 30);
                FPS.Position = new Vector2f(Game.Size.X - 70, Game.Size.Y - 40);
                AddChild(FPS);

                FPSUpdateTimer = new Timer(500); // Update every 0.5 seconds
                FPSUpdateTimer.Elapsed += FPSUpdate;
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

        public void SetScore(int score)
        {
            Score.DisplayedString = score.ToString("00000000");
        }
        public void SetScoreMultiplier(int multiplier)
        {
            ScoreMultiplier.DisplayedString = "x" + multiplier.ToString("000");
        }

        public void SetHealth(uint health)
        {
            Health.DisplayedString = health.ToString();
        }
    }
}
