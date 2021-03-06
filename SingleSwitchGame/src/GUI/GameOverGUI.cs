﻿using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class GameOverGUI : GraphicalUserInterface
    {
        public GameOverGUI(Game game)
            : base(game)
        {
            RectangleShape dim = new RectangleShape(new Vector2f(Game.Size.X, Game.Size.Y));
            dim.FillColor = new Color(0, 0, 0, 100);
            AddChild(dim);

            RectangleShape back = new RectangleShape(new Vector2f(Game.Size.X, 490));
            back.FillColor = new Color(0, 0, 0, 100);
            back.Position = new Vector2f(0, Game.Size.Y / 2 - (back.Size.Y/2));
            AddChild(back);

            FloatRect textRect;

            Text gameOver = new Text("Game Over", Game.TidyHand, 120);
            textRect = gameOver.GetLocalBounds();
            gameOver.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top  + textRect.Height/2.0f);
            gameOver.Position = new Vector2f(Game.Size.X/2, Game.Size.Y/2 - 190);
            AddChild(gameOver);

            Text msg = new Text("Press any key to retry.", Game.TidyHand, 35);
            textRect = msg.GetLocalBounds();
            msg.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);
            msg.Position = new Vector2f(Game.Size.X / 2, Game.Size.Y / 2 - 100);
            AddChild(msg);

            Text finalScore = new Text("Final Score", Game.TidyHand, 60);
            textRect = finalScore.GetLocalBounds();
            finalScore.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);
            finalScore.Position = new Vector2f(Game.Size.X / 2, Game.Size.Y / 2 + 120);
            AddChild(finalScore);
            
            Text score = new Text(Game.Player.GetScore().ToString("00000000"), Game.TidyHand, 50);
            textRect = score.GetLocalBounds();
            score.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);
            score.Position = new Vector2f(Game.Size.X / 2, Game.Size.Y / 2 + 180);
            AddChild(score);
        }

        public override void Init()
        {
            Game.Window.KeyReleased += OnKeyReleased;
            Game.NewWindow += OnNewWindow;
            Game.Window.MouseButtonReleased += OnMouseButtonReleased;

            base.Init();
        }
        public override void Deinit()
        {
            Game.NewWindow -= OnNewWindow;
            Game.Window.KeyReleased -= OnKeyReleased;
            Game.Window.MouseButtonReleased -= OnMouseButtonReleased;

            base.Deinit();
        }
        private void OnNewWindow(Object sender, EventArgs e = null)
        {
            Game.Window.KeyReleased += OnKeyReleased;
            Game.Window.MouseButtonReleased += OnMouseButtonReleased;
        }
        private void OnKeyReleased(Object sender, KeyEventArgs e = null)
        {
            if (e != null && Game.KeyIsNotAllowed(e.Code))
                return;

            Parent.RemoveChild(this);
            Game.Reset();
        }
        protected virtual void OnMouseButtonReleased(Object sender, MouseButtonEventArgs e = null)
        {
            OnKeyReleased(sender);
        }

        
    }
}
