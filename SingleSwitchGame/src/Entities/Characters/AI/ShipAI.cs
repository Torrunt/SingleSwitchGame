using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace SingleSwitchGame
{
    class ShipAI : ArtificialIntelligence
    {
        public bool ReachedBeach;
        public bool LeavingArea;

        public ShipAI(Game Game) : base(Game) { }

        public override void Init(Character obj)
        {
            base.Init(obj);

            Range = new Vector2f(10, 10);
        }

        protected override void Tick(object source = null, System.Timers.ElapsedEventArgs e = null)
        {
            if (!Waypoint.Equals(new Vector2f(-1, -1)))
            {
                base.Tick(source, e);
                return;
            }
            
            if (!ReachedBeach)
            {
                // Generate path to Beach
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

                //Debug_ShowWaypoints();
            }
            else if (((Ship)Obj).AmountOfInfantry <= 0)
            {
                // Generate path off screen
                LeavingArea = true;
                Obj.CanMove = false; // Can't move until Ship rotates to first waypoint in path
                if (Obj.Model is AnimatedSprite)
                {
                    Obj.Model.Sprite.Color = new Color(255, 255, 255, 100);
                    Obj.Model.Play();
                }
                else
                    Obj.Model.Color = new Color(255, 255, 255, 100);

                int pointCount = Utils.RandomInt(4, 15);
                float originalAngle = (float)Utils.GetAngle(Game.Island.Position, Obj.Position) + (Utils.RandomInt(50, 80) * (Utils.RandomInt() == 1 ? 1 : -1));
                Vector2f destination = Utils.GetPointInDirection(Game.Island.Position, originalAngle,(Game.Size.X / 2) + 300);
                float originalDistance = Utils.Distance(Obj.Position, destination);

                Vector2f pos;
                for (int i = 0; i < pointCount; i++)
                {
                    pos = Utils.GetPointInDirection(Obj.Position, (float)(Utils.GetAngle(Obj.Position, destination)), (originalDistance / pointCount) * (i + 1));
                    if (i != pointCount - 1 && (i + 1) % 2 == 0)
                        pos += Utils.GetPointInDirection(new Vector2f(), originalAngle + (Utils.RandomInt() == 1 ? 90 : -90), Utils.RandomInt(10, 40));
                    AddWaypointToPath(pos);
                }
            }

            base.Tick(source, e);
        }

        protected override void OnWaypointReached()
        {
            base.OnWaypointReached();

            if (WaypointPath.Count != 0)
                return;

            if (!ReachedBeach)
            {
                // Reached Beach
                ReachedBeach = true;
                Game.AIManager.OnShipReachedBeach((Ship)Obj);

                if (Obj.Model is AnimatedSprite)
                    Obj.Model.GotoAndStop(1);
            }
            else if (LeavingArea)
            {
                // Left Area
                Game.AIManager.EnemyRemoved(Obj, false);
                if (Obj.Parent != null)
                    Obj.Parent.RemoveChild(Obj);
            }
        }

        public override void Update(float dt)
        {
            if (Game.Player != null && Game.Player.HasPowerup(Powerup.FREEZE_TIME))
                return;

            base.Update(dt);

            if (LeavingArea && WaypointPath.Count > 0)
            {
                if (!Obj.CanMove)
                {
                    // Rotate to first waypoint in path
                    float targetAngle = (float)Utils.GetAngle(Obj.Position, WaypointPath[0]);
                    if (Obj.Rotation != targetAngle)
                        Obj.Rotation = Utils.StepTo(Obj.Rotation, targetAngle, 1);
                    else
                        Obj.CanMove = true;
                }
                else
                    Obj.Rotation = (float)Utils.GetAngle(Obj.Position, WaypointPath[WaypointPath.Count - 1]);
            }
            else if ((!ReachedBeach && WaypointPath.Count > 1))
                Obj.Rotation = (float)Utils.GetAngle(Obj.Position, WaypointPath[WaypointPath.Count-1]);

        }

    }
}
