using System;
using System.Timers;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;

namespace SingleSwitchGame
{
    class ArtificialIntelligence
    {
        protected Game Game;
        protected Character Obj;

        public Entity Target;
        public bool HasWaypoint = false;
        public Vector2f Waypoint;
        protected List<Vector2f> WaypointPath;
        protected Vector2f T;

        public bool UpdateMoveAngle = true;

        public Vector2f Range;
        /// <summary>Set to true by Character if next move was going to go past the Target.</summary>
        public bool ForcedStop = false;
        public bool StopAtWaypoints = false;

        private Timer TickTimer;

        public ArtificialIntelligence(Game game)
        {
            Game = game;
        }

        public virtual void Init(Character obj)
        {
            Obj = obj;
            Obj.DefaultVelocityApplication = false;

            Target = null;
            Waypoint = new Vector2f(-1, -1);
            WaypointPath = new List<Vector2f>();

            Range = new Vector2f(100.0f, 100.0f);

            TickTimer = new Timer(500); // Thinks every 500ms
            TickTimer.AutoReset = false;
            TickTimer.Elapsed += Tick;
            TickTimer.Start();
        }
        public void Deinit()
        {
            if (TickTimer != null)
            {
                TickTimer.Stop();
                TickTimer.Dispose();
                TickTimer = null;
            }

            Debug_ShowWaypoints(false);
        }

        public virtual void Update(float dt)
        {
            // Get T (Waypoint or Target)
            if (Target != null)
                T = Target.Position;
            else if (Waypoint.X != -1 && Waypoint.Y != 1)
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
                ForcedStop = false;
                Obj.StopMoving();
            }
        }
        protected virtual void Tick(Object source = null, ElapsedEventArgs e = null)
        {
            // Put stuff that doesn't need to happen very often in here
            // Such as looking for targets, checking if the current target went behind a wall, etc.

            TickTimer.Start();
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
            Target = target;
        }

        public void SetWaypoint(Vector2f point)
        {
            Waypoint = point;
            Target = null;
            HasWaypoint = true;
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
            if (WaypointPath.Count != 0)
            {
                // Next Waypoint in Path?
                if (WaypointPath[0].Equals(Waypoint))
                    WaypointPath.RemoveAt(0);
                if (WaypointPath.Count == 0)
                {
                    Waypoint = new Vector2f(-1, -1);
                    HasWaypoint = false;
                }
                else
                    SetWaypoint(WaypointPath[0]);
            }
            else
            {
                Waypoint = new Vector2f(-1, -1);
                HasWaypoint = false;
            }

            // Debugging Waypoints?
            if (Debug_ShowingWaypoints)
                Debug_ShowWaypointsUpdate();
        }

        // Debugging
        private bool Debug_ShowingWaypoints = false;
        private DisplayObject Debug_Waypoints;
        public void Debug_ShowWaypoints(bool value = true)
        {
            Debug_ShowingWaypoints = value;
            if (Debug_ShowingWaypoints)
            {
                Debug_Waypoints = new DisplayObject();
                Game.Layer_Other.AddChild(Debug_Waypoints);

                Debug_ShowWaypointsUpdate();
            }
            else if (Debug_Waypoints != null)
            {
                if (Debug_Waypoints.Parent != null)
                    Debug_Waypoints.Parent.RemoveChild(Debug_Waypoints);
                Debug_Waypoints = null;
            }
        }
        private void Debug_ShowWaypointsUpdate()
        {
            Debug_Waypoints.Clear();

            Vector2f lastPos = Obj.Position;
            for (int i = 0; i < WaypointPath.Count; i++)
            {
                CircleShape wp = new CircleShape(4);
                wp.Origin = new Vector2f(4, 4);
                wp.FillColor = new Color(255, 0, 0, 200);
                wp.Position = WaypointPath[i];
                Debug_Waypoints.AddChild(wp);

                VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                line[0] = new Vertex(lastPos, new Color(255, 0, 0, 140));
                line[1] = new Vertex(WaypointPath[i], new Color(255, 0, 0, 140));
                Debug_Waypoints.AddChild(line);
                lastPos = WaypointPath[i];
            }
        }

    }
}
