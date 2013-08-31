using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame.src.Display
{
    class SpriteAnimationFrame
    {
        SpriteAnimationFrame(uint number, int x, int y, int width, int height)
        {
            this.Number = number;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        public uint Number;
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    class SpriteAnimation
    {
        public string Name = "";
        public uint CurrentFrame = 0;
        public uint TotalFrames = 0;

        public bool Playing = false;
        public bool Looping { get { return Looping; } set { Looping = value; Finished = false; } }
        public bool Finished = false;

        private Sprite Sprite = null;
        private SpriteAnimationFrame CurrentFrameInfo = null;
        private List<SpriteAnimationFrame> Frames = new List<SpriteAnimationFrame>();

        public SpriteAnimation(Sprite sprite)
        {
            this.Sprite = sprite;
        }

        public bool SetFrame(uint number)
        {
            if (number >= TotalFrames)
		        return false;

            CurrentFrame = number;
            Finished = false;
            
            Sprite.TextureRect = GetIntRect();
            Sprite.Origin = new Vector2f(CurrentFrameInfo.Width / 2, CurrentFrameInfo.Height / 2);

	        return true;
        }
        public void NextFrame() { SetFrame(Math.Min(CurrentFrame + 1, TotalFrames)); }
        public void PrevFrame() { SetFrame(Math.Max(CurrentFrame - 1, 1)); }
        public void Update()
        {
            if (Looping || CurrentFrame < TotalFrames - 1)
                NextFrame();
            else
                Finished = true;
        }

        public void Stop() { Playing = false; }
        public void Play() { Playing = true; }

        public void GotoAndPlay(uint number) { SetFrame(number); Play(); }
        public void GotoAndStop(uint number) { SetFrame(number); Stop(); }

        public IntRect GetIntRect() { return new IntRect(CurrentFrameInfo.X, CurrentFrameInfo.Y, CurrentFrameInfo.Width, CurrentFrameInfo.Height); }
    }
}
