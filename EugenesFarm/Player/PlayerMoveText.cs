using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using EugenesFarm.Player; // Add this at the top if not present

namespace EugenesFarm.Player
{
    public class PlayerMoveText
    {

        public string customTrailText = "Brr";
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly List<TextParticle> particles = new();
        private readonly List<SpeechBubble> speechBubbles = new();
        private readonly RainbowTrail rainbowTrail = new();
        private int trailIndex = 0;
        private readonly string[] trailText = new[] { "B", "r", "r" };
        private int spawnCooldown = 0;
        private int speechCooldown = 0;
        private readonly Random rand = new Random();

        private StarTrail? starTrail;
        private Texture2D[] starTextures;

        public PlayerMoveText(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper;
            this.monitor = monitor;

            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            starTextures = new Texture2D[5];
            for (int i = 0; i < 5; i++)
            {
                string assetRelativePath = $"galaxyTrailAssets/star{i + 1}.png";
                string assetFullPath = System.IO.Path.Combine(helper.DirectoryPath, assetRelativePath);
                monitor.Log($"Attempting to load star texture {i + 1} from: {assetFullPath}", LogLevel.Info);

                try
                {
                    starTextures[i] = helper.ModContent.Load<Texture2D>(assetRelativePath);
                }
                catch (Exception ex)
                {
                    monitor.Log($"Failed to load star texture {i + 1}: {ex.Message}", LogLevel.Error);
                    starTextures[i] = null;
                }
            }
            if (Array.Exists(starTextures, t => t == null))
            {
                monitor.Log("One or more star textures failed to load. Star trail will be disabled.", LogLevel.Warn);
                starTrail = null;
            }
            else
            {
                starTrail = new StarTrail(starTextures);
            }
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
                if (CurrentTrailType == TrailType.Text)
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
                else if (CurrentTrailType == TrailType.Rainbow)
                {
                    rainbowTrail.Update();
                }
                else if (CurrentTrailType == TrailType.Star)
                {
                    // currently just draws on top of player, need to update like other trails
                    var worldPos = Game1.player.Position;
                    var screenPos = Game1.GlobalToLocal(worldPos);
                    starTrail.AddStar(screenPos);
                    spawnCooldown = 8;
                }
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

            if (speechCooldown <= 0 && IsPlayerNearNPC())
            {
                speechBubbles.Add(new SpeechBubble(
                    "Hey, I'm walking here!",
                    0f
                ));
                speechCooldown = 300;
            }
            else if (speechCooldown > 0)
            {
                speechCooldown--;
            }

            for (int i = speechBubbles.Count - 1; i >= 0; i--)
            {
                speechBubbles[i].Update();
                if (speechBubbles[i].IsDead)
                    speechBubbles.RemoveAt(i);
            }

            if (TrailEnabled && CurrentTrailType == TrailType.Rainbow)
            {
                rainbowTrail.Update();
            }
            if (TrailEnabled && CurrentTrailType == TrailType.Star && starTrail != null)
            {
                starTrail.Update(Game1.currentGameTime);
            }
        }

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            var spriteBatch = e.SpriteBatch;

