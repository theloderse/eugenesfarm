using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;

namespace EugenesFarm.Player
{
    public class StarTrail
    {
        private class Star
        {
            public Vector2 Position;
            public float Scale;
            public float Alpha;
            public float TwinkleSpeed;
            public float TwinklePhase;
            public Texture2D Texture;
            public float Rotation;
        }

        private readonly List<Star> stars = new List<Star>();
        private readonly Texture2D[] starTextures;
        private readonly Random random = new Random();
        private int spawnCooldown = 0;

        public StarTrail(Texture2D[] starTextures)
        {
            this.starTextures = starTextures;
        }

        public void Update(GameTime gameTime, bool isPlayerMoving, Vector2 playerPosition, int facingDirection)
        {
            if (isPlayerMoving && spawnCooldown <= 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    float trailDistance = 40f + (float)(random.NextDouble() * 25);
                    float angle = 0f;
                    
                    switch (facingDirection)
                    {
                        case 0: angle = MathHelper.PiOver2; break;
                        case 1: angle = MathHelper.Pi; break;
                        case 2: angle = -MathHelper.PiOver2; break;
                        case 3: angle = 0f; break;
                    }
                    
                    angle += (float)(random.NextDouble() - 0.5) * 0.8f;
                    
                    var trailOffset = new Vector2(
                        (float)Math.Cos(angle) * trailDistance,
                        (float)Math.Sin(angle) * trailDistance
                    );
                    
                    var randomOffset = new Vector2(
                        (float)(random.NextDouble() - 0.5) * 15f,
                        (float)(random.NextDouble() - 0.5) * 15f
                    );
                    
                    var starWorldPos = playerPosition + trailOffset + randomOffset;
                    AddStar(starWorldPos);
                }
                spawnCooldown = 6;
            }
            else if (spawnCooldown > 0)
            {
                spawnCooldown--;
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = stars.Count - 1; i >= 0; i--)
            {
                var star = stars[i];
                
                float twinkle = 0.5f + 0.5f * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * star.TwinkleSpeed + star.TwinklePhase);
                float fadeOut = Math.Max(0f, star.Scale / 2.2f);
                star.Alpha = twinkle * fadeOut;
                
                star.Scale -= elapsed * 0.3f;
                star.Rotation += elapsed * 0.5f;
                
                if (star.Scale <= 0.1f || star.Alpha <= 0.05f)
                    stars.RemoveAt(i);
            }
        }

        public void AddStar(Vector2 worldPosition)
        {
            int textureIndex = random.Next(starTextures.Length);
            float rotation = (float)(random.NextDouble() * MathHelper.TwoPi);

            stars.Add(new Star
            {
                Position = worldPosition,
                Scale = 2.5f + (float)random.NextDouble() * 2.0f,
                Alpha = 1f,
                TwinkleSpeed = 1.5f + (float)random.NextDouble() * 1.5f,
                TwinklePhase = (float)random.NextDouble() * MathHelper.TwoPi,
                Texture = starTextures[textureIndex],
                Rotation = rotation
            });
            
            if (stars.Count > 30)
            {
                stars.RemoveAt(0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var star in stars)
            {
                var screenPos = Game1.GlobalToLocal(star.Position);
                
                spriteBatch.Draw(
                    star.Texture,
                    screenPos,
                    null,
                    Color.White * star.Alpha,
                    star.Rotation,
                    new Vector2(star.Texture.Width / 2, star.Texture.Height / 2),
                    star.Scale,
                    SpriteEffects.None,
                    1f
                );
            }
        }
    }
}


