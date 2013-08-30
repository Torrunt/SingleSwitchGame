using System;
using System.Timers;
using System.Collections.Generic;
using SFML.Window;

namespace SingleSwitchGame
{
    class ArtificialIntelligence
    {
        protected Game game;
        protected Character Obj;

        public Entity Target;
        public Vector2f Waypoint;
        protected List<Vector2f> WaypointPath;
        protected Vector2f T;

        public Vector2f Range;

        private Timer TickTimer;

        public ArtificialIntelligence() { }
        public virtual void Init(Game game, Character obj)
        {
            this.game = game;
            this.Obj = obj;

            Target = null;
            Waypoint = new Vector2f(-1, -1);
            WaypointPath = new List<Vector2f>();

            Range = new Vector2f(100.0f, 100.0f);

            TickTimer = new Timer(500); // Thinks every 500ms
            TickTimer.Elapsed += new ElapsedEventHandler(Tick);
            TickTimer.Start();
        }
        public virtual void Deinit()
        {
            TickTimer.Stop();
            TickTimer.Dispose();
        }

        protected virtual void Tick(Object source, ElapsedEventArgs e)
        {
            if (Target != null)
                T = Target.Position;
            else if (!Waypoint.Equals(new Vector2f(-1, -1)))
                T = Waypoint;
            else
                return;

            // Move Towards Target/Waypoint
            Obj.MoveRight = (Obj.X + Range.X < T.X);
            Obj.MoveLeft = (Obj.X - Range.X > T.X);
            if (Obj.CanMoveVertically)
            {
                Obj.MoveUp = (Obj.Y - Range.Y > T.Y);
                Obj.MoveDown = (Obj.Y + Range.Y < T.Y);
            }

            if (Target == null && !Obj.IsMoving())
            {
                // On Reach Waypoint
                OnWaypointReached();
            }
        }

        public void SetTarget(Entity target)
        {
            this.Target = target;
        }

        public void SetWaypoint(Vector2f point)
        {
            Waypoint = point;
            Target = null;
        }
        public void AddWaypointToPath(Vector2f point)
        {
            if (WaypointPath.Count == 0)
                SetWaypoint(point);
            WaypointPath.Add(point);
        }
        public void AddWaypointsToPath(params Vector2f[] points)
        {
            for (int i = 0; i < points.Length; i++)
                AddWaypointToPath(points[i]);
        }
        protected virtual void OnWaypointReached()
        {
            if (WaypointPath.Count != 0 && WaypointPath[0].Equals(Waypoint))
            {
                // Next Waypoint in Path?
                WaypointPath.RemoveAt(0);
                if (WaypointPath.Count != 0)
                    SetWaypoint(WaypointPath[0]);
                else
                    Waypoint = new Vector2f(-1, -1);
            }
            else
                Waypoint = new Vector2f(-1, -1);
        }


    }
}
