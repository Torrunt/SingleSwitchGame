using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class AnimatedSprite : Entity
    {
        public AnimatedSpriteData Data;
        public int CurrentFrame = 0;
        public int TotalFrames = 0;
        private AnimatedSpriteFrame CurrentFrameInfo = null;

        private AnimatedSprite CurrentNestedAnimatedSprite;

        public bool Playing = true;
        private bool _Looping = true;
        public bool Looping { get { return _Looping; } set { _Looping = value; Finished = false; } }
        public bool Finished = false;

        public float FPS = 60.0f;
        /// <summary> Keeps track of when to change frame. Resets on frame change. </summary>
        public float CurrentTime;
        /// <summary> Length of a frame according to the current fps. </summary>
        public float FrameLength;

        public AnimatedSprite(Game game, AnimatedSpriteData data) : base(game)
        {
            Data = data;
            TotalFrames = Data.Frames.Count;

            Model = new DisplayObject();
            if (Data.Texture != null)
            {
                Sprite sprite = new Sprite(Data.Texture);
                sprite.Texture.Smooth = true;
                Model.AddChild(sprite);
            }
            AddChild(Model);

            CurrentTime = 0;
            FrameLength = 1 / FPS;

            SetFrame(0);
        }

        public override void Update(float dt)
        {
            if (!Playing)
                return;

            CurrentTime += dt;

            while (CurrentTime > FrameLength)
            {
                if (Looping || CurrentFrame < TotalFrames - 1)
                {
                    if (CurrentFrame == TotalFrames - 1)
                        SetFrame(0);
                    else
                        NextFrame();
                }
                else
                    Finished = true;

                CurrentTime -= FrameLength;
            }
        }

        public bool SetFrame(int number)
        {
            if (number >= TotalFrames)
		        return false;

            CurrentFrame = number;
            Finished = false;

            if (CurrentNestedAnimatedSprite != null)
            {
                RemoveChild(CurrentNestedAnimatedSprite);
                CurrentNestedAnimatedSprite = null;
            }
            if (Data.Frames[(int)CurrentFrame] is AnimatedSpriteDataReference)
            {
                Model.Visible = false;
                // TODO - instead of re-creating AnimateSprites, loop through Frames and create Array filled with all the Nested AnimatedSprites.
                CurrentNestedAnimatedSprite = new AnimatedSprite(Game, Data.Frames[(int)CurrentFrame].Data);
                CurrentNestedAnimatedSprite.Position = Data.Frames[(int)CurrentFrame].Origin;
                AddChild(CurrentNestedAnimatedSprite);

                return false;
            }
            else
                Model.Visible = true; 

            if (Data.Frames[(int)CurrentFrame] is int)
                CurrentFrameInfo = Data.Frames[Data.Frames[(int)CurrentFrame]];
            else
                CurrentFrameInfo = Data.Frames[(int)CurrentFrame];
            
            Model.GetChildAt(0).TextureRect = GetIntRect();
            Model.GetChildAt(0).Origin = CurrentFrameInfo.Origin;

	        return true;
        }
        public void NextFrame() { SetFrame(Math.Min(CurrentFrame + 1, TotalFrames)); }
        public void PrevFrame() { SetFrame(Math.Max(CurrentFrame - 1, 0)); }

        public void Stop() { Playing = false; }
        public void Play() { Playing = true; }

        public void GotoAndPlay(int number) { SetFrame(number); Play(); }
        public void GotoAndStop(int number) { SetFrame(number); Stop(); }

        public IntRect GetIntRect() { return new IntRect(CurrentFrameInfo.X, CurrentFrameInfo.Y, CurrentFrameInfo.Width, CurrentFrameInfo.Height); }

    }
}
