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

        // Layers
        public Layer Layer_Background;
        public Layer Layer_Objects;
        public Layer Layer_GUI;
        public Layer Layer_BlackBars;

        // Objects
        public HeadsUpDisplay HUD;
        public Cannon Player;
        public CircleShape Island;
        public CircleShape Hill;


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
            Layer_Objects = new Layer();
            Layer_GUI = new Layer();
            Layer_BlackBars = new Layer();

            // Black Bars (for fullscreen)
            RectangleShape BlackBarLeft = new RectangleShape(new Vector2f(2000, 5000));
            BlackBarLeft.Position = new Vector2f(-BlackBarLeft.Size.X, 0);
            BlackBarLeft.FillColor = new Color(0, 0, 0);
            Layer_BlackBars.AddChild(BlackBarLeft);
            RectangleShape BlackBarRight = new RectangleShape(new Vector2f(2000, 5000));
            BlackBarRight.Position = new Vector2f(Size.X, 0);
            BlackBarRight.FillColor = new Color(0, 0, 0);
            Layer_BlackBars.AddChild(BlackBarRight);

            // Background
            Sprite BluePrintBackground = Graphics.GetSprite("assets/sprites/background_blueprint_tile.png");
            BluePrintBackground.Texture.Repeated = true;
            BluePrintBackground.TextureRect = new IntRect(0, 0, (int)Window.Size.X, (int)Window.Size.Y);
            Layer_Background.AddChild(BluePrintBackground);
            
                // Island
            float IslandRadius = 240;
            Island = new CircleShape(IslandRadius);
            Island.Origin = new Vector2f(IslandRadius, IslandRadius);
            Island.Position = new Vector2f(Window.Size.X / 2, Window.Size.Y / 2);
            Island.FillColor = new Color(0, 0, 0, 0);
            Island.OutlineThickness = 2;
            Island.OutlineColor = new Color(250, 250, 250);
            Island.SetPointCount(80);
            Layer_Background.AddChild(Island);
            
                // Hill
            float HillRadius = 30;
            Hill = new CircleShape(HillRadius);
            Hill.Origin = new Vector2f(HillRadius, HillRadius);
            Hill.Position = new Vector2f(Window.Size.X / 2, Window.Size.Y / 2);
            Hill.FillColor = new Color(0, 0, 0, 0);
            Hill.OutlineThickness = 2;
            Hill.OutlineColor = new Color(250, 250, 250);
            Hill.SetPointCount(50);
            Layer_Background.AddChild(Hill);
            
            // Player (Cannon)
            Player = new Cannon(this);
            Player.SetPosition(Window.Size.X / 2, Window.Size.Y / 2);
            Layer_Objects.AddChild(Player);
            Player.SetPlayer(true);

            // HUD
            HUD = new HeadsUpDisplay(this);
            Layer_GUI.AddChild(HUD);

            // Test
                // Add Bat, make it the player
            Bat Bat = new Bat(this);
            Bat.SetPosition(100, 100);
            Layer_Objects.AddChild(Bat);
            Bat.SetPlayer(true);
                
                // Add Bat, give it AI and set the player as it's Target
            Bat Bat2 = new Bat(this);
            Bat2.Model.Color = new Color(255, 150, 150);
            Bat2.SetPosition(300, 300);
            Layer_Objects.AddChild(Bat2);
            Bat2.SetAI(new ArtificialIntelligence());
            Bat2.AI.SetTarget(Bat);
            //bat2.ai.AddWaypointsToPath(new SFML.Window.Vector2f(500, 300), new SFML.Window.Vector2f(500, 100), new SFML.Window.Vector2f(200, 150));

                // Draw text on the GUI layer
            Text Text = new Text("Single Switch Game", TidyHand, 60);
            Text.Position = new Vector2f(4, 2);
            Layer_GUI.AddChild(Text);
            
            //Music music = new Music(@"assets/sound/OrchestralTheme1.ogg");
            //music.Play();
        }
        public void Stop()
        {
            if (!Started)
                return;
            Started = false;
            Running = false;

            Layer_Background.Clear();
            Layer_Objects.Clear();
            Layer_GUI.Clear();
        }
        public void Reset() { Stop(); Start(); }

        public void Draw()
        {
            Window.Draw(Layer_Background);
            Window.Draw(Layer_Objects);
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


        // Window Management
        public void CreateWindow()
        {
            Window = new RenderWindow(new VideoMode(ResolutionDefault.X, ResolutionDefault.Y), "SingleSwitchGame", WindowStyle, WindowSettings);
            Window.Closed += new EventHandler(OnClose);
            Window.KeyReleased += new EventHandler<KeyEventArgs>(OnKeyReleased);
            Window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);

            if (WindowStyle == Styles.None)
            {
                Window.Size = new Vector2u(VideoMode.DesktopMode.Width, VideoMode.DesktopMode.Height);
                Window.Position = new Vector2i(0, 0);
                float difference = (float)VideoMode.DesktopMode.Height / (float)ResolutionDefault.Y;
                View view = new View(new FloatRect(-(VideoMode.DesktopMode.Width - (VideoMode.DesktopMode.Width * difference))/2, 0, ResolutionDefault.X * (2 - difference), ResolutionDefault.Y));
                Window.SetView(view);
            }
            else
            {
                Window.Size = WindowSizeDefault;
                Window.Position = new Vector2i((int)((VideoMode.DesktopMode.Width - WindowSizeDefault.X) / 2), (int)((VideoMode.DesktopMode.Height - WindowSizeDefault.Y) / 2));
                Window.SetView(Window.DefaultView);
            }
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
                case Keyboard.Key.F11: ToggleFullscreen(); break;
            }
        }
        private void OnMouseButtonPressed(Object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
