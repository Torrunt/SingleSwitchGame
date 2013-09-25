using System;
using System.Collections.Generic;
using System.ComponentModel;
using SFML.Graphics;
using SFML.Window;
using System.Timers;

namespace SingleSwitchGame
{
    class WaterRipplePoint
    {
        public Vector2f Position;
        public Vector2f Origin;
        public Vector2f MoveDirection;
        public float Speed;

        /// <summary> Reference to the connecting WaterRipplePoint above.</summary>
        public WaterRipplePoint PointAbove;
        /// <summary> Reference to the connecting WaterRipplePoint on the left.</summary>
        public WaterRipplePoint PointLeft;

        public WaterRipplePoint(float x, float y)
        {
            Position = new Vector2f(x, y);
            Origin = new Vector2f(x, y);
            MoveDirection = new Vector2f(Utils.RandomInt() == 1 ? 1 : -1, Utils.RandomInt() == 1 ? 1 : -1);

            Speed = WaterRipples.MOVE_SPEED;
        }

        public void Update(float dt)
        {
            if (MoveDirection.X == 1 && X + ((Speed * MoveDirection.X) * dt) > Origin.X + WaterRipples.DISPLACEMENT_MAX)
                MoveDirection.X = -1;
            else if (MoveDirection.X == -1 && X + ((Speed * MoveDirection.X) * dt) < Origin.X - WaterRipples.DISPLACEMENT_MAX)
                MoveDirection.X = 1;

            if (MoveDirection.Y == 1 && Y + ((Speed * MoveDirection.Y) * dt) > Origin.Y + WaterRipples.DISPLACEMENT_MAX)
                MoveDirection.Y = -1;
            else if (MoveDirection.Y == -1 && Y + ((Speed * MoveDirection.Y) * dt) < Origin.Y - WaterRipples.DISPLACEMENT_MAX)
                MoveDirection.Y = 1;

            Position = new Vector2f(X + ((Speed * MoveDirection.X) * dt), Y + ((Speed * MoveDirection.Y) * dt));
        }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2f(value, Position.Y); }
        }
        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2f(Position.X, value); }
        }   
    }

    class WaterRipples : DisplayObject
    {
        protected Game Game;

        private List<List<WaterRipplePoint>> Points = new List<List<WaterRipplePoint>>();

        private List<RectangleShape> Lines = new List<RectangleShape>(); 
        private List<CircleShape> Corners = new List<CircleShape>();

        public const int DISPLACEMENT_MAX = 15;
        public const float MOVE_SPEED = 6f;

        private Timer UpdateTimer; // WaterRipples uses a seperate Update Timer from the game to reduce slow-down
        private float UpdateTimerDeltaTime;

        public WaterRipples(Game Game, Vector2f Size, int Gap = 90, int LineThickness = 8, Color Colour = default(Color))
        {
            this.Game = Game;
            //Colour = new Color(94, 190, 255, 255);

            // Generate Points
            int GAP_HALF = Gap/2;
            int yc = 0;
            for (int py = -GAP_HALF; py < Size.Y + Gap; py += Gap)
            {
                Points.Add(new List<WaterRipplePoint>());
                for (int px = -GAP_HALF; px < Size.X + Gap; px += Gap)
                {
                    WaterRipplePoint p = new WaterRipplePoint(px, py);
                    if (Points[yc].Count != 0)
                        p.PointLeft = Points[yc][Points[yc].Count-1];
                    if (yc != 0)
                        p.PointAbove = Points[yc-1][Points[yc].Count];

                    Points[yc].Add(p);
                }
                yc++;
            }

            // add random additional points
            int yCount = Points.Count;
            for (int py = 1; py < yCount; py++)
            {
                int xCount = Points[py].Count;
                int px;
                for (px = 1; px < xCount; px++)
                {
                    // add random additional points along the y axis (just add them at the end of the current x List)
                    if (Utils.RandomInt(0, 3) == 0)
                        continue;

                    WaterRipplePoint pointY = new WaterRipplePoint(Points[py][px].X + (Utils.RandomInt((int)(Gap * 0.1), (int)(Gap * 0.4)) * (Utils.RandomInt() == 1 ? 1 : -1)),
                            Points[py][px].Y + GAP_HALF);
                    pointY.PointAbove = Points[py][px];
                    Points[py].Add(pointY);
                    if (py < Points.Count-1)
                        Points[py + 1][px].PointAbove = pointY;
                }
                for (px = 1; px < xCount; px++)
                {
                    // add random additional points along the x axis
                    if (Utils.RandomInt(0, 3) == 0)
                        continue;

                    WaterRipplePoint pointX = new WaterRipplePoint(Points[py][px - 1].X + GAP_HALF,
                        Points[py][px].Y + (Utils.RandomInt((int)(Gap * 0.1), (int)(Gap * 0.4)) * (Utils.RandomInt() == 1 ? 1 : -1)));
                    pointX.PointLeft = Points[py][px - 1];
                    pointX.MoveDirection.X = Utils.RandomInt() == 1 || px == Points[py].Count - 1 ? Points[py][px - 1].MoveDirection.X : Points[py][px + 1].MoveDirection.X;
                    Points[py].Insert(px, pointX);
                    Points[py][px + 1].PointLeft = Points[py][px];
                    px++;
                    xCount++;
                }
            }

            Displace();

            // Draw
            for (int py = 1; py < Points.Count; py++)
            {
                for (int px = 1; px < Points[py].Count; px++)
                {
                    RectangleShape l;

                    if (Points[py][px].PointLeft != null)
                    {
                        l = new RectangleShape(new Vector2f(Utils.Distance(Points[py][px].PointLeft.Position, Points[py][px].Position), LineThickness));
                        l.FillColor = Colour;
                        l.Origin = new Vector2f(0, LineThickness / 2);
                        l.Position = Points[py][px].Position;
                        l.Rotation = (float)Utils.GetAngle(Points[py][px].Position, Points[py][px].PointLeft.Position);
                        AddChild(l);
                        Lines.Add(l);
                    }
                    if (Points[py][px].PointAbove != null)
                    {
                        l = new RectangleShape(new Vector2f(Utils.Distance(Points[py][px].PointAbove.Position, Points[py][px].Position), LineThickness));
                        l.FillColor = Colour;
                        l.Origin = new Vector2f(0, LineThickness / 2);
                        l.Position = Points[py][px].Position;
                        l.Rotation = (float)Utils.GetAngle(Points[py][px].Position, Points[py][px].PointAbove.Position);
                        AddChild(l);
                        Lines.Add(l);
                    }

                    CircleShape corner = new CircleShape((Points[py][px].PointLeft == null || Points[py][px].PointAbove == null) ? LineThickness / 2 : LineThickness);
                    corner.FillColor = Colour;
                    corner.Origin = new Vector2f(corner.Radius, corner.Radius);
                    corner.Position = Points[py][px].Position;
                    AddChild(corner);
                    Corners.Add(corner);
                }
            }
        }

        public override void OnAdded()
        {
            base.OnAdded();

            UpdateTimer = new Timer(100);
            UpdateTimerDeltaTime = (float)(UpdateTimer.Interval/1000);
            UpdateTimer.Elapsed += Tick;
            UpdateTimer.Start();
        }

        public override void OnRemoved()
        {
            base.OnRemoved();

            if (UpdateTimer == null)
                return;
            UpdateTimer.Stop();
            UpdateTimer.Elapsed -= Tick;
            UpdateTimer = null;
        }

        public void Displace()
        {
            for (int py = 1; py < Points.Count; py++)
            {
                for (int px = 1; px < Points[py].Count; px++)
                {
                    Points[py][px].X += Utils.RandomInt(-DISPLACEMENT_MAX, DISPLACEMENT_MAX);
                    Points[py][px].Y += Utils.RandomInt(-DISPLACEMENT_MAX, DISPLACEMENT_MAX);
                }
            }
        }

        private void Tick(Object source, ElapsedEventArgs e)
        {
            if (!Game.IsRunning())
                return;

            int l = 0;
            int c = 0;
            for (int py = 0; py < Points.Count; py++)
            {
                for (int px = 0; px < Points[py].Count; px++)
                {
                    // Update Point
                    Points[py][px].Update(UpdateTimerDeltaTime);

                    // Update Display Objects
                    if (px == 0 || py == 0)
                        continue;

                    if (Points[py][px].PointLeft != null)
                    {
                        Lines[l].Size = new Vector2f(Utils.Distance(Points[py][px].PointLeft.Position, Points[py][px].Position), Lines[l].Size.Y);
                        Lines[l].Position = Points[py][px].Position;
                        Lines[l].Rotation = (float)Utils.GetAngle(Points[py][px].Position, Points[py][px].PointLeft.Position);
                        l++;
                    }
                    if (Points[py][px].PointAbove != null)
                    {
                        Lines[l].Size = new Vector2f(Utils.Distance(Points[py][px].PointAbove.Position, Points[py][px].Position), Lines[l].Size.Y);
                        Lines[l].Position = Points[py][px].Position;
                        Lines[l].Rotation = (float)Utils.GetAngle(Points[py][px].Position, Points[py][px].PointAbove.Position);
                        l++;
                    }

                    Corners[c++].Position = Points[py][px].Position;
                }
            }
            
        }
    }
}
