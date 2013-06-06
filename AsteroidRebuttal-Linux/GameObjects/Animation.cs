using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidRebuttal.GameObjects
{
    public class Animation
    {
        private Texture2D texture;
        private int columns;
        private int rows;
        private int cellWidth;

        private List<int> frames;
        private int currentFrame = 0;
        private bool looping;
        private float layerDepth;
        private Vector2 position;
        private float frameTime;

        private float thisFrameTime = 0;

        public bool FlaggedForRemoval = false;

        public Animation(Texture2D animTexture, int[] animFrames, int animResolution, float frameRate, Vector2 pos, bool loop, float layer = 0)
        {
            texture = animTexture;
            looping = loop;
            layerDepth = layer;

            frames = new List<int>();
            foreach (int i in animFrames)
                frames.Add(i);

            columns = texture.Width / animResolution;
            rows = texture.Height / animResolution;
            cellWidth = animResolution;
            position = pos;
            frameTime = 1f / frameRate;
        }

        public void Update(GameTime gameTime)
        {
            thisFrameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (thisFrameTime >= frameTime)
            {
                NextFrame();
                thisFrameTime = 0;
            }
        }

        public void NextFrame()
        {
            currentFrame++;
            if (currentFrame >= frames.Count)
            {
                if (looping)
                    currentFrame = 0;
                else
                    FlaggedForRemoval = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, cellWidth, cellWidth), GetSourceRectangle(currentFrame), Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        }


        public Rectangle GetSourceRectangle(int index)
        {
            int x, y;

            x = (index % columns) * cellWidth;
            y = (int)(index / columns) * cellWidth;

            return new Rectangle(x, y, cellWidth, cellWidth);
        }
    }
}
