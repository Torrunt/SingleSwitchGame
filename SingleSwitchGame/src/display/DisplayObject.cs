using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System;

namespace SingleSwitchGame
{

    class DisplayObject : Transformable, Drawable
    {
        private List<Object> Children = new List<Object>();

        public int NumChildren { get { return Children.Count; } }
        public void AddChild(Object child) { Children.Add(child); }
        public void RemoveChild(Object child) { Children.Remove(child); }
        public void Clear() { Children.Clear(); }

        public Object GetChildAt(int i) { return Children[i]; }

        public void Draw(RenderTarget Target, RenderStates states)
        {
            states.Transform *= Transform;
            foreach (Drawable child in Children)
                child.Draw(Target, states);
        }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2f(value, Position.Y); }
        }
        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2f(Position.X, value); }
        }
        public void SetPosition(float x, float y) { Position = new Vector2f(x, y); }
        public void SetPosition(Vector2f pos) { Position = pos; }

        public void Move(float offsetX, float offsetY) { Position = new Vector2f(X + offsetX, Y + offsetY); }
        public void Move(Vector2f offset) { Position = new Vector2f(X + offset.X, Y + offset.Y); }

        public float ScaleX
        {
            get { return Scale.X; }
            set { Scale = new Vector2f(value, Scale.Y); }
        }
        public float ScaleY
        {
            get { return Scale.Y; }
            set { Scale = new Vector2f(Scale.X, value); }
        }
        public void SetScale(float scaleX, float scaleY) { Scale = new Vector2f(scaleX, scaleY); }
        public void SetScale(Vector2f scale) { Scale = scale; }

        public void Rotate(float amount) { Rotation += amount; }
    }

    class Layer : DisplayObject { }
}
