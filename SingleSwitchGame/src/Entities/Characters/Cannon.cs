using System;
using SFML.Window;

namespace SingleSwitchGame
{
    class Cannon : Character
    {
        public float RotateSpeed = 60;

        private bool KeyDown = false;
        private bool KeyWasDown = false;

        public Cannon(Game game)
            : base(game, Graphics.GetSprite("assets/sprites/cannon.png"))
        {
            Origin = new Vector2f(13, 15);
            CanMove = false;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            Rotate(RotateSpeed * dt);
        }

        private void OnButtonDown()
        {
            Console.Write("Button Down\n");
        }
        private void OnButtonReleased()
        {
            Console.Write("Button Released\n");
        }


        protected override void PlayerControls()
        {
            KeyWasDown = KeyDown;
            KeyDown = false;
            for (Keyboard.Key i = Keyboard.Key.A; i <= Keyboard.Key.KeyCount; i++)
            {
                if (i != Keyboard.Key.Escape && Keyboard.IsKeyPressed(i))
                {
                    KeyDown = true;
                    break;
                }
            }

            if (KeyDown && !KeyWasDown)
                OnButtonDown();
            else if (KeyWasDown && !KeyDown)
                OnButtonReleased();
        }

    }
}
