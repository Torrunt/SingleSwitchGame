using System;
using SFML.Window;
using SFML.Graphics;
using System.Timers;

namespace SingleSwitchGame
{
    class MessageFade : Entity
    {
        private Text Text;
        private int Alpha;

        private int FadeSpeed = 10;
        private bool FadingOut;
        private bool Showing;
        private Timer ShowTimer;

        public MessageFade(Game game, string msg, uint fontSize, Vector2f position, double showTime = 1500, bool center = true) : base(game)
        {
            Text = new Text(msg, Game.TidyHand, fontSize);
            Text.Color = new Color(255, 255, 255, 0);
            FloatRect textRect = Text.GetLocalBounds();
            if (center)
                Text.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);

            AddChild(Text);
            Position = position;

            ShowTimer = new Timer(showTime);
            ShowTimer.AutoReset = false;
            ShowTimer.Elapsed += ShowTimerHandler;
        }
        public override void Deinit()
        {
            base.Deinit();

            if (ShowTimer == null)
                return;
            ShowTimer.Stop();
            ShowTimer.Elapsed -= ShowTimerHandler;
            ShowTimer = null;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (Showing)
                return;

            if (FadingOut)
            {
                Alpha = Alpha - FadeSpeed < 0 ? 0 : Alpha - FadeSpeed;
                if (Alpha == 0)
                    Parent.RemoveChild(this);
            }
            else
            {
                Alpha = Alpha + FadeSpeed > 255 ? 255 : Alpha + FadeSpeed;
                if (Alpha == 255)
                {
                    ShowTimer.Start();
                    Showing = true;
                }
            }

            Text.Color = new Color(255, 255, 255, (byte)Alpha);
        }

        private void ShowTimerHandler(Object source, ElapsedEventArgs e)
        {
            Showing = false;
            FadingOut = true;
        }
    }
}
