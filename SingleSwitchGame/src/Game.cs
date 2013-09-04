using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using System.Collections.Generic;
using System;

namespace SingleSwitchGame
{
    class Game
    {
        public RenderWindow Window;

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

        // Objects
        public HeadsUpDisplay HUD;
        public Cannon Player;
        public CircleShape Island;
        public CircleShape Hill;


        public Game(ref RenderWindow window)
        {
            this.Window = window;
            Started = false;
            Running = false;
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
            
            // Background
            Sprite BluePrintBackground = Graphics.GetSprite("assets/sprites/background_blueprint_tile.png");
            BluePrintBackground.Texture.Repeated = true;
            BluePrintBackground.TextureRect = new IntRect(0, 0, (int)Window.Size.X, (int)Window.Size.Y);
            Layer_Background.AddChild(BluePrintBackground);
            
                // Island
            float IslandRadius = 140;
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
            Text Text = new Text("Single Switch Game", TidyHand);
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
    }
}
