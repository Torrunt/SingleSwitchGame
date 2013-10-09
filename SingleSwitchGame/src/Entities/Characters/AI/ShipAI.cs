using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;

namespace SingleSwitchGame
{
    class ShipAI : ArtificialIntelligence
    {
        private bool ReachedBeach;

        public ShipAI(Game Game) : base(Game) { }

        public override void Init(Character obj)
        {
            base.Init(obj);

            Range = new Vector2f(10, 10);
        }

        protected override void Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            // Calculate random path to Beach
            if (!ReachedBeach && Waypoint.Equals(new Vector2f(-1, -1)))
            {
                int pointCount = Utils.RandomInt(10, 20);
                float originalAngle = (float)Utils.GetAngle(Game.Island.Position, Obj.Position);
                Vector2f destination = Utils.GetPointInDirection(Game.Island.Position, originalAngle, Game.Island.Radius + 132.5f);
                float originalDistance = Utils.Distance(Obj.Position, destination);

                Vector2f pos;
                for (int i = 0; i < pointCount; i++)
                {
                    pos = Utils.GetPointInDirection(Obj.Position, (float)(Utils.GetAngle(Obj.Position, destination)), (originalDistance / pointCount) * (i + 1));
                    if (i != pointCount - 1 && (i + 1) % 2 == 0)
                        pos += Utils.GetPointInDirection(new Vector2f(), originalAngle + (Utils.RandomInt() == 1 ? 90 : -90), Utils.RandomInt(10, 40));
                    AddWaypointToPath(pos);
                }

                Debug_ShowWaypoints();
            }

            base.Tick(source, e);
        }

        protected override void OnWaypointReached()
        {
            base.OnWaypointReached();

            if (WaypointPath.Count == 0)
            {
                ReachedBeach = true;
            }
        }

        public override void Update(float dt)
        {
            if (Game.Player != null && Game.Player.CurrentPowerup == Cannon.POWERUP_FREEZE_TIME)
                return;

            base.Update(dt);

            if (WaypointPath.Count > 1)
                Obj.Rotation = (float)Utils.GetAngle(Obj.Position, WaypointPath[WaypointPath.Count-1]);
        }

    }
}
