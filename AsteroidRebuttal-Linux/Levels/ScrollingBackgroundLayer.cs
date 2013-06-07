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

		// This now uses a Vector2 because there's no reason to store the textures in a second location
        Vector2[] scrollingBackgroundImages;
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

		float XOffset = 0;

        #endregion

        public ScrollingBackgroundLayer(GameScene newScene, Texture2D texture, float scrollSpeed, Color color, float xoff = 0)
        {
            layerColor = color;
            layerSpeed = scrollSpeed;
            layerTexture = texture;
            thisScene = newScene;
			XOffset = xoff;

            Initialize();
        }

        public void Initialize()
        {
            scrollingBackgroundImages = new Vector2[2];

            for (int i = 0; i < 2; i++)
            {
                scrollingBackgroundImages[i] = new Vector2(XOffset, thisScene.ScreenArea.Height - (layerTexture.Height * i));
            }
        }


        public void Update(GameTime gameTime)
        {
			for (int i = 0; i < scrollingBackgroundImages.Length; i++)
            {
                scrollingBackgroundImages[i].Y += (float)gameTime.ElapsedGameTime.TotalSeconds * layerSpeed;

                if (layerSpeed > 0 && scrollingBackgroundImages[i].Y >= thisScene.ScreenArea.Height)
                {
                    scrollingBackgroundImages[i] = new Vector2(scrollingBackgroundImages[i].X, (scrollingBackgroundImages[i].Y - layerTexture.Height * 2));
                }
                else if (layerSpeed < 0 && scrollingBackgroundImages[i].Y + layerTexture.Height <= 0)
                {
                    scrollingBackgroundImages[i] = new Vector2(scrollingBackgroundImages[i].X, (scrollingBackgroundImages[i].Y + layerTexture.Height * 2));
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
            foreach (Vector2 sb in scrollingBackgroundImages)
            {
				spriteBatch.Draw(layerTexture, sb, null, layerColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, DrawLayer);

				//spriteBatch.Draw(GameObjects.PlayerShip.hitboxTexture, sb, null, layerColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, DrawLayer);
				//spriteBatch.Draw(layerTexture, sb, null, layerColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, DrawLayer);
            }
        }
    }
}
