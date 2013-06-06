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
    public class Level3 : Level
    {
        // Content
        public static Texture2D Level3GroundTexture;
        //public static Texture2D Level2CloudTexture;
        public static SoundEffect Level3Theme;
        public static Texture2D Level3TitleTexture;


        public Level3(LevelManager thisManager) : base(thisManager) { }

        public override void Initialize()
        {
            SetupBackground();
            TitleTexture = Level3TitleTexture;
        }

        public override void SetupBackground()
        {
            scrollingBackground = new List<ScrollingBackgroundLayer>();

            // Individually add each layer to the scrolling background...
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level3GroundTexture, 70f, Color.White));
            scrollingBackground[0].DrawLayer = .99f;
        }

        public override IEnumerator<float> LevelScript()
        {
            // Time remaining: 65 seconds.
            manager.thisScene.fader.LerpColor(Color.Transparent, 1f);
            Enemy e;

            AudioManager.PlaySong(Level3Theme, false, .5f);

            TitleShown = true;
            manager.thisScene.LerpTitleColor(Color.White, 1f);
            yield return 1f;

            yield return 2f;

            manager.thisScene.LerpTitleColor(Color.Transparent, 1f);
            yield return 1f;
            TitleShown = false;

            // Time remaining: 61 seconds.

            e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(600, -50), (float)Math.PI / 2f * 1.5f, 100f);
            e.CustomValue1 = .3f;
            e.CustomValue2 = 3f;
            scriptManager.Execute(BulletFountain, e);

            yield return 1.5f;

            // Time remaining: 59.5 seconds.

            e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(100, -50), (float)Math.PI / 2f * .5f, 100f);
            e.CustomValue1 = .3f;
            e.CustomValue2 = 3f;
            scriptManager.Execute(BulletFountain, e);

            yield return 3f;

            // Time remaining: 56.5 seconds

            //Now spawn some slicers to drop down and fire spread shots.
            List<Enemy> firingSquad = new List<Enemy>();
            for (int i = 0; i < 5; i++)
            {
                float x = 175 + (75 * i);
                e = SpawnEnemy(EnemyType.Slicer, new Vector2(x, -40));
                e.CustomValue1 = i * .5f;
                e.Rotation = (float)Math.PI / 2f;
                e.LerpPosition(new Vector2(x, 250), 1.8f);
                scriptManager.Execute(LazySpreadshot, e);

                firingSquad.Add(e);
                yield return .4f;
            }

            // Time remaining 54.5

            yield return 3.5f;

            // Time remaining 51;

            for (int i = 0; i < 8; i++)
            {
                float x;
                if (i % 2 == 0)
                    x = 0;
                else
                    x = 650;

                SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(x, -50), VectorMathHelper.GetAngleTo(new Vector2(x, -50), manager.thisScene.player.InnerHitbox.Center), 180f);

                yield return .5f;
            }

            // Time remaining 47;

            yield return 1f;

            // Time remaining 46
            if (firingSquad.Count > 0)
            {
                foreach (Enemy en in firingSquad)
                {
                    en.LerpVelocity(60, 3f);
                }
            }

            // Now do something else in the meantime... BRING IN A KOMODO!!
            scrollingBackground[0].LerpSpeed(160f, 4.5f);
            e = SpawnEnemy(EnemyType.Komodo, new Vector2(325, -40f));
            e.Health = 100;
            e.Rotation = (float)Math.PI / 2f;
            e.Velocity = 50f;
            e.LerpVelocity(0f, 4f);
            scriptManager.Execute(FirstKomodo, e);

            Enemy firstKomodo = e;

            yield return 4.5f;
            // Komodo in position, remaining time: 41.5

            yield return 10f;

            // Remaining time 31.5

            for (int i = 0; i < 20; i++)
            {
                float x;
                if (firstKomodo == null || firstKomodo.Health <= 0)
                {
                    if (i % 2 == 0)
                        x = 0;
                    else
                        x = 650;

                    SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(x, -50), VectorMathHelper.GetAngleTo(new Vector2(x, -50), manager.thisScene.player.InnerHitbox.Center), 180f);
                }
                
                yield return .5f;
            }
            
            // Remaining time 21.5

            if (firstKomodo != null && firstKomodo.Health > 0)
            {
                firstKomodo.Rotation = VectorMathHelper.GetAngleTo(firstKomodo.Center, manager.thisScene.player.InnerHitbox.Center);
                firstKomodo.LerpVelocity(130f, 3f);
            }

            yield return 3f;

            // Komodo is gone no matter what now. Remaining time 18.5

            Random rand = new Random();
            for (int i = 0; i < 12; i++)
            {
                if (rand.Next(0, 2) > 0)
                {
                    e = SpawnEnemy(EnemyType.Tortoise, new Vector2(rand.Next(25, 625), -40));
                    e.Rotation = (float)Math.PI / 2;
                    e.Velocity = 120f;
                    e.LerpVelocity(55f, 3f);
                    e.CustomValue1 = 1.5f;
                    e.CustomValue2 = 2f;
                    e.CustomValue2 = 170f;
                    scriptManager.Execute(BulletFountain, e);
                }
                else
                {
                    e = SpawnEnemy(EnemyType.Slicer, new Vector2(rand.Next(25, 625), -40));
                    e.Rotation = (float)Math.PI / 2;
                    e.Velocity = 160f;
                    e.LerpVelocity(55f, 3f);
                    scriptManager.Execute(LazySpreadshot, e);
                }

                yield return 1f;
            }

            yield return 3f;

            manager.thisScene.PlayBossWarning();
            yield return 6.6f;
            //SPAWN THE BOSS

            Boss boss = new CrablordBoss(manager.thisScene, new Vector2(300f, -150f));
            BeginBossBattle(boss);

            yield return 1.5f;
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
            foreach (GameObject go in manager.thisScene.gameObjects.FindAll(x => x is CrablordEgg))
            {
                go.Destroy();
            }
            yield return 2f;
            manager.thisScene.fader.LerpColor(Color.Transparent, 3f);
            yield return 3f;

            manager.thisScene.fader.LerpColor(Color.Black, 2f);


            yield return 2f;
            manager.thisScene.player.Phasing = false;
            manager.SetLevel(4);
        }


        public IEnumerator<float> BulletFountain(GameObject go)
        {
            BulletEmitter thisEmitter = new BulletEmitter(go, go.Center, false);
            thisEmitter.LockedToParentPosition = true;
            thisEmitter.LockPositionOffset = Vector2.Zero;

            yield return go.CustomValue1;
            float timeToFire = go.CustomValue2;

            float startTime = manager.thisScene.currentGameTime;
            while (manager.thisScene.currentGameTime < startTime + timeToFire)
            {
                if(manager.thisScene.PointOnScreen(go.Center))
                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .1f, -.3f);

                thisEmitter.FireBulletCluster(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center), 1, 10f, (float)Math.Max(100, go.CustomValue3), 0f, Color.DeepSkyBlue);
                yield return .06f;
            }
        }

        public IEnumerator<float> LazySpreadshot(GameObject go)
        {
            yield return go.CustomValue1;

            while (true)
            {
                int shots = 0;
                while (shots < 4)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot3Sound, .8f, .6f);
                    new BulletEmitter(go, go.Center, true).FireBulletSpread((float)Math.PI / 2f, 4, 90f, 180f, Color.Orange);
                    yield return .35f;
                    shots++;
                }

                yield return 3f;
            }
        }

        public IEnumerator<float> FirstKomodo(GameObject go)
        {
            yield return 3.5f;

            while (true)
            {
                int shots = 0;
                while (shots < 10)
                {
                    shots++;

                    AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .7f, -.3f);
                    foreach (Bullet b in new BulletEmitter(go, go.Center, true).FireBulletExplosion(15, 80f, Color.DeepSkyBlue))
                    {
                        scriptManager.Execute(SlowHomingBullet, b);
                    }

                    yield return .4f;
                }

                yield return 2f;

                shots = 0;
                while (shots < 50)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot2Sound, .3f, 0f);
                    shots++;
                    new BulletEmitter(go, go.Center, true).FireBulletCluster((float)Math.PI / 2f, 3, 180f, 210f, 50f, Color.Orange, BulletType.DiamondSmall);

                    yield return .15f;
                }

                yield return 2f;
            }
        }

        public IEnumerator<float> SlowHomingBullet(GameObject go)
        {
            yield return 1.5f;

            AudioManager.PlaySoundEffect(GameScene.Shot8Sound, .3f, -.6f);
            go.LerpVelocity(0f, .5f);

            go.Rotation = VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center);
            go.LerpVelocity(145f, 1.5f);
        }
    }
}