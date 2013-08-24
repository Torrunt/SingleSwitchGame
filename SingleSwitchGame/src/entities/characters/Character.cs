using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class Character : PhysicalEntity
    {
        public uint team;
        protected bool player;
        public ArtificialIntelligence ai;

        // Movement
        public bool canMove = true;
        public bool canMoveVertically = true;
        public bool turnAroundOnMove = true;

        public bool moveLeft = false;
        public bool moveRight = false;
        public bool moveUp = false;
        public bool moveDown = false;


        public Character(Game game, Sprite model)
            : base(game, model)
        {
            team = 0;
            player = false;
        }
        public override void deinit()
        {
            base.deinit();

            setAI(null);
        }

        public override void update(float dt)
        {
            base.update(dt);

            if (isDead())
                return;

            if (player)
                playerControls();

            if (canMove)
            {
                // Movement
                if (moveLeft && velocity.X > -speedMax)
                    velocity.X = Math.Max(velocity.X - (acc * dt), -speedMax);
                if (moveRight && velocity.X < speedMax)
                    velocity.X = Math.Min(velocity.X + (acc * dt), speedMax);
                
                if (canMoveVertically)
                {
                    if (moveUp && velocity.Y > -speedMax)
                        velocity.Y = Math.Max(velocity.Y - (acc * dt), -speedMax);
                    if (moveDown && velocity.Y < speedMax)
                        velocity.Y = Math.Min(velocity.Y + (acc * dt), speedMax);
                }

                // Flip Horizontally based on movement direction
                if (turnAroundOnMove)
                {
                    if (Scale.X == 1 && moveLeft && !moveRight)
                        setScale(-1, 1);
                    else if (Scale.X == -1 && moveRight && !moveLeft)
                        setScale(1, 1);
                }
            }
        }


        public override bool isMoving() { return moveLeft || moveRight || moveUp || moveDown; }
        public override bool isMovingHorizontally() { return moveLeft || moveRight; }
        public override bool isMovingVertically() { return moveUp || moveDown; }

        public void setAI(ArtificialIntelligence ai)
        {
            if (ai != null)
            {
                this.ai = ai;
                this.ai.init(game, this);
            }
            else if (this.ai != null)
                this.ai.deint();
        }
        public void setPlayer(bool val)
        {
            player = val;
        }

        protected void playerControls()
        {
            moveLeft = Keyboard.IsKeyPressed(Keyboard.Key.Left);
            moveRight = Keyboard.IsKeyPressed(Keyboard.Key.Right);
            if (canMoveVertically)
            {
                moveUp = Keyboard.IsKeyPressed(Keyboard.Key.Up);
                moveDown = Keyboard.IsKeyPressed(Keyboard.Key.Down);
            }
        }

    }
}
