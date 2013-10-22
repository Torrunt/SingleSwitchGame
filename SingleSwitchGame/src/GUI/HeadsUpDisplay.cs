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

        private Layer Layer_Powerups;
        public List<PowerupGUI> PowerupGUIItems = new List<PowerupGUI>();

        public bool DisplayFPS = false;
        private Text FPS;
        private Timer FPSUpdateTimer;
        private float DT;

        public HeadsUpDisplay(Game Game)
            : base(Game, null)
        {
            // Score
            Score = new Text("00000000", Game.TidyHand, 50);
            Score.Color = new Color(255, 255, 255, 180);
            Score.Position = new Vector2f(Game.Size.X - 270, 2);
            AddChild(Score);

            ScoreMultiplier = new Text("x001", Game.TidyHand, 35);
            ScoreMultiplier.Color = new Color(200, 200, 200, 180);
            ScoreMultiplier.Position = new Vector2f(Game.Size.X - 100, 50);
            AddChild(ScoreMultiplier);

            // Powerups
            Layer_Powerups = new Layer();
            AddChild(Layer_Powerups);

            // FPS
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
        public override void Deinit()
        {
            base.Deinit();

            if (FPSUpdateTimer != null)
            {
                FPSUpdateTimer.Stop();
                FPSUpdateTimer.Dispose();
                FPSUpdateTimer = null;
            }
        }

        public override void Update(float dt)
        {
            if (DisplayFPS)
                DT = dt;
        }

        // FPS
        protected virtual void FPSUpdate(Object source = null, ElapsedEventArgs e = null)
        {
            FPS.DisplayedString = (1 / DT).ToString("00.0");
        }

        // Score
        public void SetScore(int score)
        {
            Score.DisplayedString = score.ToString("00000000");
        }
        public void SetScoreMultiplier(int multiplier)
        {
            ScoreMultiplier.DisplayedString = "x" + multiplier.ToString("000");
        }

        // Health
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

        // Powerups
        public void AddPowerup(int type, double time = 0)
        {
            for (int i = 0; i < PowerupGUIItems.Count; i++)
            {
                if (PowerupGUIItems[i].Type == type)
                {
                    PowerupGUIItems[i].ResetTime(time);
                    return;
                }
            }

            PowerupGUI item = new PowerupGUI(this, type, time, Game.GraphicsMode);
            item.Y = Game.Size.Y - 40;
            PowerupGUIItems.Add(item);
            UpdatePowerupPositions();
            Layer_Powerups.AddChild(item);
        }
        public void RestartPowerup(int type, double time = 0)
        {
            for (int i = 0; i < PowerupGUIItems.Count; i++)
            {
                if (PowerupGUIItems[i].Type != type)
                    continue;

                PowerupGUIItems[i].ResetTime(time);
                return;
            }
        }
        public void RemovePowerup(int type)
        {
            for (int i = 0; i < PowerupGUIItems.Count; i++)
            {
                if (PowerupGUIItems[i].Type != type)
                    continue;

                Layer_Powerups.RemoveChild(PowerupGUIItems[i]);
                return;
            }
        }
        public void RemovePowerups()
        {
            while (PowerupGUIItems.Count > 0)
                Layer_Powerups.RemoveChild(PowerupGUIItems[0]);
        }

        public void UpdatePowerupPositions()
        {
            const float GAPX = 35;
            switch (PowerupGUIItems.Count)
            {
                case 5:
                {
                    PowerupGUIItems[0].X = (Game.Size.X / 2) - GAPX - GAPX - GAPX - GAPX;
                    PowerupGUIItems[1].X = (Game.Size.X / 2) - GAPX - GAPX;
                    PowerupGUIItems[2].X = (Game.Size.X / 2);
                    PowerupGUIItems[3].X = (Game.Size.X / 2) + GAPX + GAPX;
                    PowerupGUIItems[4].X = (Game.Size.X / 2) + GAPX + GAPX + GAPX + GAPX;
                    break;
                }
                case 4:
                {
                    PowerupGUIItems[0].X = (Game.Size.X / 2) - GAPX - GAPX - GAPX;
                    PowerupGUIItems[1].X = (Game.Size.X / 2) - GAPX;
                    PowerupGUIItems[2].X = (Game.Size.X / 2) + GAPX;
                    PowerupGUIItems[3].X = (Game.Size.X / 2) + GAPX + GAPX + GAPX;
                    break;
                }
                case 3:
                {
                    PowerupGUIItems[0].X = (Game.Size.X / 2) - GAPX - GAPX;
                    PowerupGUIItems[1].X = (Game.Size.X / 2);
                    PowerupGUIItems[2].X = (Game.Size.X / 2) + GAPX + GAPX;
                    break;
                }
                case 2:
                {
                    PowerupGUIItems[0].X = (Game.Size.X / 2) - GAPX;
                    PowerupGUIItems[1].X = (Game.Size.X / 2) + GAPX;
                    break;
                }
                case 1: PowerupGUIItems[0].X = Game.Size.X / 2; break;
            }
        }
    }
    

    class PowerupGUI : DisplayObject
    {
        private HeadsUpDisplay HUD;
        public int Type;
        private CircleShape Shape;
        private Text Time;
        private Timer UpdateTimer;
        private double TimeLeft;

        public PowerupGUI(HeadsUpDisplay hud, int type, double time, uint graphicsMode)
        {
            HUD = hud;
            Type = type;

            Shape = PowerupPickup.GetModel(type, graphicsMode, 160, 180);
            Shape.Scale = new Vector2f(1.5f, 1.5f);
            AddChild(Shape);

            Time = new Text("0.0", Game.TidyHand, 24);
            Time.Color = new Color(15, 15, 15, 220);
            FloatRect textRect = Time.GetLocalBounds();
            Time.Origin = new Vector2f(textRect.Left + (textRect.Width / 2.0f), textRect.Top + (textRect.Height / 2.0f));
            AddChild(Time);

            UpdateTimer = new Timer(100);
            UpdateTimer.Elapsed += OnUpdate;

            Update(time);
            UpdateTimer.Start();
        }
        public override void OnRemoved()
        {
            HUD.PowerupGUIItems.Remove(this);
            HUD.UpdatePowerupPositions();

            Deinit();
            base.OnRemoved();
        }
        public void Deinit()
        {
            if (UpdateTimer != null)
            {
                UpdateTimer.Stop();
                UpdateTimer.Dispose();
                UpdateTimer = null;
            }
        }

        public void ResetTime(double time)
        {
            TimeLeft = time;
            Update(time);
            UpdateTimer.Stop();
            UpdateTimer.Start();
        }

        private void Update(double time)
        {
            if (time < 0)
            {
                if (Parent != null)
                    Parent.RemoveChild(this);
            }
            else
            {
                Time.DisplayedString = (time / 1000).ToString("0.0");
                FloatRect textRect = Time.GetLocalBounds();
                Time.Origin = new Vector2f(textRect.Left + (textRect.Width / 2.0f), Time.Origin.Y);
            }

            TimeLeft = time;
        }
        private void OnUpdate(Object source, ElapsedEventArgs e)
        {
            // need to minus an additional 10ms each update to make sure it stays in sync with a powerup timer
            Update(TimeLeft - 110);
        }
    }
}
