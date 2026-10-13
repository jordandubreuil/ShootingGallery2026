using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace ShootingGallery
{
    internal class Target
    {
        public Texture2D Texture { get; }
        public Vector2 Position { get; set; }
        public int Radius { get; }
        public float Scale { get; set; }
        float GrowthRate = 4.0f;


        public Target(Texture2D texture, Vector2 position, int radius) 
        {
            Texture = texture;
            Position = position;
            Radius = radius;
        }

        public bool IsHit(Vector2 point)
        {
            Vector2 center = Position;

            

            return Vector2.Distance(center, point) <Radius;
        }

        public void Update(GameTime gameTime)
        {
            if (Scale < 1.0f)
            {
                Scale += GrowthRate * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Scale > 1.0f) Scale = 1.0f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(Radius, Radius);
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f,origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
