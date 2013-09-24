using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using System.Timers;

namespace SingleSwitchGame
{
    class UpgradeMenuGUI : GraphicalUserInterface
    {
        private readonly List<string> UpgradeNames = new List<string>()
        {
            "Health",
            "Explosion Blast Radius",
            "Landmines",
            "Base Score Multiplier"
        };
        private readonly List<string> UpgradeDescriptions = new List<string>()
        {
            "Increases your health by 1.",
            "Increases your cannon ball's explosion blast radius by 5%.",
            "Increases chance of infantry blowing up when on the beach by 1%.",
            "Increases your base score multiplier by 2."
        };
        /// <summary>The max level an upgrade can be leveled.</summary>
        public readonly List<uint> UpgradeLevelCaps = new List<uint>() { 45, 20, 30, 1000 }; 

        private int CurrentSelection = 0;
        private List<DisplayObject> UpgradeButtons = new List<DisplayObject>();

        private Timer HoldButtonTimer;

        public UpgradeMenuGUI(Game game)
            : base(game)
        {
            // dim game
            RectangleShape dim = new RectangleShape(new Vector2f(Game.Size.X, Game.Size.Y));
            dim.FillColor = new Color(0, 0, 0, 100);
            AddChild(dim);

            // title
            Text title = new Text("Upgrade Shop", Game.TidyHand, 100);
            FloatRect textRect = title.GetLocalBounds();
            title.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);
            title.Position = new Vector2f(Game.Size.X / 2, 100);

            // back
            RectangleShape back = new RectangleShape(new Vector2f(textRect.Width + 300, Game.Size.Y));
            back.FillColor = new Color(0, 0, 0, 100);
            back.Position = new Vector2f(title.Position.X - (back.Size.X/2), 0);
            AddChild(back);

            AddChild(title);

            // help
            Text help = new Text("(tap to change selection, hold down to pick)", Game.TidyHand, 24);
            textRect = help.GetLocalBounds();
            help.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);
            help.Position = new Vector2f(Game.Size.X / 2, 210);
            AddChild(help);

            // upgrades
            for (int i = 0; i < UpgradeNames.Count; i++)
            {
                bool pickable = UpgradeLevelCaps[i] == 0 || Game.Player.UpgradeLevels[i] < UpgradeLevelCaps[i];

                DisplayObject upgrade = new DisplayObject();
                upgrade.Position = new Vector2f(back.Position.X, title.Position.Y + 200 + (280 * i));

                // back
                RectangleShape upgrade_back = new RectangleShape(new Vector2f(back.Size.X, 200));
                upgrade_back.FillColor = new Color(0, 0, 0, (byte)(pickable ? 100 : 50));
                upgrade.AddChild(upgrade_back);

                // name
                Text upgrade_name = new Text(UpgradeNames[i], Game.TidyHand, 50);
                FloatRect upgrade_nameRect = upgrade_name.GetLocalBounds();
                upgrade_name.Origin = new Vector2f(upgrade_nameRect.Left + upgrade_nameRect.Width / 2.0f, 0);
                upgrade_name.Position = new Vector2f(upgrade_back.Size.X/2, 0);
                if (!pickable)
                    upgrade_name.Color = new Color(255, 255, 255, 50);
                upgrade.AddChild(upgrade_name);

                // description
                Text upgrade_description = new Text(UpgradeDescriptions[i], Game.TidyHand, 30);
                FloatRect upgrade_descriptionRect = upgrade_description.GetLocalBounds();
                upgrade_description.Origin = new Vector2f(upgrade_descriptionRect.Width / 2.0f, 0);
                upgrade_description.Position = new Vector2f(upgrade_back.Size.X / 2, 60);
                upgrade_description.Color = new Color(180, 180, 180, (byte)(pickable ? 255 : 120));
                upgrade.AddChild(upgrade_description);

                // current level
                Text upgrade_current = new Text("", Game.TidyHand, 45);
                if (pickable)
                    upgrade_current.DisplayedString = Game.Player.GetUpgradeValue(i, Game.Player.UpgradeLevels[i]) + " -> " + Game.Player.GetUpgradeValue(i, Game.Player.UpgradeLevels[i] + 1);
                else
                    upgrade_current.DisplayedString = Game.Player.GetUpgradeValue(i, Game.Player.UpgradeLevels[i]) + " (MAX)";
                FloatRect upgrade_currentRect = upgrade_current.GetLocalBounds();
                upgrade_current.Origin = new Vector2f(upgrade_currentRect.Width / 2.0f, 0);
                upgrade_current.Position = new Vector2f(upgrade_back.Size.X / 2, 120);
                upgrade_current.Color = new Color(255, 255, 255, (byte)(pickable ? 255 : 120));
                upgrade.AddChild(upgrade_current);

                
                AddChild(upgrade);
                if (pickable)
                    UpgradeButtons.Add(upgrade);
                else
                    UpgradeButtons.Add(null);
            }
            SelectUpgrade(CurrentSelection);
        }
        public override void Init()
        {
            HoldButtonTimer = new Timer(500);
            HoldButtonTimer.AutoReset = false;
            HoldButtonTimer.Elapsed += HoldButtonTimerReleased;

            Game.Window.KeyPressed += OnKeyPressed;
            Game.Window.KeyReleased += OnKeyReleased;
            Game.NewWindow += OnNewWindow;

            Game.Pause();

            base.Init();
        }
        public override void Deinit()
        {
            if (HoldButtonTimer != null)
            {
                HoldButtonTimer.Stop();
                HoldButtonTimer.Dispose();
                HoldButtonTimer = null;
            }

            Game.NewWindow -= OnNewWindow;
            Game.Window.KeyPressed -= OnKeyPressed;
            Game.Window.KeyReleased -= OnKeyReleased;

            Game.Resume();

            base.Deinit();
        }
        private void OnNewWindow(Object sender, EventArgs e)
        {
            Game.Window.KeyPressed += OnKeyPressed;
            Game.Window.KeyReleased += OnKeyReleased;
        }

        private void OnKeyReleased(Object sender, KeyEventArgs e)
        {
            if (Game.KeyIsNotAllowed(e.Code))
                return;

            HoldButtonTimer.Stop();

            SelectUpgrade(CurrentSelection == UpgradeNames.Count - 1 ? 0 : CurrentSelection + 1);
        }
        private void OnKeyPressed(Object sender, KeyEventArgs e)
        {
            if (Game.KeyIsNotAllowed(e.Code))
                return;

            HoldButtonTimer.Start();
        }

        private void HoldButtonTimerReleased(Object source, ElapsedEventArgs e)
        {
            BuyCurrentUpgrade();

            if (Parent != null)
            {
                Parent.RemoveChild(this);
                Game.UpgradeMenu = null;
            }
        }

        private void BuyCurrentUpgrade()
        {
            Game.Player.LevelUpUpgrade(CurrentSelection);

        }
        private void SelectUpgrade(int no)
        {
            // unselect current button
            DisplayObject button = UpgradeButtons[CurrentSelection];
            RectangleShape back = (RectangleShape)button.GetChildAt(0);
            back.FillColor = new Color(0, 0, 0, 100);

            // select button
            while (UpgradeButtons[no] == null)
                no = no == UpgradeNames.Count - 1 ? 0 : no + 1;

            button = UpgradeButtons[no];
            back = (RectangleShape) button.GetChildAt(0);
            back.FillColor = new Color(255, 255, 255, 20);

            CurrentSelection = no;
        }

    }
}
