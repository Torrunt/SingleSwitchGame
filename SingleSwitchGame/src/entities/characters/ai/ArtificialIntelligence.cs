using System;
using System.Timers;
using System.Collections.Generic;
using SFML.Window;

namespace SingleSwitchGame
{
    class ArtificialIntelligence
    {
        protected Game Game;
        protected Character Obj;

        public Entity Target;
        public bool HasWayPoint = false;
        public Vector2f Waypoint;
        protected List<Vector2f> WaypointPath;
        protected Vector2f T;

        public bool UpdateMoveAngle = true;

        public Vector2f Range;
        /// <summary>Set to true by Character if next move was going to go past the Target.</summary>
        public bool ForcedStop = false;

        private Timer TickTimer;

        public ArtificialIntelligence() { }
        public virtual void Init(Game Game, Character obj)
        {
            this.Game = Game;
            this.Obj = obj;

            this.Obj.DefaultVelocityApplication = false;

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
            if (!Obj.DefaultVelocityApplication && UpdateMoveAngle)
                Obj.MoveAngle = (float)Utils.GetAngle(Obj.Position, T, false);

            Obj.MoveRight = (Obj.X + Range.X < T.X);
            Obj.MoveLeft = (Obj.X - Range.X > T.X);
            if (Obj.CanMoveVertically)
            {
                Obj.MoveUp = (Obj.Y - Range.Y > T.Y);
                Obj.MoveDown = (Obj.Y + Range.Y < T.Y);
            }
            
            if (Target == null && (ForcedStop || !Obj.IsMoving()))
            {
                // On Reach Waypoint
                OnWaypointReached();
            }
        }

        public dynamic GetTarget()
        {
            if (Target != null)
                return Target.Position;
            else if (!Waypoint.Equals(new Vector2f(-1, -1)))
                return Waypoint;

            return null;
        }

        public void SetTarget(Entity target)
        {
            this.Target = target;
        }

        public void SetWaypoint(Vector2f point)
        {
            Waypoint = point;
            Target = null;
            HasWayPoint = true;
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
                {
                    Waypoint = new Vector2f(-1, -1);
                    HasWayPoint = false;
                }
            }
            else
            {
                Waypoint = new Vector2f(-1, -1);
                HasWayPoint = false;
            }
        }


    }
}
