using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using AsteroidRebuttal;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Enemies;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Levels;
using AsteroidRebuttal.Controls;



namespace AsteroidRebuttal
{
    public class AsteroidRebuttal : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // The current scene of the game.
        public Scene CurrentScene { get; private set; }

        public static bool ImmortalMode = false;
        public static bool ManicMode = false;
        public static bool HardcoreMode = false;

        public AsteroidRebuttal()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferHeight = 650;
            graphics.PreferredBackBufferWidth = 925;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

       
        protected override void Initialize()
        {
            base.Initialize();
            ChangeScene(new LogoSplashScene(this));
            this.Window.Title = "Asteroid Rebuttal";
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            PlayerShip.LoadContent(Content);
            Bullet.LoadContent(Content);
            BulletEmitter.LoadContent(Content);
            QuadTree.LoadContent(Content);
            Enemy.LoadContent(Content);
            Boss.LoadContent(Content);
            LevelManager.LoadContent(Content);
            Fader.LoadContent(Content);
        }


        protected override void UnloadContent()
        {
            //
        }

        
        protected override void Update(GameTime gameTime)
        {
            KeyboardManager.Update(gameTime);
			GamepadManager.Update(gameTime);

            if (CurrentScene != null)
                CurrentScene.Update(gameTime);

            base.Update(gameTime);
        }

        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (CurrentScene != null)
                CurrentScene.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ChangeScene(Scene newScene)
        {
            if(CurrentScene != null)
                CurrentScene.Unload();

            CurrentScene = newScene;
            CurrentScene.LoadContent(Content);
            CurrentScene.Initialize();
        }
    }
}
