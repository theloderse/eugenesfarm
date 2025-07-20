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

        public StarTrail(Texture2D[] starTextures)
        {
            this.starTextures = starTextures;
        }

        public void AddStar(Vector2 position)
        {
            int textureIndex = random.Next(starTextures.Length);
            float rotation = (float)(random.NextDouble() * MathHelper.TwoPi);

            stars.Add(new Star
            {
                Position = position,
                Scale = 0.7f + (float)random.NextDouble() * 0.6f,
                Alpha = 1f,
                TwinkleSpeed = 2f + (float)random.NextDouble() * 2f,
                TwinklePhase = (float)random.NextDouble() * MathHelper.TwoPi,
                Texture = starTextures[textureIndex],
                Rotation = rotation
            });
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = stars.Count - 1; i >= 0; i--)
            {
                var star = stars[i];
                star.Alpha = 0.5f + 0.5f * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * star.TwinkleSpeed + star.TwinklePhase);
                star.Scale -= elapsed * 0.2f;
                if (star.Scale <= 0.1f)
                    stars.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var star in stars)
            {
                spriteBatch.Draw(
                    star.Texture,
                    star.Position,
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


