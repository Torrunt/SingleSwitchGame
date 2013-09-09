using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using System.Timers;

namespace SingleSwitchGame
{
    class CircleWaves : Entity
    {
        private List<CircleShape> Waves;
        private Timer WaveTimer;
        private const int WAVE_FREQUENCY_MIN = 250;
        private const int WAVE_FREQUENCY_MAX = 5000;
        private const float SPEED = 0.0025f;
        private const float FADE_MIN = 130;
        private float Radius;
        private float ScaleMax = 2;
        private float ScaleOverlap;
        private float ScaleOverlapReverse;
        private List<bool> Reverse;

        private uint PointCount;
        private float Thickness;

        /// <summary>
        /// Dynamic animation of fading circle waves. Used for having waves approach a circle island and fade out on the shore.
        /// </summary>
        /// <param name="Game"></param>
        /// <param name="Radius">End Radius (ie: the radius of an Island that waves are going into).</param>
        /// <param name="ScaleOverlap">How much waves overlap into the Radius. Between 0-1.</param>
        /// <param name="ScaleMax">The starting scale of incoming waves.</param>
        /// <param name="Thickness"></param>
        /// <param name="PointCount"></param>
        public CircleWaves(Game Game, float Radius, float ScaleOverlap, float ScaleMax, uint Thickness = 4, uint PointCount = 50)
            : base(Game, null)
        {
            this.Radius = Radius;
            this.ScaleOverlap = ScaleOverlap;
            this.ScaleMax = ScaleMax;
            this.PointCount = PointCount;
            this.Thickness = Thickness;

            this.ScaleOverlapReverse = ScaleOverlap / 2;

            Waves = new List<CircleShape>();
            Reverse = new List<bool>();
            WaveTimer = new Timer(Utils.RandomInt(WAVE_FREQUENCY_MIN, WAVE_FREQUENCY_MAX));
            WaveTimer.Elapsed += new ElapsedEventHandler(WaveTimerHandler);
            WaveTimer.Start();

            AddWave();
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            // Update Waves (Scale and Fade)
            for (int i = 0; i < Waves.Count; i++)
            {
                if (!Reverse[i])
                {
                    if (Waves[i].Scale.X <= 1 - ScaleOverlap)
                        Reverse[i] = true;
                }
                else if (Waves[i].Scale.X >= 1)
                {
                    RemoveChild(Waves[i]);
                    Waves.RemoveAt(i);
                    Reverse.RemoveAt(i);
                    if (i == Waves.Count)
                        continue;
                }

                float scale;
                if (Waves[i].Scale.X <= 1)
                    scale = Waves[i].Scale.X - (SPEED * (Reverse[i] ? -0.4f : 0.6f));
                else
                    scale = Waves[i].Scale.X - SPEED;
                Waves[i].Scale = new Vector2f(scale, scale);
                if (Reverse[i])
                    Waves[i].OutlineColor = new Color(255, 255, 255, (byte)((FADE_MIN / 2) - Math.Min((FADE_MIN / 2) * Math.Abs((scale - (1f - ScaleOverlap)) / ScaleOverlap), (FADE_MIN / 2)))); // Fade out going back out
                else if (Waves[i].Scale.X <= 1)
                    Waves[i].OutlineColor = new Color(255, 255, 255, (byte)(FADE_MIN - Math.Min(Math.Ceiling((FADE_MIN/2) * Math.Abs((scale-1) / ScaleOverlap)), FADE_MIN))); // Fade out a bit going in
                else
                    Waves[i].OutlineColor = new Color(255, 255, 255, (byte)(FADE_MIN - Math.Min(Math.Ceiling(FADE_MIN * ((scale - 1) / (ScaleMax - 1))), FADE_MIN))); // Fade in approaching beach
            }
        }

        private void AddWave()
        {
            CircleShape wave = new CircleShape(Radius, PointCount);
            Waves.Add(wave);
            Reverse.Add(false);
            wave.Origin = new Vector2f(Radius, Radius);
            wave.FillColor = new Color(0, 0, 0, 0);
            wave.OutlineColor = new Color(0, 0, 0, 0);
            wave.OutlineThickness = Thickness;
            wave.Scale = new Vector2f(ScaleMax, ScaleMax);

            AddChild(wave);
        }

        private void WaveTimerHandler(Object source, ElapsedEventArgs e)
        {
            WaveTimer.Interval = Utils.RandomInt(WAVE_FREQUENCY_MIN, WAVE_FREQUENCY_MAX);

            AddWave();
        }
    }
}
