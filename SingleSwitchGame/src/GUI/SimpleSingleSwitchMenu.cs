using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using System.Timers;

namespace SingleSwitchGame
{
    class SimpleSingleSwitchMenu : GraphicalUserInterface
    {

        // Defaults
        protected string Title = "Title";
        protected float StripWidth = 0;
        protected float ButtonHeight = 200;
        protected float ButtonGap = 280;
        /// <summary>If 0, will use title.Position.Y + 200.</summary>
        protected float StartY = 0;
        protected float TitleY = 100;
        protected float HelpY = 220;

        protected List<string> ButtonNames = new List<string>()
        {
            "Item 1",
            "Item 2",
        };
        protected List<string> ButtonDescriptions = new List<string>()
        {
            "blah blah blah.",
            "blahdy blahdy blah.",
        };

        protected bool PauseWhenOpen = true;

        protected int CurrentSelection = 0;
        protected List<DisplayObject> Buttons = new List<DisplayObject>();

        private Timer HoldButtonTimer;

        protected RectangleShape Dim;
        protected RectangleShape Back;

        public SimpleSingleSwitchMenu(Game game)
            : base(game)
        {
            Setup();
        }
        protected virtual void Setup()
        {
            // dim game
            Dim = new RectangleShape(new Vector2f(Game.Size.X, Game.Size.Y));
            Dim.FillColor = new Color(0, 0, 0, 100);
            AddChild(Dim);

            // title
            Text title = new Text(Title, Game.TidyHand, 100);
            FloatRect textRect = title.GetLocalBounds();
            title.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);
            title.Position = new Vector2f(Game.Size.X / 2, TitleY);
            if (StartY == 0)
                StartY = title.Position.Y + 200;

            // back
            Back = new RectangleShape(new Vector2f(StripWidth == 0 ? textRect.Width + 300 : StripWidth, Game.Size.Y));
            Back.FillColor = new Color(0, 0, 0, 100);
            Back.Position = new Vector2f(title.Position.X - (Back.Size.X / 2), 0);
            AddChild(Back);

            AddChild(title);

            // help
            Text help = new Text("(tap to change selection, hold down to choose)", Game.TidyHand, 28);
            textRect = help.GetLocalBounds();
            help.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);
            help.Position = new Vector2f(Game.Size.X / 2, HelpY);
            AddChild(help);

            // upgrades
            for (int i = 0; i < ButtonNames.Count; i++)
            {
                bool pickable = IsPickable(i);

                DisplayObject button = new DisplayObject();
                button.Position = new Vector2f(Back.Position.X, StartY + (ButtonGap * i));

                // back
                RectangleShape button_back = new RectangleShape(new Vector2f(Back.Size.X, ButtonHeight));
                button_back.FillColor = new Color(0, 0, 0, (byte)(pickable ? 150 : 50));
                button_back.OutlineThickness = 6;
                button_back.OutlineColor = new Color(0, 0, 0, 0);
                button.AddChild(button_back);

                // name
                Text button_name = new Text(ButtonNames[i], Game.TidyHand, 50);
                FloatRect button_nameRect = button_name.GetLocalBounds();
                button_name.Origin = new Vector2f(button_nameRect.Left + button_nameRect.Width / 2.0f, 0);
                button_name.Position = new Vector2f(button_back.Size.X / 2, 0);
                if (!pickable)
                    button_name.Color = new Color(255, 255, 255, 50);
                button.AddChild(button_name);

                // description
                if (ButtonDescriptions.Count > i)
                {
                    Text button_description = new Text(ButtonDescriptions[i], Game.TidyHand, 30);
                    FloatRect button_descriptionRect = button_description.GetLocalBounds();
                    button_description.Origin = new Vector2f(button_descriptionRect.Width / 2.0f, 0);
                    button_description.Position = new Vector2f(button_back.Size.X / 2, 60);
                    button_description.Color = new Color(180, 180, 180, (byte)(pickable ? 255 : 120));
                    button.AddChild(button_description);
                }

                SetupButtonAdditional(i, button, button_back, pickable);

                AddChild(button);
                if (pickable)
                    Buttons.Add(button);
                else
                    Buttons.Add(null);
            }
            SelectButton(CurrentSelection);
        }
        /// <summary>Additonal Setup for buttons (made to be overridden)</summary>
        protected virtual void SetupButtonAdditional(int i, DisplayObject button, RectangleShape button_back, bool pickable) { }

        public override void Init()
        {
            HoldButtonTimer = new Timer(300);
            HoldButtonTimer.AutoReset = false;
            HoldButtonTimer.Elapsed += HoldButtonTimerReleased;

            Game.Window.KeyPressed += OnKeyPressed;
            Game.Window.KeyReleased += OnKeyReleased;
            Game.Window.MouseButtonPressed += OnMouseButtonPressed;
            Game.Window.MouseButtonReleased += OnMouseButtonReleased;
            Game.NewWindow += OnNewWindow;

            if (PauseWhenOpen)
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
            Game.Window.MouseButtonPressed -= OnMouseButtonPressed;
            Game.Window.MouseButtonReleased -= OnMouseButtonReleased;

            if (PauseWhenOpen)
                Game.Resume();

            base.Deinit();
        }
        private void OnNewWindow(Object sender, EventArgs e)
        {
            Game.Window.KeyPressed += OnKeyPressed;
            Game.Window.KeyReleased += OnKeyReleased;
            Game.Window.MouseButtonPressed += OnMouseButtonPressed;
            Game.Window.MouseButtonReleased += OnMouseButtonReleased;
        }

        private void OnKeyReleased(Object sender, KeyEventArgs e = null)
        {
            if (e != null && Game.KeyIsNotAllowed(e.Code))
                return;

            HoldButtonTimer.Stop();

            SelectButton(CurrentSelection == ButtonNames.Count - 1 ? 0 : CurrentSelection + 1);
        }
        private void OnKeyPressed(Object sender, KeyEventArgs e = null)
        {
            if (e != null && Game.KeyIsNotAllowed(e.Code))
                return;

            HoldButtonTimer.Start();
        }
        protected virtual void OnMouseButtonPressed(Object sender, MouseButtonEventArgs e = null)
        {
            OnKeyPressed(sender);
        }
        protected virtual void OnMouseButtonReleased(Object sender, MouseButtonEventArgs e = null)
        {
            OnKeyReleased(sender);
        }

        private void HoldButtonTimerReleased(Object source, ElapsedEventArgs e)
        {
            ActivateButton();
        }

        protected virtual void ActivateButton()
        {
        }
        protected virtual void SelectButton(int no)
        {
            // unselect current button
            DisplayObject button = Buttons[CurrentSelection];
            RectangleShape back = (RectangleShape)button.GetChildAt(0);
            back.FillColor = new Color(0, 0, 0, 150);
            back.OutlineColor = new Color(0, 0, 0, 0);

            // select button
            while (Buttons[no] == null)
                no = no == ButtonNames.Count - 1 ? 0 : no + 1;

            button = Buttons[no];
            back = (RectangleShape) button.GetChildAt(0);
            back.FillColor = new Color(255, 255, 255, 20);
            back.OutlineColor = new Color(255, 255, 255, 180);

            CurrentSelection = no;
        }

        /// <summary>Returns true if the button is pickable./summary>
        protected virtual bool IsPickable(int i)
        {
            return true;
        }

    }
}
