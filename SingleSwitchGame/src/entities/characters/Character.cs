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
        public bool TurnAroundOnMove = true;

        public bool MoveLeft = false;
        public bool MoveRight = false;
        public bool MoveUp = false;
        public bool MoveDown = false;


        public Character(Game game, Sprite model)
            : base(game, model)
        {
            Team = 0;
            Player = false;
        }
        public override void Deinit()
        {
            base.Deinit();

            SetAI(null);
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (IsDead())
                return;

            if (Player)
                PlayerControls();

            if (CanMove)
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

        public void SetAI(ArtificialIntelligence AI)
        {
            if (AI != null)
            {
                this.AI = AI;
                this.AI.Init(game, this);
            }
            else if (this.AI != null)
                this.AI.Deinit();
        }
        public void SetPlayer(bool val)
        {
            this.Player = val;
        }

        protected virtual void PlayerControls()
        {
            MoveLeft = Keyboard.IsKeyPressed(Keyboard.Key.Left);
            MoveRight = Keyboard.IsKeyPressed(Keyboard.Key.Right);
            if (CanMoveVertically)
            {
                MoveUp = Keyboard.IsKeyPressed(Keyboard.Key.Up);
                MoveDown = Keyboard.IsKeyPressed(Keyboard.Key.Down);
            }
        }

    }
}
