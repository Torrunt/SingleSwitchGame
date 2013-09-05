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
        public VertexArray CircleLine;

        public VertexArray LineCenter;
        public VertexArray LineLeft;
        public VertexArray LineRight;

        private bool Aiming = false;
        private int AimDirection = 1;

        public AimAssistance(Game Game, Cannon SourceObject)
            : base(Game)
        {
            this.SourceObject = SourceObject;

            // Reticle
            Reticle = new DisplayObject();
            Reticle.Visible = false;
            AddChild(Reticle);

            Circle = new CircleShape(SourceObject.Weapon.ExplosionRadius, 40);
            Circle.FillColor = new Color(0, 0, 0, 0);
            Circle.OutlineThickness = 1;
            Circle.Origin = new Vector2f(Circle.Radius, Circle.Radius);
            Reticle.AddChild(Circle);

            CircleLine = new VertexArray(PrimitiveType.Lines, 2);
            CircleLine[0] = new Vertex(new Vector2f(Circle.Radius, 0));
            CircleLine[1] = new Vertex(new Vector2f(-Circle.Radius, 0));
            Reticle.AddChild(CircleLine);

            // Aiming Lines
            LineCenter = new VertexArray(PrimitiveType.Lines, 2);
            LineCenter[0] = new Vertex(new Vector2f(0, 48));
            LineCenter[1] = new Vertex(new Vector2f(0, Game.Size.X - 48));
            AddChild(LineCenter);

            LineLeft = new VertexArray(PrimitiveType.Lines, 2);
            LineLeft[0] = new Vertex(new Vector2f(11, 43));
            AddChild(LineLeft);

            LineRight = new VertexArray(PrimitiveType.Lines, 2);
            LineRight[0] = new Vertex(new Vector2f(-11, 43));
            AddChild(LineRight);

            SetReticlePosition(RETICLE_START_Y);
        }

        public override void OnAdded()
        {
            base.OnAdded();

            SetPosition(SourceObject.Position);
        }

        public override void Update(float dt)
        {
            Rotation = SourceObject.Rotation - 90;

            if (Aiming)
            {
                // Aim Forward/Backward
                if (AimDirection < 0 && Reticle.Y - SourceObject.AimSpeed <= RETICLE_START_Y)
                {
                    SetReticlePosition(RETICLE_START_Y);
                    AimDirection = 1;
                }
                else if (AimDirection > 0 && !Utils.InBounds(Game.Bounds, Utils.GetPointInDirection(Position, SourceObject.Rotation, Reticle.Y + SourceObject.AimSpeed)))
                {
                    AimDirection = -1;
                }
                else
                    SetReticlePosition(Reticle.Y + (SourceObject.AimSpeed * AimDirection));
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
            Reticle.Visible = true; 
        }
        public void AimEnd()
        {
            Aiming = false;
            Reticle.Visible = false;
            SetReticlePosition(RETICLE_START_Y);
        }

        public void SetReticlePosition(float y)
        {
            Reticle.Y = y;

            //LineLeft[1] = new Vertex(new Vector2f(Circle.Radius, Reticle.Y));
            //LineRight[1] = new Vertex(new Vector2f(-Circle.Radius, Reticle.Y));

            // (Version for going past the Reticle)
            LineLeft[1] = new Vertex(Utils.GetPointInDirection(LineLeft[0].Position, (float)Utils.GetAngle(LineLeft[0].Position, new Vector2f(Circle.Radius, Reticle.Y)), Game.Size.X - 43));
            LineRight[1] = new Vertex(Utils.GetPointInDirection(LineRight[0].Position, (float)Utils.GetAngle(LineRight[0].Position, new Vector2f(-Circle.Radius, Reticle.Y)), Game.Size.X - 43));
        }
    }
}
