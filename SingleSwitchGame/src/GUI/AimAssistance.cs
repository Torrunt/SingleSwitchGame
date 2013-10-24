using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame.GUI
{
    class AimAssistance : Entity
    {
        public Cannon SourceObject;

        private const float RETICLE_START_Y =  75;

        public DisplayObject Reticle;
        public CircleShape Circle;
        public RectangleShape CircleLine;

        public RectangleShape LineCenter;
        public RectangleShape LineLeft;
        public RectangleShape LineRight;

        public ConvexShape Fill;

        public Color Colour;
        public Color FillColour;
        public int Thickness;

        private bool Aiming = false;
        private int AimDirection = 1;

        public float RotationOffset = 0;

        public AimAssistance(Game Game, Cannon SourceObject)
            : base(Game)
        {
            this.SourceObject = SourceObject;

            if (Game.GraphicsMode == Game.GRAPHICSMODE_NORMAL)
            {
                Colour = new Color(0, 0, 0, 192);
                Thickness = 3;
                FillColour = new Color(255, 255, 255, 25);
            }
            else if (Game.GraphicsMode == Game.GRAPHICSMODE_BLUEPRINT)
            {
                Colour = new Color(255, 255, 255, 255);
                Thickness = 1;
                FillColour = new Color(255, 255, 255, 15);
            }
            

            // Reticle
            Reticle = new DisplayObject();
            Reticle.Visible = false;
            AddChild(Reticle);

            UpdateReticle();

            // Aiming Lines
            LineCenter = new RectangleShape(new Vector2f(Utils.Distance(new Vector2f(0, 48), new Vector2f(0, Game.Size.X - 48)), Thickness));
            LineCenter.FillColor = Colour;
            LineCenter.Origin = new Vector2f(0, Thickness / 2);
            LineCenter.Position = new Vector2f(0, 48);
            LineCenter.Rotation = 90;
            AddChild(LineCenter);

            LineLeft = new RectangleShape(new Vector2f(10, Thickness));
            LineLeft.FillColor = Colour;
            LineLeft.Origin = new Vector2f(0, Thickness / 2);
            LineLeft.Position = new Vector2f(11, 43);
            AddChild(LineLeft);

            LineRight = new RectangleShape(new Vector2f(10, Thickness));
            LineRight.FillColor = Colour;
            LineRight.Origin = new Vector2f(0, Thickness / 2);
            LineRight.Position = new Vector2f(-11, 43);
            AddChild(LineRight);

            Fill = new ConvexShape(4);
            Fill.FillColor = FillColour;
            AddChild(Fill);
        }
        public override void OnAdded()
        {
            SetReticlePosition(RETICLE_START_Y);
        }

        public override void Update(float dt)
        {
            Rotation = SourceObject.Rotation - 90 + RotationOffset;

            if (Aiming)
            {
                // Aim Forward/Backward
                if (AimDirection < 0 && Reticle.Y - (SourceObject.AimSpeed * dt) <= RETICLE_START_Y)
                {
                    SetReticlePosition(RETICLE_START_Y);
                    AimDirection = 1;
                }
                else if (AimDirection > 0 && !Utils.InBounds(Game.Bounds, Utils.GetPointInDirection(Position, SourceObject.Rotation + RotationOffset, Reticle.Y + (SourceObject.AimSpeed * dt))))
                {
                    AimDirection = -1;
                }
                else
                    SetReticlePosition(Reticle.Y + ((SourceObject.AimSpeed * AimDirection) * dt));
            }
            else if (Reticle.Y != RETICLE_START_Y)
            {
                // Move back into position after firing
                if (Reticle.Y - ((SourceObject.AimSpeed * 4) * dt) <= RETICLE_START_Y)
                    SetReticlePosition(RETICLE_START_Y);
                else
                    SetReticlePosition(Reticle.Y - ((SourceObject.AimSpeed * 4) * dt));
            }

            /*
            else
            {
                // Always aim at the edge of screen when simply rotating (only works if Recticle is visible)
                Vector2f intersectPoint = Utils.RaycastAgainstBounds(Game.Bounds, SourceObject.Position, Utils.GetPointInDirection(SourceObject.Position, SourceObject.Rotation, Game.Size.X));
                SetReticlePosition(Utils.Distance(SourceObject.Position, intersectPoint) - Circle.Radius);
            }
            */
        }

        public void AimStart()
        {
            Aiming = true;
            SetReticlePosition(RETICLE_START_Y);
            AimDirection = 1;
            Reticle.Visible = true; 
        }
        public void AimEnd()
        {
            Aiming = false;
            Reticle.Visible = false;
        }

        public void SetReticlePosition(float y)
        {
            Reticle.Y = y;

            Vector2f LineLeftP = Utils.GetPointInDirection(LineLeft.Position, (float)Utils.GetAngle(LineLeft.Position, new Vector2f(Circle.Radius, Reticle.Y)), Game.Size.X * 3);
            Vector2f LineRightP = Utils.GetPointInDirection(LineRight.Position, (float)Utils.GetAngle(LineRight.Position, new Vector2f(-Circle.Radius, Reticle.Y)), Game.Size.X * 3);

            LineLeft.Size = new Vector2f(Utils.Distance(LineLeft.Position, LineLeftP), LineLeft.Size.Y);
            LineLeft.Rotation = (float)Utils.GetAngle(LineLeft.Position, LineLeftP);

            LineRight.Size = new Vector2f(Utils.Distance(LineRight.Position, LineRightP), LineRight.Size.Y);
            LineRight.Rotation = (float)Utils.GetAngle(LineRight.Position, LineRightP);

            Fill.SetPoint(0, LineLeft.Position);
            Fill.SetPoint(1, LineLeftP);
            Fill.SetPoint(2, LineRightP);
            Fill.SetPoint(3, LineRight.Position);
        }

        public void UpdateReticle()
        {
            if (Circle != null)
            {
                Reticle.RemoveChild(Circle);
                Reticle.RemoveChild(CircleLine);
            }

            Circle = new CircleShape(SourceObject.Weapon.ExplosionRadius, 40);
            Circle.FillColor = new Color(0, 0, 0, 0);
            Circle.OutlineColor = Colour;
            Circle.OutlineThickness = Thickness;
            Circle.Origin = new Vector2f(Circle.Radius, Circle.Radius);
            Reticle.AddChild(Circle);

            CircleLine = new RectangleShape(new Vector2f(Circle.Radius*2, Thickness));
            CircleLine.FillColor = Colour;
            CircleLine.Origin = new Vector2f(0, Thickness / 2);
            CircleLine.Position = new Vector2f(-Circle.Radius, 0);
            Reticle.AddChild(CircleLine);
        }

        public void UpdateReticlePosition() { SetReticlePosition(Reticle.Y); }
        public float GetReticlePosition() { return Reticle.Y; }

        public bool IsAiming() { return Aiming; }

    }
}
