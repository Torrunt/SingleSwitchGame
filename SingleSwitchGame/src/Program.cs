using System;
using SFML.Audio;
using SFML.Window;
using SFML.Graphics;
using System.Diagnostics;

namespace SingleSwitchGame
{
    class Program
    {
        /// <summary> Desired FPS </summary>
        const float FPS = 60.0f;
        
        static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        static void Main(string[] args)
        {
            // Create the main window
            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "SingleSwitchGame");
            window.Closed += new EventHandler(OnClose);

            // Game Loop
            Game game = new Game(ref window);
            game.start();
            Stopwatch clock = new Stopwatch();
            clock.Start();

            while (window.IsOpen())
            {
                // Process events
                window.DispatchEvents();
                
                if (clock.Elapsed.TotalSeconds >= (1.0f / FPS))
                {
                    // Clear screen
                    window.Clear();

                    //Console.Write((float)clock.Elapsed.TotalSeconds + "\n");
                    //Console.Write("fps: " + (1 / (float)clock.Elapsed.TotalSeconds) + "\n");

                    // Update Game 
                    game.update((float)clock.Elapsed.TotalSeconds);
                    clock.Restart();

                    // Draw Game
                    game.draw(ref window);

                    // Update the window
                    window.Display();
                }
            }
        }

    }
}
