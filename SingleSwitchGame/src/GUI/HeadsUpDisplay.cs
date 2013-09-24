using System;
using System.Collections.Generic;
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
        private List<DisplayObject> HealthPoints = new List<DisplayObject>(); 
        private readonly Vector2f HEALTH_START = new Vector2f(2, 2);

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
            if (health > HealthPoints.Count)
            {
                for (int i = HealthPoints.Count; i < health; i++)
                {
                    DisplayObject bar = new DisplayObject();
                    int barWidth = 1;
                    if (Game.GraphicsMode == Game.GRAPHICSMODE_NORMAL)
                    {
                        Sprite bar_sprite = Graphics.GetSprite("assets/sprites/gui/hp.png");
                        bar_sprite.Color = new Color(255, 255, 255, 200);
                        barWidth = bar_sprite.TextureRect.Width;
                        bar.AddChild(bar_sprite);
                    }
                    else
                    {
                        RectangleShape bar_rectangle = new RectangleShape(new Vector2f(28, 60));
                        bar_rectangle.FillColor = new Color(255, 255, 255, 50);
                        bar_rectangle.OutlineColor = new Color(255, 255, 255, 180);
                        bar_rectangle.OutlineThickness = 1;
                        barWidth = (int)bar_rectangle.Size.X + 4;
                        bar.AddChild(bar_rectangle);
                    }

                    bar.Position = new Vector2f(HEALTH_START.X + ((barWidth + 2) * i), HEALTH_START.Y);
                    AddChild(bar);
                    HealthPoints.Add(bar);
                }
            }
            else if (health < HealthPoints.Count)
            {
                for (int i = HealthPoints.Count-1; i >= health; i--)
                {
                    HealthPoints[i].Parent.RemoveChild(HealthPoints[i]);
                    HealthPoints.RemoveAt(i);
                }
            }
        }
    }
}