            if (CurrentTrailType == TrailType.Text)
            {
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

            foreach (var bubble in speechBubbles)
            {
                var playerWorldPos = Game1.player.Position + new Vector2(0, -80);
                var screenPos = Game1.GlobalToLocal(playerWorldPos);

                var font = Game1.smallFont;
                var color = Color.Black;
                var scale = bubble.Scale * 0.7f;
                var alpha = bubble.Alpha;

                var textSize = font.MeasureString(bubble.Text) * scale;
                var padding = 8f;

                var bubbleWidth = textSize.X + padding * 2;
                var bubbleHeight = textSize.Y + padding * 2;
                var bubbleX = screenPos.X - bubbleWidth / 2;
                var bubbleY = screenPos.Y - bubbleHeight - bubble.VerticalOffset;

                var cloudColor = Color.White * alpha;
                var centerX = screenPos.X;
                var centerY = bubbleY + bubbleHeight / 2;

                var circleRadius = Math.Max(bubbleWidth, bubbleHeight) / 6;
                var circles = new[]
                {
                    new Vector2(centerX - bubbleWidth/4, centerY),
                    new Vector2(centerX + bubbleWidth/4, centerY),
                    new Vector2(centerX, centerY - bubbleHeight/6),
                    new Vector2(centerX, centerY + bubbleHeight/6),
                    new Vector2(centerX, centerY), // Center (largest)
                };

                for (int i = 0; i < circles.Length; i++)
                {
                    var circle = circles[i];
                    var radius = (i == 4) ? circleRadius * 1.1f : circleRadius * (0.8f + (i % 2) * 0.1f); // Smaller variations

                    for (int x = (int)(circle.X - radius); x <= circle.X + radius; x++)
                    {
                        for (int y = (int)(circle.Y - radius); y <= circle.Y + radius; y++)
                        {
                            float distance = Vector2.Distance(new Vector2(x, y), circle);
                            if (distance <= radius)
                            {
                                var pixelRect = new Rectangle(x, y, 1, 1);
                                spriteBatch.Draw(Game1.staminaRect, pixelRect, cloudColor);
                            }
                        }
                    }
                }

                var tailY = centerY + bubbleHeight / 2;
                var tailSteps = 2;
                for (int i = 0; i < tailSteps; i++)
                {
                    var progress = (float)i / (tailSteps - 1);
                    var tailX = centerX - 4 + progress * 8;
                    var tailYPos = tailY + progress * 12;
                    var tailRadius = (tailSteps - i) * 3;

                    for (int x = (int)(tailX - tailRadius); x <= tailX + tailRadius; x++)
                    {
                        for (int y = (int)(tailYPos - tailRadius); y <= tailYPos + tailRadius; y++)
                        {
                            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(tailX, tailYPos));
                            if (distance <= tailRadius)
                            {
                                var pixelRect = new Rectangle(x, y, 1, 1);
                                spriteBatch.Draw(Game1.staminaRect, pixelRect, cloudColor);
                            }
                        }
                    }
                }

                var textPos = new Vector2(
                    centerX - textSize.X / 2,
                    centerY - textSize.Y / 2
                );

                spriteBatch.DrawString(
                    font,
                    bubble.Text,
                    textPos,
                    color * alpha,
                    0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    1f
                );
            }

            if (TrailEnabled && CurrentTrailType == TrailType.Rainbow)
            {
                rainbowTrail.Render(spriteBatch);
            }
            if (TrailEnabled && CurrentTrailType == TrailType.Star && starTrail != null)
            {
                starTrail.Draw(spriteBatch);
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
        public enum TrailType { Text, Rainbow, Star }
        public TrailType CurrentTrailType { get; set; } = TrailType.Text;

        public void ToggleTrail()
        {
            TrailEnabled = !TrailEnabled;
        }

        private bool IsPlayerNearNPC()
        {
            var player = Game1.player;
            var playerPosition = player.Position;
            var currentLocation = Game1.currentLocation;

            foreach (var npc in currentLocation.characters)
            {
                if (npc != null)
                {
                    var distance = Vector2.Distance(playerPosition, npc.Position);
                    if (distance < 128f) // Within 2 tiles
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private class SpeechBubble
        {
            public string Text { get; }
            public float Age { get; private set; }
            public float Scale => MathHelper.Clamp(1f + (float)Math.Sin(Age * 3f) * 0.1f, 0.9f, 1.1f);
            public float Alpha => MathHelper.Clamp(1f - Age / 3f, 0f, 1f);
            public float VerticalOffset => (float)Math.Sin(Age * 2f) * 5f;
            public bool IsDead => Age > 3f;
            public SpeechBubble(string text, float age)
            {
                Text = text;
                Age = age;
            }

            public void Update()
            {
                Age += 0.025f;
            }
        }
    }
}