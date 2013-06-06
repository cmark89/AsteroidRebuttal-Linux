using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidRebuttal.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Enemies;
using AsteroidRebuttal.Enemies.Bosses;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Levels;
using Microsoft.Xna.Framework.Input;

namespace AsteroidRebuttal.Scenes
{
    public class GameOverScene : Scene
    {
        public static Texture2D GameOverTexture;

        AsteroidRebuttal Game;

        Color currentColor = Color.Black;
        int fadephase = 0;
        float time = 0;

        public GameOverScene(AsteroidRebuttal thisGame)
        {
            Game = thisGame;
        }

        public override void Initialize()
        {
            // Initialization logic here.
        }

        public override void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (KeyboardManager.KeyPressedUp(Keys.Space) || GamepadManager.ProceedButtonDown())
            {
                if(fadephase < 2)
                    time = 8f;
            }

            switch (fadephase)
            {
                case(0):
                    currentColor = Color.Lerp(Color.Black, Color.White, time/3f);
                    if(currentColor == Color.White) { fadephase = 1; }
                    break;
                case(1):
                    if(time > 8f) { fadephase = 2; }
                    break;
                case(2):
                    currentColor = Color.Lerp(Color.White, Color.Black, (time - 8f)/3f);
                    if(currentColor == Color.Black) { FadeComplete(); }
                    break;
                default:
                    break;
            }
        }

        private void FadeComplete()
        {
            Game.ChangeScene(new TitleScene(Game));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameOverTexture, Vector2.Zero, currentColor);
        }

        public override void Unload()
        {
            // Nothing!
        }

        public override void LoadContent(ContentManager content)
        {
            if (GameOverTexture == null)
                GameOverTexture = content.Load<Texture2D>("Graphics/GUI/GameOver");
        }
    }
}
