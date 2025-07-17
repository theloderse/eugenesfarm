using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace EugenesFarm.Player
{
    public class PlayerMoveText
    {

        public string customTrailText = "Brr";
        private readonly IModHelper helper;
        private readonly List<TextParticle> particles = new();
        private int trailIndex = 0;
        private readonly string[] trailText = new[] { "B", "r", "r" };
        private int spawnCooldown = 0;
        private readonly Random rand = new Random();

        public PlayerMoveText(IModHelper helper)
        {
            this.helper = helper;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == SButton.F2)
            {
                Game1.activeClickableMenu = new TrailTextMenu(this);
            }
        }
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (TrailEnabled && Game1.player.isMoving() && spawnCooldown <= 0)
            {
                var position = Game1.player.Position;
                float angle = 0f;
                switch (Game1.player.FacingDirection)
                {
                    case 0: angle = MathHelper.PiOver2; break;
                    case 1: angle = MathHelper.Pi; break;
                    case 2: angle = -MathHelper.PiOver2; break;
                    case 3: angle = 0f; break;
                }
                angle += (float)(rand.NextDouble() - 0.5) * 0.5f;

                float distance = 36f + (float)(rand.NextDouble() * 12);
                var offset = new Vector2(
                    (float)Math.Cos(angle) * distance,
                    (float)Math.Sin(angle) * distance
                );

                particles.Add(new TextParticle(
                    customTrailText[trailIndex % customTrailText.Length].ToString(),
                    position + offset,
                    0f
                ));
                trailIndex++;

                if (particles.Count > 3)
                {
                    particles[0].MarkForGracefulRemoval();
                }

                spawnCooldown = 10;
            }
            else if (spawnCooldown > 0)
            {
                spawnCooldown--;
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update();
                if (particles[i].IsDead || (particles[i].GracefulRemove && particles[i].Alpha <= 0.01f))
                    particles.RemoveAt(i);
            }


        }

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            var spriteBatch = e.SpriteBatch;
            foreach (var particle in particles)
            {
                var screenPos = Game1.GlobalToLocal(particle.Position);
                var font = Game1.smallFont;
                var color = particle.GetRainbowColor();
                var scale = particle.Scale;
                var alpha = particle.Alpha;

                spriteBatch.DrawString(
                    font,
                    particle.Text,
                    screenPos + new Vector2(0, -particle.VerticalOffset),
                    color * alpha,
                    0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    1f
                );
            }
        }


        private class TextParticle
        {
            public string Text { get; }
            public Vector2 Position { get; }
            public float Age { get; private set; }
            public float Scale => 1f + Age * 0.8f;
            public float Alpha => MathHelper.Clamp(1f - Age / 3.5f, 0f, 1f);
            public float VerticalOffset => Age * 80f;
            public bool IsDead => Age > 3.5f;
            public bool GracefulRemove { get; private set; } = false;

            public TextParticle(string text, Vector2 position, float age)
            {
                Text = text;
                Position = position;
                Age = age;
            }

            public void Update()
            {
                Age += 0.015f;
            }

            public void MarkForGracefulRemoval()
            {
                GracefulRemove = true;
            }

            public Color GetRainbowColor()
            {
                float hue = (Age * 1.2f) % 1f;
                return HsvToRgb(hue, 1f, 1f);
            }

            private static Color HsvToRgb(float h, float s, float v)
            {
                int i = (int)(h * 6);
                float f = h * 6 - i;
                v = v * 255;
                float p = v * (1 - s);
                float q = v * (1 - f * s);
                float t = v * (1 - (1 - f) * s);
                switch (i % 6)
                {
                    case 0: return new Color((byte)v, (byte)t, (byte)p);
                    case 1: return new Color((byte)q, (byte)v, (byte)p);
                    case 2: return new Color((byte)p, (byte)v, (byte)t);
                    case 3: return new Color((byte)p, (byte)q, (byte)v);
                    case 4: return new Color((byte)t, (byte)p, (byte)v);
                    case 5: return new Color((byte)v, (byte)p, (byte)q);
                    default: return Color.White;
                }
            }
        }

        public void SetCustomTrailText(string text)
        {
            customTrailText = string.IsNullOrWhiteSpace(text) ? "Brrr" : text;
            trailIndex = 0;
        }

        public bool TrailEnabled { get; private set; } = true;

        public void ToggleTrail()
        {
            TrailEnabled = !TrailEnabled;
        }
    }
}