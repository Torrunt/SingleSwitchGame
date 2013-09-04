using System;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class HeadsUpDisplay : Entity
    {

        public bool DisplayFPS = true;
        private Text FPS;

        public HeadsUpDisplay(Game Game)
            : base(Game, null)
        {
            if (DisplayFPS)
            {
                FPS = new Text("fps", Game.TidyHand, 16);
                FPS.Position = new Vector2f(Game.Window.Size.X - 35, Game.Window.Size.Y - 20);
                Game.Layer_GUI.AddChild(FPS);
            }
        }

        public override void Update(float dt)
        {
            if (DisplayFPS)
                FPS.DisplayedString = (1 / dt).ToString("00.0");
        }
    }
}
