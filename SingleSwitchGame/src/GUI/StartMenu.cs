using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class StartMenu : SimpleSingleSwitchMenu
    {

        private HowToPlay HowToPlay;
        /// <summary>The max level an upgrade can be leveled.</summary>
        public readonly List<uint> UpgradeLevelCaps = new List<uint>() { 45, 20, 30, 1000 };

        public StartMenu(Game game)
            : base(game) { }
        public override void Init()
        {
            PauseWhenOpen = false;
            base.Init();
        }
        protected override void Setup()
        {
            Title = "";
            ButtonNames = new List<string>()
            {
                "Play",
                "How to Play",
                "Exit",
            };
            StripWidth = Game.Size.X;
            StartY = 800;
            ButtonDescriptions = new List<string>() { };
            ButtonHeight = 75;
            ButtonGap = 150;
            HelpY = 750;

            // Background
            if (Game.GraphicsMode == Game.GRAPHICSMODE_NORMAL)
            {
                Graphics.GetSprite("assets/sprites/background_blueprint_tile.png");
                RectangleShape Water = new RectangleShape(new Vector2f(Game.Size.X, Game.Size.Y));
                Water.FillColor = new Color(40, 118, 188);
                AddChild(Water);

                WaterRipples WaterRipplesBelow = new WaterRipples(Game, new Vector2f(Game.Size.X + 40, Game.Size.Y + 40), 120, 10, new Color(68, 131, 186));
                WaterRipplesBelow.Position = new Vector2f(-40, -40);
                AddChild(WaterRipplesBelow);

                WaterRipples WaterRipples = new WaterRipples(Game, new Vector2f(Game.Size.X, Game.Size.Y), 120, 10, new Color(80, 158, 228));
                AddChild(WaterRipples);

                //VoronoiDiagram WaterEffect = new VoronoiDiagram(this, new Vector2f(Size.X, Size.Y));
                //Layer_Background.AddChild(WaterEffect);
            }
            else if (Game.GraphicsMode == Game.GRAPHICSMODE_BLUEPRINT)
            {
                Sprite BluePrintBackground = Graphics.GetSprite("assets/sprites/background_blueprint_tile.png");
                BluePrintBackground.Texture.Repeated = true;
                BluePrintBackground.TextureRect = new IntRect(0, 0, (int)Game.Size.X, (int)Game.Size.Y);
                AddChild(BluePrintBackground);
            }
            Game.Resume();

            base.Setup();
            RemoveChild(Back);
            RemoveChild(Dim);

            // Title
            Sprite title = Graphics.GetSprite(Graphics.ASSETS_SPRITES + "gui/Title_Cannon_Island_Defence.png");
            FloatRect titleRect = title.GetLocalBounds();
            title.Position = new Vector2f((Game.Size.X - titleRect.Width) / 2, 100);
            AddChild(title);

            // Credits
            RectangleShape credits_back = new RectangleShape(new Vector2f(Game.Size.X, 40));
            credits_back.Position = new Vector2f(0, Game.Size.Y - 40);
            credits_back.FillColor = new Color(0, 0, 0, 60);
            AddChild(credits_back);

            Text credit_corey = new Text("created by Corey Zeke Womack (torrunt.net)", Game.TidyHand, 24);
            credit_corey.Position = new Vector2f(5, Game.Size.Y - 35);
            AddChild(credit_corey);

            Text credit_music = new Text("music by luigisounds.newgrounds.com", Game.TidyHand, 24);
            FloatRect textRect = credit_music.GetLocalBounds();
            credit_music.Origin = new Vector2f(textRect.Width, 0);
            credit_music.Position = new Vector2f(Game.Size.X - 5, Game.Size.Y - 35);
            AddChild(credit_music);
        }
        public override void OnRemoved()
        {
            base.OnRemoved();
            Game.StartMenu = null;
        }

        protected override void ActivateButton()
        {
            if (HowToPlay != null && HowToPlay.Parent != null)
                return;

            switch (CurrentSelection)
            {
                case 0:
                {
                    // Play
                    Game.Start();
                    if (Parent != null)
                        Parent.RemoveChild(this);
                }
                break;
                case 1:
                {
                    // How to Play
                    HowToPlay = new HowToPlay(Game);
                    AddChild(HowToPlay);
                }
                break;
                case 2:
                {
                    // Exit
                    Game.CloseWindow();
                }
                break;
            }
        }

    }
}
