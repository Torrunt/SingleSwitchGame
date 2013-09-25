using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class Character : PhysicalEntity
    {
        public uint Team;
        protected bool Player;
        public ArtificialIntelligence AI;

        // Movement
        public bool CanMove = true;
        public bool CanMoveVertically = true;
        public bool TurnAroundOnMove = false;

        public bool MoveLeft = false;
        public bool MoveRight = false;
        public bool MoveUp = false;
        public bool MoveDown = false;


        public Character(Game Game, dynamic model = null)
            : base(Game, (object)model)
        {
            Team = 0;
            Player = false;
        }
        public override void Deinit()
        {
            SetAI(null);
            SetPlayer(false);

            base.Deinit();
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (IsDead())
                return;

            if (AI != null)
                AI.Update(dt);

            if (CanMove)
            {
                if (DefaultVelocityApplication)
                {
                    // Movement
                    if (MoveLeft && Velocity.X > -SpeedMax)
                        Velocity.X = Math.Max(Velocity.X - (Acc * dt), -SpeedMax);
                    if (MoveRight && Velocity.X < SpeedMax)
                        Velocity.X = Math.Min(Velocity.X + (Acc * dt), SpeedMax);

                    if (CanMoveVertically)
                    {
                        if (MoveUp && Velocity.Y > -SpeedMax)
                            Velocity.Y = Math.Max(Velocity.Y - (Acc * dt), -SpeedMax);
                        if (MoveDown && Velocity.Y < SpeedMax)
                            Velocity.Y = Math.Min(Velocity.Y + (Acc * dt), SpeedMax);
                    }
                }
                else if (IsMoving())
                {
                    // AI Movement - Stop if next move is going to go past Target
                    if (AI != null && (AI.Target != null || AI.HasWaypoint))
                    {
                        AI.ForcedStop = true;
                        Vector2f nextPos = new Vector2f(X + ((float)Math.Cos(MoveAngle) * (MoveAngleVelocity * dt)), Y + ((float)Math.Sin(MoveAngle) * (MoveAngleVelocity * dt)));
                        if (MoveLeft && nextPos.X < AI.GetTarget().X - AI.Range.X)
                            MoveLeft = false;
                        else if (MoveRight && nextPos.X > AI.GetTarget().X + AI.Range.X)
                            MoveRight = false;
                        else if (MoveUp && nextPos.Y < AI.GetTarget().Y - AI.Range.Y)
                            MoveUp = false;
                        else if (MoveDown && nextPos.Y > AI.GetTarget().Y + AI.Range.Y)
                            MoveDown = false;
                        else
                            AI.ForcedStop = false;
                    }

                    // Movement
                    if (MoveAngleVelocity < SpeedMax && IsMoving())
                        MoveAngleVelocity = Math.Min(MoveAngleVelocity + (Acc *dt), SpeedMax);
                }

                // Flip Horizontally based on movement direction
                if (TurnAroundOnMove)
                {
                    if (Scale.X == 1 && MoveLeft && !MoveRight)
                        SetScale(-1, 1);
                    else if (Scale.X == -1 && MoveRight && !MoveLeft)
                        SetScale(1, 1);
                }
            }
        }


        public override bool IsMoving() { return MoveLeft || MoveRight || MoveUp || MoveDown; }
        public override bool IsMovingHorizontally() { return MoveLeft || MoveRight; }
        public override bool IsMovingVertically() { return MoveUp || MoveDown; }
        public void StopMoving()
        {
            MoveLeft = false;
            MoveRight = false;
            MoveUp = false;
            MoveDown = false;
        }

        public void SetAI(ArtificialIntelligence ai)
        {
            if (ai != null)
            {
                AI = ai;
                AI.Init(this);
            }
            else if (AI != null)
            {
                AI.Deinit();
                AI = null;
            }
        }

        public void SetPlayer(bool val)
        {
            if (Player == val)
                return;
            
            Player = val;

            if (Player)
            {
                Game.Window.KeyReleased += OnKeyReleased;
                Game.Window.KeyPressed += OnKeyPressed;
                Game.NewWindow += OnNewWindow;
            }
            else
            {
                Game.Window.KeyReleased -= OnKeyReleased;
                Game.Window.KeyPressed -= OnKeyPressed;
                Game.NewWindow -= OnNewWindow;
            }
        }
        protected virtual void OnNewWindow(Object sender, EventArgs e)
        {
            if (!Player)
                return;

            Game.Window.KeyReleased += OnKeyReleased;
            Game.Window.KeyPressed += OnKeyPressed;
        }

        protected virtual void OnKeyPressed(Object sender, KeyEventArgs e)
        {
            MoveLeft = e.Code == Keyboard.Key.Left;
            MoveRight = e.Code == Keyboard.Key.Right;
            if (CanMoveVertically)
            {
                MoveUp = e.Code == Keyboard.Key.Up;
                MoveDown = e.Code == Keyboard.Key.Down;
            }
        }

        protected virtual void OnKeyReleased(Object sender, KeyEventArgs e)
        {
            
        }

    }
}
