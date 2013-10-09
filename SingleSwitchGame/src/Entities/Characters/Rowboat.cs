using SFML.Window;
using SFML.Graphics;
using System;

namespace SingleSwitchGame
{
    class Rowboat : Character
    {
        public Rowboat(Game game)
            : base(game, null)
        {
            //if (Game.GraphicsMode == Game.GRAPHICSMODE_NORMAL)
            Model = Graphics.GetSprite("assets/sprites/Rowboat.png");
            AddChild(Model);
        }
    }
}