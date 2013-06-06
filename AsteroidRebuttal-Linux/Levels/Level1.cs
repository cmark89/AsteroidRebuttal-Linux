using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Enemies;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Enemies.Bosses;

namespace AsteroidRebuttal.Levels
{
    public class Level1 : Level
    {
        // Content
        public static Texture2D Level1GroundTexture;
        public static Texture2D Level1TitleTexture;
        public static SoundEffect Level1Theme;


        public Level1(LevelManager thisManager) : base(thisManager) { }

        public override void Initialize()
        {
            SetupBackground();
            TitleTexture = Level1TitleTexture;
        }

        public override void SetupBackground()
        {
            scrollingBackground = new List<ScrollingBackgroundLayer>();

            // Individually add each layer to the scrolling background...
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level1GroundTexture, 100f, Color.White));
            scrollingBackground[0].DrawLayer = .99f;
        }

        public override IEnumerator<float> LevelScript()
        {
            manager.thisScene.fader.LerpColor(Color.Transparent, 1f);
            Enemy e;

            AudioManager.PlaySong(Level1Theme, false, .5f);

            TitleShown = true;
            manager.thisScene.LerpTitleColor(Color.White, 1f);
            yield return 1f;

            yield return 2f;

            manager.thisScene.LerpTitleColor(Color.Transparent, 1f);
            yield return 1f;
            TitleShown = false;

            // A wave of downward enemies 
            for (int i = 0; i < 5; i++)
            {
                e = SpawnEnemy(EnemyType.Slicer, new Vector2(100 + 100 * i, -40));
                e.Velocity = 100f;

                e.CustomValue1 = 3f;
                e.CustomValue2 = 100f;

                yield return .8f;
            }

            yield return .5f;

            // Level time: 8.5 seconds

            // Enemies coming in from the sides at an angle with homing shots
            e = SpawnEnemyAtAngle(EnemyType.Slicer, new Vector2(manager.thisScene.ScreenArea.Width + 60, 25), (float)Math.PI / 4 * 3, 70);
            e.CustomValue1 = .9f;
            e.CustomValue2 = 100;
            scriptManager.Execute(SimplePlayerShot, e);
            yield return 1f;

            e = SpawnEnemyAtAngle(EnemyType.Slicer, new Vector2(-60, 25), (float)Math.PI / 4, 70);
            e.CustomValue1 = .9f;
            e.CustomValue2 = 100;
            scriptManager.Execute(SimplePlayerShot, e);
            yield return 1f;

            e = SpawnEnemyAtAngle(EnemyType.Slicer, new Vector2(manager.thisScene.ScreenArea.Width + 60, 55), (float)Math.PI / 4 * 3, 70);
            e.CustomValue1 = .9f;
            e.CustomValue2 = 100;
            scriptManager.Execute(SimplePlayerShot, e);
            yield return 1f;

            e = SpawnEnemyAtAngle(EnemyType.Slicer, new Vector2(-60, 55), (float)Math.PI / 4, 70);
            e.CustomValue1 = .9f;
            e.CustomValue2 = 100;
            scriptManager.Execute(SimplePlayerShot, e);

            yield return 4f;

            // Level time: 15.5 seconds
            List<Enemy> enemygroup = new List<Enemy>();
            // Spawn a larger group of 5 enemies from the top to drop toward the middle before beginning to fire spread shots
            for (int i = 0; i < 3; i++)
            {
                e = SpawnEnemy(EnemyType.Slicer, new Vector2(250 + 50 * i, -40));
                enemygroup.Add(e);
                e.Health = 5;
                e.LerpPosition(new Vector2(e.Center.X, 250), 3f);
                e.CustomValue1 = 1.5f;
                e.CustomValue2 = 3;
                e.CustomValue3 = 50f;
                e.CustomValue4 = 140f;

                scriptManager.Execute(SimplePlayerSpreadShot, e);
            }
            for (int i = 0; i < 2; i++)
            {
                e = SpawnEnemy(EnemyType.Slicer, new Vector2(275 + 50 * i, -80));
                enemygroup.Add(e);
                e.Health = 5;
                e.LerpPosition(new Vector2(e.Center.X, 210), 3f);

                e.CustomValue1 = 2f;
                e.CustomValue2 = 3;
                e.CustomValue3 = 50f;
                e.CustomValue4 = 140f;

                scriptManager.Execute(SimplePlayerSpreadShot, e);
            }

            yield return 3f;

            // Level time: 18.5

            // Spawn a larger group of 5 enemies from the top to drop toward the middle before beginning to fire spread shots
            for (int i = 0; i < 3; i++)
            {
                e = SpawnEnemy(EnemyType.Slicer, new Vector2(100 + 50 * i, -40));
                enemygroup.Add(e);
                e.Health = 3;
                e.LerpPosition(new Vector2(e.Center.X, 350), 3f);
                e.CustomValue1 = 1.5f;
                e.CustomValue2 = 3;
                e.CustomValue3 = 50f;
                e.CustomValue4 = 140f;

                scriptManager.Execute(SimplePlayerSpreadShot, e);
            }
            for (int i = 0; i < 2; i++)
            {
                e = SpawnEnemy(EnemyType.Slicer, new Vector2(125 + 50 * i, -80));
                enemygroup.Add(e);
                e.Health = 3;
                e.LerpPosition(new Vector2(e.Center.X, 310), 3f);

                e.CustomValue1 = 2f;
                e.CustomValue2 = 3;
                e.CustomValue3 = 50f;
                e.CustomValue4 = 140f;

                scriptManager.Execute(SimplePlayerSpreadShot, e);
            }

            yield return 2f;

            // Level time: 20.5

            // Spawn a larger group of 5 enemies from the top to drop toward the middle before beginning to fire spread shots
            for (int i = 0; i < 3; i++)
            {
                e = SpawnEnemy(EnemyType.Slicer, new Vector2(425 + 50 * i, -40));
                enemygroup.Add(e);
                e.Health = 3;
                e.LerpPosition(new Vector2(e.Center.X, 350), 3f);
                e.CustomValue1 = 1.5f;
                e.CustomValue2 = 3;
                e.CustomValue3 = 50f;
                e.CustomValue4 = 140f;

                scriptManager.Execute(SimplePlayerSpreadShot, e);
            }
            for (int i = 0; i < 2; i++)
            {
                e = SpawnEnemy(EnemyType.Slicer, new Vector2(450 + 50 * i, -80));
                enemygroup.Add(e);
                e.Health = 3;
                e.LerpPosition(new Vector2(e.Center.X, 310), 3f);

                e.CustomValue1 = 2f;
                e.CustomValue2 = 3;
                e.CustomValue3 = 50f;
                e.CustomValue4 = 140f;

                scriptManager.Execute(SimplePlayerSpreadShot, e);
            }

            yield return 5f;
            // Level time: 25.5

            foreach (Enemy enemy in enemygroup)
            {
                enemy.LerpVelocity(100f, 3f);
            }

            yield return 3f;

            // Level time 28.5

            // Spawn a tortoise from below a couple times, and then add a couple from above
            e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(55, 655), ((float)Math.PI / 2f) * -.8f, 40f);
            scriptManager.Execute(TortoiseExplosiveShot, e);
            yield return 2.5f;

            e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(555, 655), (((float)Math.PI / 2f) * -.3f) - ((float)Math.PI / 2), 40f);
            scriptManager.Execute(TortoiseExplosiveShot, e);
            yield return 3f;

            e = SpawnEnemy(EnemyType.Tortoise, new Vector2(200f, -50f));
            e.LerpVelocity(40f, 4f);
            scriptManager.Execute(TortoiseExplosiveShot, e);

            e = SpawnEnemy(EnemyType.Tortoise, new Vector2(400f, -50f));
            e.LerpVelocity(40f, 4f);
            scriptManager.Execute(TortoiseExplosiveShot, e);

            // Level time: 34

            yield return 2f;

            // Level time 36

            for (int i = 0; i < 12; i++)
            {
                float x;
                if (i % 2 == 0)
                    x = -40;
                else
                    x = 740;

                Vector2 spawnPoint = new Vector2(x, 100 + (15 * i));

                e = SpawnEnemyAtAngle(EnemyType.Dragonfly, spawnPoint, VectorMathHelper.GetAngleTo(spawnPoint, manager.thisScene.player.InnerHitbox.Center), 130f);
                yield return .5f;
            }

            //Level time 42
            Random rand = new Random();
            for (int i = 0; i < 15; i++)
            {
                e = SpawnEnemy(EnemyType.Slicer, new Vector2(rand.Next(50, 600), -50));
                e.Velocity = 200f;
                e.LerpVelocity(55f, 2f);
                e.CustomValue1 = 1.25f;
                e.CustomValue2 = 135f;
                scriptManager.Execute(SimplePlayerShot, e);

                yield return .5f;
            }

            // Level time 49.5

            yield return 1.5f;

            // A tortoise from below that will track the player with its cannons as it moves
            e = SpawnEnemy(EnemyType.Tortoise, new Vector2(250, 680));
            e.Rotation = (float)Math.PI / 2f * 3f;
            scriptManager.Execute(HeavyTortoise, e);

            yield return 4f;

            // A tortoise from below that will track the player with its cannons as it moves
            e = SpawnEnemy(EnemyType.Tortoise, new Vector2(550, 680));
            e.Rotation = (float)Math.PI / 2f * 3f;
            scriptManager.Execute(HeavyTortoise, e);

            yield return 4f;

            yield return 6f;

            manager.thisScene.PlayBossWarning();
            yield return 6.6f;

            //SPAWN THE BOSS
            Boss boss = new TestBoss(manager.thisScene, new Vector2(350f, -20f));
            BeginBossBattle(boss);
            yield return 1f;
            bossTheme = AudioManager.PlaySong(BossTheme);

            while (boss.Health > 0)
            {
                yield return .03f;
            }

            // Stop the boss theme and hide the health bar
            bossTheme.Dispose();
            manager.thisScene.HideBossHealthbar();

            // Explode the boss!!
            scriptManager.AbortObjectScripts(boss);
            manager.thisScene.player.Phasing = true;
            foreach (GameObject b in manager.thisScene.gameObjects.FindAll(x => x is Bullet))
            {
                scriptManager.AbortObjectScripts(b);
                b.Phasing = true;
                b.LerpColor(Color.Transparent, 1f);
            }

            scriptManager.Execute(manager.thisScene.BossExplosion, boss);
            yield return 3.3f;
            yield return 2f;
            manager.thisScene.fader.LerpColor(Color.Transparent, 3f);
            yield return 3f;

            manager.thisScene.fader.LerpColor(Color.Black, 2f);


            yield return 2f;
            manager.thisScene.player.Phasing = false;
            manager.SetLevel(2);
        }

        public IEnumerator<float> SimplePlayerShot(GameObject go)
        {
            while (true)
            {
                yield return go.CustomValue1;
                AudioManager.PlaySoundEffect(GameScene.Shot6Sound, 1f);
                new BulletEmitter(go, go.Center, true).FireBullet(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center), go.CustomValue2, Color.Orange);
            }
        }

        public IEnumerator<float> SimplePlayerSpreadShot(GameObject go)
        {
            while (true)
            {
                yield return go.CustomValue1;
                AudioManager.PlaySoundEffect(GameScene.Shot2Sound, .4f);
                new BulletEmitter(go, go.Center, true).FireBulletSpread(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center), (int)go.CustomValue2, go.CustomValue3, go.CustomValue4, Color.DeepSkyBlue);
            }
        }

        public IEnumerator<float> TortoiseExplosiveShot(GameObject go)
        {
            yield return 1.5f;
            while (true)
            {
                foreach (Bullet b in new BulletEmitter(go, go.Center, true).FireBulletExplosion(10, 200f, Color.DeepSkyBlue))
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .7f);
                    b.LerpVelocity(100f, 2f);
                }

                yield return 2.72f;
            }
        }

        public IEnumerator<float> HeavyTortoise(GameObject go)
        {
            Enemy e = (Enemy)go;
            e.Rotation = (float)Math.PI / 2f * 3f;
            e.Health = 25f;
            e.LerpVelocity(65f, 8f);
            e.LerpRotation(e.Rotation + VectorMathHelper.GetAngleTo(e.Center, manager.thisScene.player.InnerHitbox.Center) / 2f, 12f);
            yield return 1.5f;

            BulletEmitter mainEmitter = new BulletEmitter(e, e.Center, false);
            mainEmitter.LockedToParentPosition = true;
            mainEmitter.LockPositionOffset = Vector2.Zero;
            yield return .01f;
            while(true)
            {
                int shots  = 0;
                while (shots < 20)
                {
                    if(manager.thisScene.PointOnScreen(go.Center))
                        AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .25f);

                    mainEmitter.FireBulletCluster(VectorMathHelper.GetAngleTo(mainEmitter.Center, manager.thisScene.player.InnerHitbox.Center), 1, 10f, 150f, 0f, Color.DeepSkyBlue);
                    shots++;
                    yield return .06f;
                }

                yield return 1.73f;
            }
        }
    }
}
