using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;

namespace EugenesFarm.Player
{
    public class SpeechBubbleManager
    {
        private readonly List<SpeechBubbleParticle> speechBubbles = new();
        private int speechCooldown = 0;

        public void Update(bool isPlayerNearNPC)
        {
            if (speechCooldown <= 0 && isPlayerNearNPC)
            {
                speechBubbles.Add(new SpeechBubbleParticle(
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
        }

        public void Render(SpriteBatch spriteBatch)
        {
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

                var bubbleRect = new Rectangle(
                    (int)(centerX - bubbleWidth / 2),
                    (int)(centerY - bubbleHeight / 2),
                    (int)bubbleWidth,
                    (int)bubbleHeight
                );

                spriteBatch.Draw(Game1.staminaRect, bubbleRect, cloudColor);

                var cornerRadius = Math.Min(bubbleWidth, bubbleHeight) * 0.15f;
                var corners = new[]
                {
                    new Vector2(bubbleRect.Left, bubbleRect.Top),
                    new Vector2(bubbleRect.Right, bubbleRect.Top),
                    new Vector2(bubbleRect.Left, bubbleRect.Bottom),
                    new Vector2(bubbleRect.Right, bubbleRect.Bottom)
                };

                foreach (var corner in corners)
                {
                    for (int x = (int)(corner.X - cornerRadius); x <= corner.X + cornerRadius; x++)
                    {
                        for (int y = (int)(corner.Y - cornerRadius); y <= corner.Y + cornerRadius; y++)
                        {
                            float distance = Vector2.Distance(new Vector2(x, y), corner);
                            if (distance <= cornerRadius)
                            {
                                var pixelRect = new Rectangle(x, y, 1, 1);
                                spriteBatch.Draw(Game1.staminaRect, pixelRect, cloudColor);
                            }
                        }
                    }
                }

                var tailBaseY = bubbleRect.Bottom;
                var tailTipY = tailBaseY + 15;
                var tailWidth = 8f;

                for (int y = (int)tailBaseY; y <= tailTipY; y++)
                {
                    float progress = (y - tailBaseY) / (tailTipY - tailBaseY);
                    float currentWidth = tailWidth * (1f - progress);
                    
                    for (int x = (int)(centerX - currentWidth); x <= centerX + currentWidth; x++)
                    {
                        var pixelRect = new Rectangle(x, y, 1, 1);
                        spriteBatch.Draw(Game1.staminaRect, pixelRect, cloudColor);
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
        }
    }

    internal class SpeechBubbleParticle
    {
        public string Text { get; }
        public float Age { get; private set; }
        public float Scale => 1f;
        public float Alpha 
        { 
            get 
            {
                if (Age < 2.5f)
                    return 1f;
                else
                    return MathHelper.Clamp((3f - Age) / 0.5f, 0f, 1f);
            }
        }
        public float VerticalOffset => 0f;
        public bool IsDead => Age > 3f;

        public SpeechBubbleParticle(string text, float age)
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
