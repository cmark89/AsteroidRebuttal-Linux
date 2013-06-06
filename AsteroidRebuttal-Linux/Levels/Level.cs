using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using AsteroidRebuttal.Enemies;
using AsteroidRebuttal.Scripting;

namespace AsteroidRebuttal.Levels
{
    public class Level
    {
        protected List<ScrollingBackgroundLayer> scrollingBackground;
        protected LevelManager manager;
        protected ScriptManager scriptManager;
        public SoundEffectInstance bossTheme;

        protected Boss levelBoss;

        public bool TitleShown { get; protected set; }

        public Texture2D TitleTexture { get; protected set;}

        // Content
        public static SoundEffect BossTheme;

        public Level(LevelManager thisManager)
        {
            manager = thisManager;
            scriptManager = manager.thisScene.scriptManager;
            Initialize();
        }

        public virtual void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
            foreach (ScrollingBackgroundLayer sbl in scrollingBackground)
            {
                sbl.Update(gameTime);
            }

            OnUpdate(gameTime);
        }

        public virtual void OnUpdate(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ScrollingBackgroundLayer sbl in scrollingBackground)
            {
                sbl.Draw(spriteBatch);
            }
        }

        public virtual IEnumerator<float> LevelScript()
        {
            // This is the script for the level; controls enemy spawning and events.
            yield return 0f;
        }

        // Sets up the background to begin scrolling.
        public virtual void SetupBackground()
        {
            scrollingBackground = new List<ScrollingBackgroundLayer>();
        }

        public Enemy SpawnEnemy(EnemyType newType, Vector2 position)
        {
            Enemy newEnemy = null;
            switch (newType)
            {
                case(EnemyType.Slicer):
                    newEnemy = new Slicer(manager.thisScene, position);
                    break;
                case (EnemyType.Tortoise):
                    newEnemy = new Tortoise(manager.thisScene, position);
                    break;
                case (EnemyType.Dragonfly):
                    newEnemy = new Dragonfly(manager.thisScene, position);
                    break;
                case (EnemyType.Komodo):
                    newEnemy = new Komodo(manager.thisScene, position);
                    break;
                case (EnemyType.Phantom):
                    newEnemy = new Phantom(manager.thisScene, position);
                    break;
                default:
                    break;
            }

            return newEnemy;
        }

        public Enemy SpawnEnemyAtAngle(EnemyType type, Vector2 position, float angle, float velocity)
        {
            Enemy e;
            e = SpawnEnemy(type, position);
            e.Rotation = angle;
            e.DrawAtTrueRotation = true;
            e.Velocity = velocity;

            return e;
        }

        public void BeginBossBattle(Boss thisBoss)
        {
            manager.thisScene.levelBoss = thisBoss;
            scriptManager.Execute(manager.thisScene.ShowBossHealthBar);
        }
    }
}
