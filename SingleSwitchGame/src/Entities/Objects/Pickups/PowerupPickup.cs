using System;
using SFML.Graphics;
using SFML.Window;

namespace SingleSwitchGame
{
    class PowerupPickup : Pickup
    {
        private int Type = 0;

        private const float AnimationScaleMin = 0.8f;
        private const float AnimationScaleAcc = 0.005f;
        private int AnimationScaleDirection = 1;

        public PowerupPickup(Game game, int type) : base(game, null)
        {
            Type = type;

            //if (Game.GraphicsMode == Game.GRAPHICSMODE_NORMAL)
            byte alpha = 180;

            Model = new CircleShape(((CircleShape) Collision).Radius);
            Model.Origin = new Vector2f(Model.Radius, Model.Radius);
            Model.OutlineColor = new Color(255, 255, 255);
            Model.OutlineThickness = 3;

            switch (Type)
            {
                case Cannon.POWERUP_DOUBLE_EXPLOSION_RADIUS: Model.FillColor = new Color(255, 48, 48, alpha); break;
                case Cannon.POWERUP_AIM_SPEED_INCREASE: Model.FillColor = new Color(96, 255, 99, alpha); break;
                case Cannon.POWERUP_FREEZE_TIME: Model.FillColor = new Color(86, 198, 255, alpha); break;
                case Cannon.POWERUP_TRIPLE_CANNON: Model.FillColor = new Color(255, 135, 205, alpha); break;
                case Cannon.POWERUP_OCTUPLE_CANNON: Model.FillColor = new Color(172, 56, 255, alpha); break;
                case Cannon.POWERUP_RED_HOT_BEACH: Model.FillColor = new Color(255, 87, 10, alpha); break;
            }

            AddChild(Model);
        }

        public override void Activate(dynamic obj)
        {
            MessagePopup(Cannon.GetPowerupName(Type));

            if (obj is Cannon)
                obj.StartPowerup(Type);

            base.Activate((object)obj);
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (ScaleX + (AnimationScaleAcc * AnimationScaleDirection) > 1 || ScaleX + (AnimationScaleAcc * AnimationScaleDirection) < AnimationScaleMin)
                AnimationScaleDirection = AnimationScaleDirection == 1 ? -1 : 1;
            SetScale(ScaleX + (AnimationScaleAcc * AnimationScaleDirection));
        }
    }
}
