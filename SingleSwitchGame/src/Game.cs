using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace SingleSwitchGame
{
    class Game
    {
        // Window
        public RenderWindow Window;
        public event EventHandler NewWindow;
        private ContextSettings WindowSettings;
        private Styles WindowStyle;
        private readonly Vector2u ResolutionDefault = new Vector2u(1920, 1440/*1080*/);
        private readonly Vector2u WindowSizeDefault = new Vector2u(960, 720/*540*/);
        private bool Fullscreen = false;
        /// <summary> Desired FPS </summary>
        private const float FPS = 60.0f;

        public FloatRect Bounds;

        private bool Started;
        private bool Running;

        private List<Entity> UpdateList = new List<Entity>();
        private int UpdateListIndex = 0;

        public static Font Arial = new Font("assets/arial.ttf");
        public static Font TidyHand = new Font("assets/TidyHand.ttf");

        public const uint GRAPHICSMODE_NORMAL = 0;
        public const uint GRAPHICSMODE_BLUEPRINT = 1;
        public uint GraphicsMode = GRAPHICSMODE_NORMAL;

        // Layers
        public Layer Layer_Background;
        public Layer Layer_Other;
        public Layer Layer_Objects;
        public Layer Layer_OtherAbove;
        public Layer Layer_GUI;
        public Layer Layer_BlackBars;

        // Objects
        public AIManager AIManager;
        public HeadsUpDisplay HUD;
        public UpgradeMenuGUI UpgradeMenu;

        public Cannon Player;
        public CircleShape Island;
        public CircleWaves IslandWaves;
        public CircleShape Hill;

        public Music Music;

        public Game()
        {
            Started = false;
            Running = false;

            // Setup Window
            Bounds = new FloatRect(0, 0, ResolutionDefault.X, ResolutionDefault.Y);
            WindowSettings = new ContextSettings();
            WindowSettings.AntialiasingLevel = 6;
            WindowStyle = Styles.Close;
            CreateWindow();

            // Black Bars (for fullscreen)
            Layer_BlackBars = new Layer();
            RectangleShape BlackBarLeft = new RectangleShape(new Vector2f(2000, 5000));
            BlackBarLeft.Position = new Vector2f(-BlackBarLeft.Size.X, 0);
            BlackBarLeft.FillColor = new Color(0, 0, 0);
            Layer_BlackBars.AddChild(BlackBarLeft);
            RectangleShape BlackBarRight = new RectangleShape(new Vector2f(2000, 5000));
            BlackBarRight.Position = new Vector2f(Size.X, 0);
            BlackBarRight.FillColor = new Color(0, 0, 0);
            Layer_BlackBars.AddChild(BlackBarRight);

            // Start Game
            Start();

            // Game Loop
            Stopwatch clock = new Stopwatch();
            clock.Start();
            while (Window.IsOpen())
            {
                // Process events
                Window.DispatchEvents();

                if (clock.Elapsed.TotalSeconds >= (1.0f / FPS))
                {
                    // Clear screen
                    Window.Clear();

                    // Update Game 
                    Update((float)clock.Elapsed.TotalSeconds);
                    clock.Restart();

                    // Draw Game
                    Draw();

                    // Update the window
                    Window.Display();
                }
            }
        }

        public void Start()
        {
            if (Started)
                return;
            Started = true;
            Running = true;

            Layer_Background = new Layer();
            Layer_Other = new Layer();
            Layer_Objects = new Layer();
            Layer_OtherAbove = new Layer();
            Layer_GUI = new Layer();

            // Managers
            AIManager = new AIManager(this);
            AIManager.StartTestInfantryTimer();

            // Background
            if (GraphicsMode == GRAPHICSMODE_NORMAL)
            {
                RectangleShape Water = new RectangleShape(new Vector2f(Size.X, Size.Y));
                Water.FillColor = new Color(40, 118, 188);
                Layer_Background.AddChild(Water);

                WaterRipples WaterRipplesBelow = new WaterRipples(this, new Vector2f(Size.X + 40, Size.Y + 40), 120, 10, new Color(68, 131, 186));
                WaterRipplesBelow.Position = new Vector2f(-40, -40);
                Layer_Background.AddChild(WaterRipplesBelow);

                WaterRipples WaterRipples = new WaterRipples(this, new Vector2f(Size.X, Size.Y), 120, 10, new Color(80, 158, 228));
                Layer_Background.AddChild(WaterRipples);
            }
            else if (GraphicsMode == GRAPHICSMODE_BLUEPRINT)
            {
                Sprite BluePrintBackground = Graphics.GetSprite("assets/sprites/background_blueprint_tile.png");
                BluePrintBackground.Texture.Repeated = true;
                BluePrintBackground.TextureRect = new IntRect(0, 0, (int)Size.X, (int)Size.Y);
                Layer_Background.AddChild(BluePrintBackground);
            }

                // Island
            float IslandRadius = 240;

            if (GraphicsMode == GRAPHICSMODE_NORMAL)
            {
                Sprite IslandTexture = Graphics.GetSprite("assets/sprites/island.png");
                IslandTexture.Origin = new Vector2f(IslandRadius, IslandRadius);
                IslandTexture.Position = new Vector2f((Size.X/2) + 1, (Size.Y/2));
                Layer_Background.AddChild(IslandTexture);
            }

            Island = new CircleShape(IslandRadius);
            Island.Origin = new Vector2f(IslandRadius, IslandRadius);
            Island.Position = new Vector2f(Size.X / 2, Size.Y / 2);
            if (GraphicsMode == GRAPHICSMODE_NORMAL)
            {
                Island.FillColor = new Color(0, 0, 10, 0);
                Island.OutlineThickness = 15;
                Island.OutlineColor = new Color(138, 104, 0, 100);
            }
            else if (GraphicsMode == GRAPHICSMODE_BLUEPRINT)
            {
                Island.FillColor = new Color(0, 0, 10, 30);
                Island.OutlineThickness = 2;
                Island.OutlineColor = new Color(250, 250, 250);
            }
            Island.SetPointCount(80);
            Layer_Background.AddChild(Island);

            IslandWaves = new CircleWaves(this, IslandRadius, 0.1f, 1.5f, 6, 80);
            if (GraphicsMode == GRAPHICSMODE_NORMAL)
                IslandWaves.Colour = new Color(80, 158, 228);
            IslandWaves.Position = Island.Position;
            Layer_Background.AddChild(IslandWaves);
            
                // Hill
            float HillRadius = 30;
            Hill = new CircleShape(HillRadius);
            Hill.Origin = new Vector2f(HillRadius, HillRadius);
            Hill.Position = new Vector2f(Size.X / 2, Size.Y / 2);
            if (GraphicsMode == GRAPHICSMODE_NORMAL)
            {
                Hill.FillColor = new Color(50, 50, 50, 150);
                Hill.OutlineThickness = 4;
                Hill.OutlineColor = new Color(0, 0, 0, 215);
            }
            else if (GraphicsMode == GRAPHICSMODE_BLUEPRINT)
            {
                Hill.FillColor = new Color(0, 0, 10, 50);
                Hill.OutlineThickness = 2;
                Hill.OutlineColor = new Color(250, 250, 250);
            }
            Hill.SetPointCount(50);
            Layer_Background.AddChild(Hill);
            
            // Player (Cannon)
            Player = new Cannon(this);
            Player.SetPosition(Size.X / 2, Size.Y / 2);
            Layer_OtherAbove.AddChild(Player);
            Player.SetPlayer(true);
            
            // HUD
            HUD = new HeadsUpDisplay(this);
            HUD.SetHealth(Player.Health);
            Layer_GUI.AddChild(HUD);

            //Music = new Music("assets/sound/OrchestralTheme1.ogg");
            //Music.Play();
        }
        public void Stop()
        {
            if (!Started)
                return;
            Started = false;
            Running = false;

            if (Music != null)
            {
                Music.Stop();
                Music = null;
            }

            // Managers
            AIManager.StopTestInfantryTimer();
            AIManager = null;

            // Layers
            Layer_Background.Clear();
            Layer_Other.Clear();
            Layer_Objects.Clear();
            Layer_OtherAbove.Clear();
            Layer_GUI.Clear();

            Player = null;
            HUD = null;

            UpgradeMenu = null;

            UpdateListIndex = 0;
        }
        public void Reset() { Stop(); Start(); }

        public void Draw()
        {
            Window.Draw(Layer_Background);
            Window.Draw(Layer_Other);
            Window.Draw(Layer_Objects);
            Window.Draw(Layer_OtherAbove);
            Window.Draw(Layer_GUI);
            Window.Draw(Layer_BlackBars);
        }

        public void Pause()
        {
            if (!Running)
                return;
            Running = false;
        }
        public void Resume()
        {
            if (Running)
                return;
            Running = true;
        }

        public void Update(float dt)
        {
            if (!Running)
                return;

            for (UpdateListIndex = 0; UpdateListIndex < UpdateList.Count; UpdateListIndex++)
                UpdateList[UpdateListIndex].Update(dt);
        }

        public void AddToUpdateList(Entity entity) { UpdateList.Add(entity); }
        /// <summary>Returns true if it was removed.</summary>
        public bool RemoveFromUpdateList(Entity entity)
        {
            for (int i = 0; i < UpdateList.Count; i++)
			{
                if (UpdateList[i].Equals(entity))
                {
                    UpdateList.RemoveAt(i);
                    if (i <= UpdateListIndex)
                        UpdateListIndex--;
                    return true;
                }
            }
            return false;
        }

        public bool HasStarted() { return Started; }
        public bool IsRunning() { return Running; }

        public Vector2u Size { get { return ResolutionDefault; } set {} }

        // Debugging / Testing
        public void Debug_ShowCollision(bool value = true)
        {
            for (int i = 0; i < Layer_Objects.NumChildren; i++)
            {
                if (Layer_Objects.GetChildAt(i) is CollisionEntity)
                    Layer_Objects.GetChildAt(i).Debug_ShowCollision(value);
            }
        }


        // Window Management
        public void CreateWindow()
        {
            Window = new RenderWindow(new VideoMode(ResolutionDefault.X, ResolutionDefault.Y), "Single Switch Game", WindowStyle, WindowSettings);
            Window.Closed += OnClose;
            Window.KeyReleased += OnKeyReleased;
            Window.MouseButtonPressed += OnMouseButtonPressed;
            
            if (WindowStyle == Styles.None)
            {
                Window.Size = new Vector2u(VideoMode.DesktopMode.Width, VideoMode.DesktopMode.Height);
                Window.Position = new Vector2i(0, 0);
                float difference = (float)ResolutionDefault.Y / (float)VideoMode.DesktopMode.Height;
                View view = new View(new FloatRect((VideoMode.DesktopMode.Width - (ResolutionDefault.X * difference))/2, 0, ResolutionDefault.X * difference, ResolutionDefault.Y));
                Window.SetView(view);
            }
            else
            {
                Window.Size = WindowSizeDefault;
                Window.Position = new Vector2i((int)((VideoMode.DesktopMode.Width - WindowSizeDefault.X) / 2), (int)((VideoMode.DesktopMode.Height - WindowSizeDefault.Y) / 2));
                Window.SetView(Window.DefaultView);
            }

            if (NewWindow != null)
                NewWindow(this, EventArgs.Empty);
        }
        void OnClose(Object sender, EventArgs e)
        {
            Window.Close();
        }

        public void ToggleFullscreen()
        {
            Fullscreen = !Fullscreen;
            WindowStyle = Fullscreen ? Styles.None : Styles.Close;

            Window.Close();
            CreateWindow();
        }

        private void OnKeyReleased(Object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Escape:
                {
                    if (Running)
                        Pause();
                    else
                        Resume();
                    break;
                }
                case Keyboard.Key.F11: ToggleFullscreen(); break;

                // Testing
                case Keyboard.Key.F3:
                {
                    // Upgrade Menu
                    if (UpgradeMenu == null)
                    {
                        if (Player != null)
                        {
                            UpgradeMenu = new UpgradeMenuGUI(this);
                            Layer_GUI.AddChild(UpgradeMenu);
                        }
                    }
                    else
                    {
                        if (UpgradeMenu.Parent != null)
                            UpgradeMenu.Parent.RemoveChild(UpgradeMenu);
                        UpgradeMenu = null;
                    }
                }
                break;
                case Keyboard.Key.F2:
                {
                    GraphicsMode = GraphicsMode == GRAPHICSMODE_NORMAL ? GRAPHICSMODE_BLUEPRINT : GRAPHICSMODE_NORMAL;
                    Reset();
                }
                break;
            }
        }

        public bool KeyIsNotAllowed(Keyboard.Key key)
        {
            return key == Keyboard.Key.Escape || (key >= Keyboard.Key.F1 && key <= Keyboard.Key.F15);
        }

        private void OnMouseButtonPressed(Object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
