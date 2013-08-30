using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using System.Collections.Generic;
using System;

namespace SingleSwitchGame
{
    class Game
    {
        private bool Started;
        private bool Running;

        private List<Entity> UpdateList = new List<Entity>();

        public static Font Arial = new Font("assets/arial.ttf");

        // Layers
        Layer Layer_Background;
        Layer Layer_Objects;
        Layer Layer_GUI;


        bool DisplayFPS = true;
        Text FPS;


        public Game(ref RenderWindow window)
        {
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
            Text Text = new Text("Single Switch Game", Arial);
            Layer_GUI.AddChild(Text);

            if (DisplayFPS)
            {
                FPS = new Text("fps", Arial, 14);
                FPS.Position = new Vector2f(765, 0);
                Layer_GUI.AddChild(FPS);
            }
            
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

        public void Update(float dt)
        {
            foreach (Entity entity in UpdateList)
                entity.Update(dt);

            if (DisplayFPS)
                FPS.DisplayedString = (1 / dt).ToString("00.0");
        }

        public void Draw(ref RenderWindow window)
        {
            window.Draw(Layer_Background);
            window.Draw(Layer_Objects);
            window.Draw(Layer_GUI);
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

        public void AddToUpdateList(Entity entity) { UpdateList.Add(entity); }
        public void RemoveFromUpdateList(Entity entity) { UpdateList.Remove(entity); }

        public bool HasStarted() { return Started; }
        public bool IsRunning() { return Running; }
    }
}
