using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System;

namespace SingleSwitchGame
{

    class DisplayObject : Transformable, Drawable
    {
        private List<object> children = new List<object>();

        public int numChildren { get { return children.Count; } }
        public void addChild(object child) { children.Add(child); }
        public void removeChild(object child) { children.Remove(child); }
        public void clear() { children.Clear(); }

        public object getChildAt(int i) { return children[i]; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            foreach (Drawable child in children)
                child.Draw(target, states);
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
        public void setPosition(float x, float y) { Position = new Vector2f(x, y); }
        public void setPosition(Vector2f pos) { Position = pos; }

        public void move(float offsetX, float offsetY) { Position = new Vector2f(X + offsetX, Y + offsetY); }
        public void move(Vector2f offset) { Position = new Vector2f(X + offset.X, Y + offset.Y); }

        public float scaleX
        {
            get { return Scale.X; }
            set { Scale = new Vector2f(value, Scale.Y); }
        }
        public float scaleY
        {
            get { return Scale.Y; }
            set { Scale = new Vector2f(Scale.X, value); }
        }
        public void setScale(float scaleX, float scaleY) { Scale = new Vector2f(scaleX, scaleY); }
        public void setScale(Vector2f scale) { Scale = scale; }

        public void rotate(float amount) { Rotation += amount; }
    }

    class Layer : DisplayObject { }
}
