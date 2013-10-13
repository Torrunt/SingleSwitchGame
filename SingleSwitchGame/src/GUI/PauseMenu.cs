using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class PauseMenu : SimpleSingleSwitchMenu
    {

        /// <summary>The max level an upgrade can be leveled.</summary>
        public readonly List<uint> UpgradeLevelCaps = new List<uint>() { 45, 20, 30, 1000 };

        public PauseMenu(Game game)
            : base(game) { }
        protected override void Setup()
        {
            Title = "Paused";
            ButtonNames = new List<string>()
            {
                "Resume",
                "Quit to Start Menu",
            };
            StripWidth = Game.Size.X;
            StartY = 610;
            ButtonDescriptions = new List<string>() { };
            ButtonHeight = 75;
            ButtonGap = 150;
            HelpY = 220;

            base.Setup();
        }
        public override void OnRemoved()
        {
            base.OnRemoved();
            Game.PauseMenu = null;
        }

        protected override void ActivateButton()
        {
            base.ActivateButton();

            switch (CurrentSelection)
            {
                case 0:
                    {
                        // Resume
                        if (Parent != null)
                            Parent.RemoveChild(this);
                    }
                    break;
                case 1:
                    {
                        // Quit to Start Menu
                        Game.Stop();
                        Game.StartMenu = new StartMenu(Game);
                        Game.Layer_GUI.AddChild(Game.StartMenu);
                    }
                    break;
            }
        }

    }
}
