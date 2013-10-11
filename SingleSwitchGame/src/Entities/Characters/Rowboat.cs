using SFML.Window;
using SFML.Graphics;
using System;

namespace SingleSwitchGame
{
    class Rowboat : Character
    {
        public Rowboat(Game game)
            : base(game, game.GraphicsMode == Game.GRAPHICSMODE_NORMAL ? Graphics.GetSprite("assets/sprites/rowboat_full.png") : Graphics.GetSprite("assets/sprites/blueprint/rowboat_full.png"))
        {
        }
    }
}