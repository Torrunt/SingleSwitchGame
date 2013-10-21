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
            Model = PowerupPickup.GetModel(type, Game.GraphicsMode);
            AddChild(Model);
        }
        public static CircleShape GetModel(int type, uint graphicsMode, byte alpha = 180, byte alpha_outline = 200)
        {
            CircleShape model = new CircleShape(15);
            model.Origin = new Vector2f(model.Radius, model.Radius);
            if (graphicsMode == Game.GRAPHICSMODE_NORMAL)
            {

                model.OutlineColor = new Color(0, 0, 0, alpha_outline);
                model.OutlineThickness = 6;
            }
            else
            {
                model.OutlineColor = new Color(255, 255, alpha_outline);
                model.OutlineThickness = 3;
            }

            switch (type)
            {
                case Powerup.DOUBLE_EXPLOSION_RADIUS: model.FillColor = new Color(255, 48, 48, alpha); break;
                case Powerup.AIM_SPEED_INCREASE: model.FillColor = new Color(96, 255, 99, alpha); break;
                case Powerup.FREEZE_TIME: model.FillColor = new Color(86, 198, 255, alpha); break;
                case Powerup.TRIPLE_CANNON: model.FillColor = new Color(255, 135, 205, alpha); break;
                case Powerup.OCTUPLE_CANNON: model.FillColor = new Color(172, 56, 255, alpha); break;
                case Powerup.RED_HOT_BEACH: model.FillColor = new Color(255, 87, 10, alpha); break;
            }

            return model;
        }

        public override void Activate(dynamic obj)
        {
            MessagePopup(Powerup.GetPowerupName(Type));

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
