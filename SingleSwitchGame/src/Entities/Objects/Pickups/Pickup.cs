using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    internal class Pickup : CollisionEntity
    {
        public Pickup(Game game, dynamic model = null) : base(game, (object) model)
        {
            Collision = new CircleShape(15);
            Collision.Origin = new Vector2f(15, 15);
            Collision.FillColor = new Color(0, 255, 0);
        }

        public virtual void Activate(dynamic obj)
        {
            if (Parent != null)
                Parent.RemoveChild(this);
        }

        protected virtual void MessagePopup(string msg)
        {
            MessageFade popup = new MessageFade(Game, msg, 35, Position);
            Game.Layer_GUI.AddChild(popup);
        }
}
}
