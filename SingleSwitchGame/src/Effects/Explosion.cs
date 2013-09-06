using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace SingleSwitchGame
{
    class Explosion : Entity
    {
        private List<CircleShape> ExplosionWaves;
        private const int WAVE_AMOUNT = 4;
        private const float GROW_SPEED = 0.04f;
        private float ExplosionRadius;

        public Explosion(Game Game, float ExplosionRadius)
            : base(Game, null)
        {
            this.ExplosionRadius = ExplosionRadius;
            ExplosionWaves = new List<CircleShape>();

            // Create Waves
            for (int i = 0; i < WAVE_AMOUNT; i++)
            {
                ExplosionWaves.Add(new CircleShape(ExplosionRadius, 40));
                ExplosionWaves[i].Origin = new Vector2f(ExplosionRadius, ExplosionRadius);
                ExplosionWaves[i].FillColor = new Color(0, 0, 0, 0);
                ExplosionWaves[i].OutlineThickness = 8;
                ExplosionWaves[i].Scale = new Vector2f(GROW_SPEED, GROW_SPEED);

                AddChild(ExplosionWaves[i]);
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            // Update Waves (Scale and Fade)
            for (int i = 0; i < ExplosionWaves.Count; i++)
            {
                if (ExplosionWaves[i].Scale.X >= 1)
                {
                    RemoveChild(ExplosionWaves[i]);
                    ExplosionWaves.RemoveAt(i);
                    if (i == ExplosionWaves.Count)
                        continue;
                }
                
                float scale = ExplosionWaves[i].Scale.X + (GROW_SPEED * (i + 1));
                ExplosionWaves[i].Scale = new Vector2f(scale, scale);
                ExplosionWaves[i].OutlineColor = new Color(255, 255, 255, (byte)(255 - Math.Min(Math.Ceiling(255 * scale), 255)) );
            }
        }
    }
}
