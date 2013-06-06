using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Core;

namespace AsteroidRebuttal.GameObjects
{
    public class Fader : GameObject
    {
        #region Bullet Graphics
        public static Texture2D FadeTexture;

        #endregion

        public Fader(GameScene newScene)
        {
            Color = Color.Black;
            thisScene = newScene;
            Phasing = true;
            Hitbox = new Circle(Center, 0f);
            Initialize();
        }

        public override void Initialize()
        {
            DrawLayer = 0;
            Center = Position;
            Texture = FadeTexture;

            base.Initialize();
        }

        public static void LoadContent(ContentManager content)
        {
            if (FadeTexture == null)
            {
                FadeTexture = content.Load<Texture2D>("Graphics/GUI/fader");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(thisScene.ScreenArea.X, thisScene.ScreenArea.Y, thisScene.ScreenArea.Width, thisScene.ScreenArea.Height), Color);
        }
    }
}
