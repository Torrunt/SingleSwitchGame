using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame.GUI
{
    class AimAssistance : Entity
    {
        public Entity SourceObject;

        public VertexArray LineCenter;
        public VertexArray LineLeft;
        public VertexArray LineRight;

        public AimAssistance(Game Game, Entity SourceObject)
            : base(Game)
        {
            this.SourceObject = SourceObject;

            LineCenter = new VertexArray(PrimitiveType.Lines, 2);
            LineCenter[0] = new Vertex(new Vector2f(0, 48));
            LineCenter[1] = new Vertex(new Vector2f(0, Game.Window.Size.X-48));
            AddChild(LineCenter);

            LineLeft = new VertexArray(PrimitiveType.Lines, 2);
            LineLeft[0] = new Vertex(new Vector2f(11, 43));
            LineLeft[1] = new Vertex(new Vector2f(60, Game.Window.Size.X - 48));
            AddChild(LineLeft);

            LineRight = new VertexArray(PrimitiveType.Lines, 2);
            LineRight[0] = new Vertex(new Vector2f(-11, 43));
            LineRight[1] = new Vertex(new Vector2f(-60, Game.Window.Size.X - 48));
            AddChild(LineRight);
        }

        public override void OnAdded()
        {
            base.OnAdded();

            SetPosition(SourceObject.Position);
            Console.Write(SourceObject.Position + "\n");
        }

        public override void Update(float dt)
        {
            Rotation = SourceObject.Rotation - 90;
        }
    }
}
