using System;
using SFML.Window;
using SingleSwitchGame.GUI;
using System.Timers;

namespace SingleSwitchGame
{
    class Cannon : Character
    {
        public float AimSpeed = 300;
        public float RotateSpeed = 35;
        private bool CanRotate = true;
        private Timer RotationDelayTimer;

        private bool KeyDown = false;
        private bool KeyWasDown = false;
        
        public CannonWeapon Weapon;
        public AimAssistance AimAssistance;

        public Cannon(Game Game)
            : base(Game, Graphics.GetSprite("assets/sprites/cannon.png"))
        {
            SetScale(0.5f, 0.5f);
            Origin = new Vector2f(26, 30);
            CanMove = false;
        }
        public override void Init()
        {
            base.Init();

            RotationDelayTimer = new Timer(300); // Delay in ms before the Cannon starts rotating after firing
            RotationDelayTimer.Elapsed += new ElapsedEventHandler(OnRotationDelayEnd);

            Weapon = new CannonWeapon(Game, this);
        }
        public override void Deinit()
        {
            base.Deinit();

            if (RotationDelayTimer != null)
            {
                RotationDelayTimer.Stop();
                RotationDelayTimer.Dispose();
                RotationDelayTimer = null;
            }

            Weapon.Deinit();
            Weapon = null;
        }
        public override void OnAdded()
        {
            base.OnAdded();

            AimAssistance = new AimAssistance(Game, this);
            Game.Layer_GUI.AddChild(AimAssistance);
        }
        public override void OnRemoved()
        {
            base.OnRemoved();

            Game.Layer_GUI.RemoveChild(AimAssistance);
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (CanRotate)
                Rotate(RotateSpeed * dt);
        }

        private void OnButtonDown()
        {
            CanRotate = false;
            RotationDelayTimer.Stop();

            AimAssistance.AimStart();
        }
        private void OnButtonReleased()
        {
            Weapon.Fire(Utils.GetPointInDirection(Position, Rotation, 40), Rotation, Utils.GetPointInDirection(Position, Rotation, AimAssistance.Reticle.Y));
            StartRotationDelay();

            AimAssistance.AimEnd();
        }


        protected override void PlayerControls()
        {
            KeyWasDown = KeyDown;
            KeyDown = false;
            for (Keyboard.Key i = Keyboard.Key.A; i <= Keyboard.Key.KeyCount; i++)
            {
                if (i != Keyboard.Key.Escape && i != Keyboard.Key.F11 && Keyboard.IsKeyPressed(i))
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

        public void StartRotationDelay()
        {
            CanRotate = false;
            RotationDelayTimer.Start();
        }
        protected virtual void OnRotationDelayEnd(Object source, ElapsedEventArgs e)
        {
            RotationDelayTimer.Stop();
            CanRotate = true;
        }

    }
}
