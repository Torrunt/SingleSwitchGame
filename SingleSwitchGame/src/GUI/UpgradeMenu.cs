using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class UpgradeMenu : SimpleSingleSwitchMenu
    {

        /// <summary>The max level an upgrade can be leveled.</summary>
        public readonly List<uint> UpgradeLevelCaps = new List<uint>() { 45, 20, 30, 1000 }; 

        public UpgradeMenu(Game game)
            : base(game) { }
        protected override void Setup()
        {
            Title = "Upgrade Menu";

            ButtonNames = new List<string>()
            {
                "Health",
                "Explosion Blast Radius",
                "Landmines",
                "Base Score Multiplier"
            };
            ButtonDescriptions = new List<string>()
            {
                "Increases your health by 1.",
                "Increases your cannon ball's explosion blast radius by 5%.",
                "Increases chance of infantry blowing up when on the beach by 1%.",
                "Increases your base score multiplier by 2."
            };

            base.Setup();
        }
        protected override void SetupButtonAdditional(int i, DisplayObject button, RectangleShape button_back, bool pickable)
        {
            // current level
            Text currentLevel = new Text("", Game.TidyHand, 45);
            if (pickable)
                currentLevel.DisplayedString = Game.Player.GetUpgradeValue(i, Game.Player.UpgradeLevels[i]) + " -> " + Game.Player.GetUpgradeValue(i, Game.Player.UpgradeLevels[i] + 1);
            else
                currentLevel.DisplayedString = Game.Player.GetUpgradeValue(i, Game.Player.UpgradeLevels[i]) + " (MAX)";
            FloatRect currentLevelRect = currentLevel.GetLocalBounds();
            currentLevel.Origin = new Vector2f(currentLevelRect.Width / 2.0f, 0);
            currentLevel.Position = new Vector2f(button_back.Size.X / 2, 120);
            currentLevel.Color = new Color(255, 255, 255, (byte)(pickable ? 255 : 120));
            button.AddChild(currentLevel);
        }
        public override void OnRemoved()
        {
            base.OnRemoved();
            Game.UpgradeMenu = null;
        }

        protected override void ActivateButton()
        {
            base.ActivateButton();

            Game.Player.LevelUpUpgrade(CurrentSelection);

            if (Parent != null)
                Parent.RemoveChild(this);
        }

        protected override bool IsPickable(int i)
        {
            return UpgradeLevelCaps[i] == 0 || Game.Player.UpgradeLevels[i] < UpgradeLevelCaps[i];
        }

    }
}
