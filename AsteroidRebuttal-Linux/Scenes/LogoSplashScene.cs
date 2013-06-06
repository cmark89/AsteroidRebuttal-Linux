using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Controls;
using AsteroidRebuttal.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;


namespace AsteroidRebuttal.Scenes
{
    public class LogoSplashScene : Scene
    {
        Rectangle ScreenArea;
        Rectangle DrawArea;
        Texture2D splashTexture;
        Color splashColor;
        float titleTime;
        AsteroidRebuttal Game;

        Scripting.ScriptManager scriptManager;

        public LogoSplashScene(AsteroidRebuttal game)
        {
            Game = game;
        }

        public override void Initialize()
        {
            ScreenArea = new Rectangle(Game.GraphicsDevice.Viewport.X, Game.GraphicsDevice.Viewport.Y, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            DrawArea = new Rectangle(
                ScreenArea.X + ((int)((ScreenArea.Width - 800) / 2f)),
                ScreenArea.Y + ((int)((ScreenArea.Height - 600) / 2f)),
                800,
                600
                );
            splashColor = Color.Black;
            scriptManager = new Scripting.ScriptManager();
            scriptManager.Execute(SplashAnimation);
        }

        public override void LoadContent(ContentManager content)
        {
            if (splashTexture == null)
                splashTexture = content.Load<Texture2D>("Graphics/ObjectivelyRadicalLogo");
        }

        public override void Update(GameTime gameTime)
        {
            titleTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            scriptManager.Update(gameTime);

            // Check for input to skip the screen
            if (KeyboardManager.KeyPressedUp(Keys.Space) || KeyboardManager.KeyPressedUp(Keys.Enter) || GamepadManager.ProceedButtonDown())
            {
                Game.ChangeScene(new TitleScene(Game));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(splashTexture, DrawArea, splashColor);
        }


        public IEnumerator<float> SplashAnimation()
        {
            yield return 1f;

            titleTime = 0f;
            while (splashColor != Color.White)
            {
                splashColor = Color.Lerp(Color.Black, Color.White, (titleTime) / 2f);
                yield return 0f;
            }

            yield return 3f;

            titleTime = 0f;
            while (splashColor != Color.Black)
            {
                splashColor = Color.Lerp(Color.White, Color.Black, titleTime / 2f);
                yield return 0f;
            }

            yield return 1.6f;
            Game.ChangeScene(new TitleScene(Game));
        }

        public override void Unload()
        {
        }
    }
}
