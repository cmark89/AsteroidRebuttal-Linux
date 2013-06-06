using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidRebuttal.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidRebuttal.Levels
{
    public class ScrollingBackgroundLayer
    {
        Color layerColor;
        Texture2D layerTexture;
        public float layerSpeed;

        GameScene thisScene;

        public int LayerID { get; set; }

        ScrollingBackground[] scrollingBackgroundImages;
        public float DrawLayer;

        #region Lerping Properties

        bool lerpingSpeed;
        double speedLerpElapsedTime;
        double speedLerpDuration;
        float startSpeed;
        float targetSpeed;

        bool lerpingColor;
        double colorLerpElapsedTime;
        double colorLerpDuration;
        Color startColor;
        Color targetColor;

        #endregion

        public ScrollingBackgroundLayer(GameScene newScene, Texture2D texture, float scrollSpeed, Color color)
        {
            layerColor = color;
            layerSpeed = scrollSpeed;
            layerTexture = texture;
            thisScene = newScene;

            Initialize();
        }

        public void Initialize()
        {
            scrollingBackgroundImages = new ScrollingBackground[2];

            for (int i = 0; i < 2; i++)
            {
                scrollingBackgroundImages[i] = new ScrollingBackground(layerTexture);

                scrollingBackgroundImages[i].position = new Vector2(0, thisScene.ScreenArea.Height - (scrollingBackgroundImages[i].texture.Height * i));
            }
        }


        public void Update(GameTime gameTime)
        {
            foreach (ScrollingBackground sb in scrollingBackgroundImages)
            {
                sb.position.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * layerSpeed;

                if (layerSpeed > 0 && sb.position.Y >= thisScene.ScreenArea.Height)
                {
                    sb.position = new Vector2(sb.position.X, (sb.position.Y - sb.texture.Height * 2));
                }
                else if (layerSpeed < 0 && sb.position.Y + sb.texture.Height <= 0)
                {
                    sb.position = new Vector2(sb.position.X, (sb.position.Y + sb.texture.Height * 2));
                }
            }

            if (lerpingColor || lerpingSpeed)
                LerpUpdate(gameTime);
        }

        public void LerpUpdate(GameTime gameTime)
        {
            if (lerpingSpeed)
            {
                speedLerpElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                layerSpeed = MathHelper.Lerp(startSpeed, targetSpeed, (float)speedLerpElapsedTime / (float)speedLerpDuration);


                if (speedLerpElapsedTime >= speedLerpDuration)
                    lerpingSpeed = false;
            }

            if (lerpingColor)
            {
                colorLerpElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                layerColor = Color.Lerp(startColor, targetColor, (float)colorLerpElapsedTime / (float)colorLerpDuration);

                if (colorLerpElapsedTime >= colorLerpDuration)
                    lerpingColor = false;
            }
        }

        public void LerpSpeed(float newSpeed, float duration)
        {
            lerpingSpeed = true;
            startSpeed = layerSpeed;
            targetSpeed = newSpeed;
            speedLerpDuration = duration;
            speedLerpElapsedTime = 0f;
        }

        public void LerpColor(Color newColor, float duration)
        {
            lerpingColor = true;
            startColor = layerColor;
            targetColor = newColor;
            colorLerpDuration = duration;
            colorLerpElapsedTime = 0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ScrollingBackground sb in scrollingBackgroundImages)
            {
                spriteBatch.Draw(sb.texture, new Vector2(sb.position.X, sb.position.Y), null, layerColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, DrawLayer);
            }
        }
    }
}
