using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;

namespace SingleSwitchGame
{
    class InfantrymanAI : ArtificialIntelligence
    {
        public override void Init(Game Game, Character obj)
        {
            base.Init(Game, obj);

            Range = new Vector2f(0, 0);
        }

        protected override void Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            if (Waypoint.Equals(new Vector2f(-1, -1)))
            {
                
                int pointCount = Utils.RandomInt(5, 12);
                float originalDistance = Utils.Distance(Obj.Position, Game.Hill.Position);
                Vector2f pos = Obj.Position;
                for (int i = 0; i < pointCount; i++)
                {
                    pos = Utils.GetPointInDirection(pos + new Vector2f(Utils.RandomInt(0,100)-50,Utils.RandomInt(0,100)-50), (float)(Utils.GetAngle(pos, Game.Hill.Position)), originalDistance / pointCount);
                    AddWaypointToPath(pos);
                }
                AddWaypointToPath(Game.Hill.Position);
            }

            if (Utils.CircleCircleCollision(Obj.Position, Obj.Model.Radius, Game.Hill.Position, Game.Hill.Radius))
            {
                // Reached Hill
                Obj.Parent.RemoveChild(Obj);
                return;
            }

            base.Tick(source, e);
        }
    }
}
