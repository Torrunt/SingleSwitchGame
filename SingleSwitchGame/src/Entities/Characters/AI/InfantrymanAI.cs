using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;

namespace SingleSwitchGame
{
    class InfantrymanAI : ArtificialIntelligence
    {
        public InfantrymanAI(Game Game) : base(Game) { }

        public override void Init(Character obj)
        {
            base.Init(obj);

            Range = new Vector2f(0, 0);
        }

        protected override void Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            if (Waypoint.Equals(new Vector2f(-1, -1)))
            {
                // Calculate random path to Hill
                int pointCount = Utils.RandomInt(10, 20);
                float originalAngle = (float)Utils.GetAngle(Game.Hill.Position, Obj.Position);
                Vector2f destination = Utils.GetPointInDirection(Game.Hill.Position, originalAngle, Game.Hill.Radius - 2);
                float originalDistance = Utils.Distance(Obj.Position, destination);

                Vector2f pos;
                for (int i = 0; i < pointCount; i++)
                {
                    pos = Utils.GetPointInDirection(Obj.Position, (float)(Utils.GetAngle(Obj.Position, destination)), (originalDistance / pointCount) * (i + 1));
                    if (i != pointCount - 1 && (i + 1) % 2 == 0)
                        pos += Utils.GetPointInDirection(new Vector2f(), originalAngle + (Utils.RandomInt() == 1 ? 90 : -90), Utils.RandomInt(10, 60));
                    AddWaypointToPath(pos);
                }

                //Debug_ShowWaypoints();
            }

            base.Tick(source, e);
        }

        public override void Update(float dt)
        {
            if (Utils.CircleCircleCollision(Obj.Position, Obj.Model.Radius, Game.Hill.Position, Game.Hill.Radius))
            {
                // Reached Hill
                if (Game.Player != null)
                    Game.Player.Damage(1);
                Obj.Parent.RemoveChild(Obj);
                return;
            }

            base.Update(dt);
        }

    }
}
