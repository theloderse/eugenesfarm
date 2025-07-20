using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;

namespace EugenesFarm.Player
{
    public class RainbowTrail
    {
        private readonly List<RainbowTrailParticle> particles = new();
        private readonly Random rand = new Random();
        private int spawnCooldown = 0;

        public void Update()
        {
            if (!Game1.player.isMoving())
            {
                if (spawnCooldown > 0)
                    spawnCooldown--;
                
                for (int i = particles.Count - 1; i >= 0; i--)
                {
                    particles[i].Update();
                    if (particles[i].IsDead)
                        particles.RemoveAt(i);
                }
                return;
            }

            if (spawnCooldown <= 0)
            {
                var playerPos = Game1.player.Position;
                
                for (int i = 0; i < 1; i++)
                {
                    float trailDistance = 25f;
                    float angle = 0f;
                    
                    switch (Game1.player.FacingDirection)
                    {
                        case 0: angle = MathHelper.PiOver2; break;
                        case 1: angle = MathHelper.Pi; break;
                        case 2: angle = -MathHelper.PiOver2; break; 
                        case 3: angle = 0f; break;
                    }
                    
                    var trailOffset = new Vector2(
                        (float)Math.Cos(angle) * trailDistance,
                        (float)Math.Sin(angle) * trailDistance
                    );
                    
                    var randomOffset = new Vector2(
                        (float)(rand.NextDouble() - 0.5) * 3f,
                        (float)(rand.NextDouble() - 0.5) * 3f
                    );
                    
                    particles.Add(new RainbowTrailParticle(
                        playerPos + trailOffset + randomOffset,
                        particles.Count * 0.03f
                    ));
                }
                
                if (particles.Count > 30)
                {
                    if (particles.Count > 0)
                        particles.RemoveAt(0);
                }
                
                spawnCooldown = 3;
            }
            else
            {
                spawnCooldown--;
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update();
                if (particles[i].IsDead)
                    particles.RemoveAt(i);
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (particles.Count > 5)
            {
                for (int i = 0; i < particles.Count - 1; i++)
                {
                    var current = particles[i];
                    var next = particles[i + 1];
                    
                    var currentScreenPos = Game1.GlobalToLocal(current.Position);
                    var nextScreenPos = Game1.GlobalToLocal(next.Position);
                    
                    var currentWaveX = (float)Math.Sin(current.Age * 1.5f + current.HueOffset * 3f) * 3f;
                    var currentWaveY = (float)Math.Cos(current.Age * 1f + current.HueOffset * 2f) * 2f;
                    var nextWaveX = (float)Math.Sin(next.Age * 1.5f + next.HueOffset * 3f) * 3f;
                    var nextWaveY = (float)Math.Cos(next.Age * 1f + next.HueOffset * 2f) * 2f;
                    
                    var currentFinalPos = currentScreenPos + new Vector2(currentWaveX, currentWaveY - current.VerticalOffset);
                    var nextFinalPos = nextScreenPos + new Vector2(nextWaveX, nextWaveY - next.VerticalOffset);
                    
                    float arcProgress = (float)i / (particles.Count - 1);
                    float arcHeight = 25f;
                    float arcOffset = (float)Math.Sin(arcProgress * Math.PI) * arcHeight;

                    var smoothingFactor = 0.5f;
                    currentFinalPos.Y -= arcOffset * smoothingFactor;
                    nextFinalPos.Y -= arcOffset * smoothingFactor;
                    
                    var rainbowColors = new Color[]
                    {
                        new Color(255, 0, 0),
                        new Color(255, 127, 0),
                        new Color(255, 255, 0),
                        new Color(0, 255, 0),
                        new Color(0, 0, 255),
                        new Color(75, 0, 130),
                        new Color(148, 0, 211)
                    };
                    
                    for (int band = 0; band < rainbowColors.Length; band++)
                    {
                        float bandOffset = band * 4f;
                        var alpha = Math.Min(current.Alpha, next.Alpha) * 0.08f;
                        
                        for (int gradientStep = 0; gradientStep < 3; gradientStep++)
                        {
                            float gradientAlpha = alpha * (1f - gradientStep * 0.4f);
                            float gradientOffset = gradientStep * 2f;
                            
                            DrawRainbowLine(spriteBatch, 
                                currentFinalPos + new Vector2(0, bandOffset + gradientOffset), 
                                nextFinalPos + new Vector2(0, bandOffset + gradientOffset), 
                                rainbowColors[band] * gradientAlpha, 
                                6f - gradientStep);
                        }
                    }
                }
            }
            else
            {
                foreach (var rainbow in particles)
                {
                    var screenPos = Game1.GlobalToLocal(rainbow.Position);
                    var color = rainbow.GetRainbowColor();
                    var alpha = rainbow.Alpha;
                    var scale = rainbow.Scale;

                    var waveX = (float)Math.Sin(rainbow.Age * 1.5f + rainbow.HueOffset * 3f) * 3f;
                    var waveY = (float)Math.Cos(rainbow.Age * 1f + rainbow.HueOffset * 2f) * 2f;
                    var finalPos = screenPos + new Vector2(waveX, waveY - rainbow.VerticalOffset);

                    var glowSize = scale * 10f;
                    var glowRect = new Rectangle(
                        (int)(finalPos.X - glowSize / 2),
                        (int)(finalPos.Y - glowSize / 2),
                        (int)glowSize,
                        (int)glowSize
                    );
                    
                    spriteBatch.Draw(
                        Game1.staminaRect,
                        glowRect,
                        color * (alpha * 0.1f)
                    );
                }
            }
        }

        private void DrawRainbowLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 direction = end - start;
            float distance = direction.Length();
            
            if (distance > 0)
            {
                direction.Normalize();
                
                int segments = (int)Math.Max(2, distance / 0.5f);
                for (int i = 0; i <= segments; i++)
                {
                    float progress = (float)i / segments;
                    Vector2 position = Vector2.Lerp(start, end, progress);
                    
                    for (int layer = 0; layer < 3; layer++)
                    {
                        var circleSize = thickness * (1.8f - layer * 0.3f);
                        var layerAlpha = (1f - layer * 0.4f) * 0.15f;
                        var rect = new Rectangle(
                            (int)(position.X - circleSize / 2),
                            (int)(position.Y - circleSize / 2),
                            (int)circleSize,
                            (int)circleSize
                        );
                        
                        spriteBatch.Draw(Game1.staminaRect, rect, color * layerAlpha);
                    }
                }
            }
        }

        private class RainbowTrailParticle
        {
            public Vector2 Position { get; }
            public float Age { get; private set; }
            public float HueOffset { get; }
            public float Scale => MathHelper.Clamp(1.8f - Age * 0.3f, 0.5f, 1.8f);
            public float Alpha => MathHelper.Clamp(1f - Age / 3f, 0f, 1f);
            public float VerticalOffset => Age * 15f;
            public bool IsDead => Age > 3f;

            public RainbowTrailParticle(Vector2 position, float hueOffset)
            {
                Position = position;
                HueOffset = hueOffset;
                Age = 0f;
            }

            public void Update()
            {
                Age += 0.015f;
            }

            public Color GetRainbowColor()
            {
                float hue = (Age * 1f + HueOffset + (float)Math.Sin(Age * 2f) * 0.1f) % 1f;
                float saturation = 1f;
                float brightness = 0.9f + (float)Math.Sin(Age * 3f) * 0.1f;
                return HsvToRgb(hue, saturation, brightness);
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
    }
}
