using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using System.Collections.Generic;
using System;

namespace SingleSwitchGame
{
    class Game
    {
        private bool started;
        private bool running;

        private List<Entity> updateList = new List<Entity>();

        public static Font arial = new Font("assets/arial.ttf");

        // Layers
        Layer layer_background;
        Layer layer_objects;
        Layer layer_gui;


        bool displayFPS = true;
        Text fps;


        public Game(ref RenderWindow window)
        {
            started = false;
            running = false;
        }

        public void start()
        {
            if (started)
                return;
            started = true;
            running = true;

            layer_background = new Layer();
            layer_objects = new Layer();
            layer_gui = new Layer();
            
            // Test
                // Add Bat, make it the player
            Bat bat = new Bat(this);
            bat.setPosition(100, 100);
            layer_objects.addChild(bat);
            bat.setPlayer(true);
                
                // Add Bat, give it AI and set the player as it's target
            Bat bat2 = new Bat(this);
            bat2.model.Color = new Color(255, 150, 150);
            bat2.setPosition(300, 300);
            layer_objects.addChild(bat2);
            bat2.setAI(new ArtificialIntelligence());
            bat2.ai.setTarget(bat);
            //bat2.ai.addWayPointsToPath(new SFML.Window.Vector2f(500, 300), new SFML.Window.Vector2f(500, 100), new SFML.Window.Vector2f(200, 150));

                // Draw text on the GUI layer
            Text text = new Text("Single Switch Game", arial);
            layer_gui.addChild(text);

            if (displayFPS)
            {
                fps = new Text("fps", arial, 14);
                fps.Position = new Vector2f(765, 0);
                layer_gui.addChild(fps);
            }
            
            //Music music = new Music(@"assets/sound/OrchestralTheme1.ogg");
            //music.Play();
        }
        public void stop()
        {
            if (!started)
                return;
            started = false;
            running = false;

            layer_background.clear();
            layer_objects.clear();
            layer_gui.clear();
        }
        public void reset() { stop(); start(); }

        public void update(float dt)
        {
            foreach (Entity entity in updateList)
                entity.update(dt);

            if (displayFPS)
                fps.DisplayedString = (1 / dt).ToString("00.0");
        }

        public void draw(ref RenderWindow window)
        {
            window.Draw(layer_background);
            window.Draw(layer_objects);
            window.Draw(layer_gui);
        }

        public void pause()
        {
            if (!running)
                return;
            running = false;
        }
        public void resume()
        {
            if (running)
                return;
            running = true;
        }

        public void addToUpdateList(Entity entity) { updateList.Add(entity); }
        public void removeFromUpdateList(Entity entity) { updateList.Remove(entity); }

        public bool hasStarted() { return started; }
        public bool isRunning() { return running; }
    }
}
