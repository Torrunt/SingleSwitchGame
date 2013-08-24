using System;
using System.Timers;
using System.Collections.Generic;
using SFML.Window;

namespace SingleSwitchGame
{
    class ArtificialIntelligence
    {
        protected Game game;
        protected Character obj;

        public Entity target;
        public Vector2f waypoint;
        protected List<Vector2f> waypointPath;
        protected Vector2f t;

        public Vector2f range;

        private Timer tickTimer;

        public ArtificialIntelligence() { }
        public virtual void init(Game game, Character obj)
        {
            this.game = game;
            this.obj = obj;

            target = null;
            waypoint = new Vector2f(-1, -1);
            waypointPath = new List<Vector2f>();

            range = new Vector2f(100.0f, 100.0f);

            tickTimer = new Timer(500); // Thinks every 500ms
            tickTimer.Elapsed += new ElapsedEventHandler(tick);
            tickTimer.Start();
        }
        public virtual void deint()
        {
            tickTimer.Stop();
            tickTimer.Dispose();
        }

        protected virtual void tick(object source, ElapsedEventArgs e)
        {
            if (target != null)
                t = target.Position;
            else if (!waypoint.Equals(new Vector2f(-1, -1)))
                t = waypoint;
            else
                return;

            // Move Towards Target/Waypoint
            obj.moveRight = (obj.X + range.X < t.X);
            obj.moveLeft = (obj.X - range.X > t.X);
            if (obj.canMoveVertically)
            {
                obj.moveUp = (obj.Y- range.Y > t.Y);
                obj.moveDown = (obj.Y + range.Y < t.Y);
            }

            if (target == null && !obj.isMoving())
            {
                // On Reach Waypoint
                onWayPointReached();
            }
        }

        public void setTarget(Entity target)
        {
            this.target = target;
        }

        public void setWayPoint(Vector2f point)
        {
            waypoint = point;
            target = null;
        }
        public void addWayPointToPath(Vector2f point)
        {
            if (waypointPath.Count == 0)
                setWayPoint(point);
            waypointPath.Add(point);
        }
        public void addWayPointsToPath(params Vector2f[] points)
        {
            for (int i = 0; i < points.Length; i++)
                addWayPointToPath(points[i]);
        }
        protected virtual void onWayPointReached()
        {
            if (waypointPath.Count != 0 && waypointPath[0].Equals(waypoint))
            {
                // Next Waypoint in Path?
                waypointPath.RemoveAt(0);
                if (waypointPath.Count != 0)
                    setWayPoint(waypointPath[0]);
                else
                    waypoint = new Vector2f(-1, -1);
            }
            else
                waypoint = new Vector2f(-1, -1);
        }


    }
}
