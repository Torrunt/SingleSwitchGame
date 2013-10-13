using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class HowToPlay : GraphicalUserInterface
    {
        private AnimatedSprite Tutorial;

        public HowToPlay(Game game)
            : base(game)
        {
            // Background
            Sprite BluePrintBackground = Graphics.GetSprite("assets/sprites/background_blueprint_tile.png");
            BluePrintBackground.Texture.Repeated = true;
            BluePrintBackground.TextureRect = new IntRect(0, 0, (int)Game.Size.X, (int)Game.Size.Y);
            AddChild(BluePrintBackground);
            
            // help
            Text help = new Text("(tap to continue)", Game.TidyHand, 30);
            FloatRect textRect = help.GetLocalBounds();
            help.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);
            help.Position = new Vector2f(Game.Size.X / 2, Game.Size.Y - 25);
            AddChild(help);


            Tutorial = Graphics.GetAnimatedSprite(Game, Graphics.ASSETS_SPRITES + "gui/HowToPlay.xml");
            Tutorial.Sprite.Texture.Smooth = false;
            Tutorial.Stop();
            Tutorial.SetFrame(1);
            AddChild(Tutorial);
        }

        public override void Init()
        {
            Game.Window.KeyPressed += OnKeyPressed;
            Game.Window.MouseButtonPressed += OnMouseButtonPressed;
            Game.NewWindow += OnNewWindow;
        }
        public override void Deinit()
        {
            Game.NewWindow -= OnNewWindow;
            Game.Window.KeyPressed -= OnKeyPressed;
            Game.Window.MouseButtonPressed -= OnMouseButtonPressed;

            base.Deinit();
        }
        private void OnNewWindow(Object sender, EventArgs e)
        {
            Game.Window.KeyPressed += OnKeyPressed;
            Game.Window.MouseButtonPressed += OnMouseButtonPressed;
        }

        private void OnKeyPressed(Object sender, KeyEventArgs e = null)
        {
            if (e != null && Game.KeyIsNotAllowed(e.Code))
                return;

            if (Tutorial == null || Tutorial.Finished)
            {
                if (Parent != null)
                    Parent.RemoveChild(this);
            }
            else
                Tutorial.NextFrame();
        }
        protected virtual void OnMouseButtonPressed(Object sender, MouseButtonEventArgs e = null)
        {
            OnKeyPressed(sender);
        }

    }
}
