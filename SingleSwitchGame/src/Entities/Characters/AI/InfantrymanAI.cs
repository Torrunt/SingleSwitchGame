using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;

namespace SingleSwitchGame
{
    class InfantrymanAI : ArtificialIntelligence
    {
        private bool AlreadyAvoidedLandmine;
        private int PossibleLandmineAtWaypoint;
        private int PossibleLandmineCount;

        public InfantrymanAI(Game Game) : base(Game) { }

        public override void Init(Character obj)
        {
            base.Init(obj);

            Range = new Vector2f(0, 0);
            StopAtWaypoints = true;
        }

        protected override void Tick(object source = null, System.Timers.ElapsedEventArgs e = null)
        {
            // Calculate random path to Hill
            if (Waypoint.Equals(new Vector2f(-1, -1)))
            {
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

                PossibleLandmineAtWaypoint = Utils.RandomInt(0, WaypointPath.Count-1);

                //Debug_ShowWaypoints();
            }

            base.Tick(source, e);
        }

        protected override void OnWaypointReached()
        {
            base.OnWaypointReached();

            // Chance to step on landmine
            PossibleLandmineCount++;
            if (AlreadyAvoidedLandmine || PossibleLandmineCount < PossibleLandmineAtWaypoint)
                return;

            if (Game.Player != null && Game.Player.LandmineExplosionChance != 0 && Utils.RandomInt(1, 100) <= Game.Player.LandmineExplosionChance)
            {
                Explosion explosion = new Explosion(Game, 8);
                explosion.Position = Obj.Position;
                Game.Layer_Other.AddChild(explosion);
                Obj.Parent.RemoveChild(Obj);
            }
            AlreadyAvoidedLandmine = true;
        }

        public override void Update(float dt)
        {
            if (Game.Player != null && Game.Player.CurrentPowerup == Cannon.POWERUP_FREEZE_TIME)
                return;

            if (Utils.CircleCircleCollision(Obj.Position, Obj.Model.Radius, Game.Hill.Position, Game.Hill.Radius))
            {
                // Reached Hill
                if (Game.Player != null)
                    Game.Player.Damage(1);
                Obj.Damage(Obj.Health);
                return;
            }

            base.Update(dt);
        }

    }
}
