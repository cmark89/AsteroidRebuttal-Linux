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
    public class Level2 : Level
    {
        // Content
        public static Texture2D Level2GroundTexture;
        public static Texture2D Level2CloudTexture;
        public static SoundEffect Level2Theme;
        public static Texture2D Level2TitleTexture;


        public Level2(LevelManager thisManager) : base(thisManager) { }

        public override void Initialize()
        {
            SetupBackground();
            TitleTexture = Level2TitleTexture;
        }

        public override void SetupBackground()
        {
            scrollingBackground = new List<ScrollingBackgroundLayer>();

            // Individually add each layer to the scrolling background...
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level2GroundTexture, 50f, Color.White));
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level2CloudTexture, 85f, Color.White));

            scrollingBackground[0].DrawLayer = .99f;
            scrollingBackground[1].DrawLayer = .98f;
        }

        public override IEnumerator<float> LevelScript()
        {
            manager.thisScene.fader.LerpColor(Color.Transparent, 1f);
            // Time remaining: 65 seconds.
            Enemy e;

            AudioManager.PlaySong(Level2Theme, false, .5f);

            TitleShown = true;
            manager.thisScene.LerpTitleColor(Color.White, 1f);
            yield return 1f;

            yield return 2f;

            manager.thisScene.LerpTitleColor(Color.Transparent, 1f);
            yield return 1f;
            TitleShown = false;

            // Time remaining: 61 seconds

            // Begin spawning two columns of dragonflies from the sides
            for (int i = 0; i < 10; i++)
            {
                e = new Dragonfly(manager.thisScene, new Vector2(100, 700));
                scriptManager.Execute(ColumnDragonfly, e);
                

                e = new Dragonfly(manager.thisScene, new Vector2(600, 700));
                scriptManager.Execute(ColumnDragonfly, e);
                yield return .4f;
            }

            // Time remaining: 57 seconds

            e = SpawnStrafingSlicer(new Vector2(780, 70f), (float)(Math.PI / 8f) * 7);
            scriptManager.Execute(SimpleStrafe, e);
            yield return .5f;

            e = SpawnStrafingSlicer(new Vector2(-80, 90f), (float)(Math.PI / 8f) * 1);
            scriptManager.Execute(SimpleStrafe, e);
            yield return .5f;

            e = SpawnStrafingSlicer(new Vector2(780, 150f), (float)(Math.PI / 8f) * 7);
            scriptManager.Execute(SimpleStrafe, e);
            yield return .5f;

            e = SpawnStrafingSlicer(new Vector2(-80, 150f), (float)(Math.PI / 8f) * 1);
            scriptManager.Execute(SimpleStrafe, e);
            yield return .5f;

            e = SpawnStrafingSlicer(new Vector2(780, 70f), (float)(Math.PI / 8f) * 7);
            scriptManager.Execute(SimpleStrafe, e);
            yield return .5f;

            e = SpawnStrafingSlicer(new Vector2(-80, 90f), (float)(Math.PI / 8f) * 1);
            scriptManager.Execute(SimpleStrafe, e);
            yield return .5f;

            e = SpawnStrafingSlicer(new Vector2(780, 150f), (float)(Math.PI / 8f) * 7);
            scriptManager.Execute(SimpleStrafe, e);
            yield return .5f;

            e = SpawnStrafingSlicer(new Vector2(-80, 150f), (float)(Math.PI / 8f) * 1);
            scriptManager.Execute(SimpleStrafe, e);
            yield return .5f;

            yield return 4f;

            e = SpawnEnemy(EnemyType.Tortoise, new Vector2(350, -50));
            e.CustomValue1 = 1.5f;
            scriptManager.Execute(EasyTortoise, e);
            e = SpawnEnemy(EnemyType.Tortoise, new Vector2(300, -80));
            e.CustomValue1 = 3f;
            scriptManager.Execute(EasyTortoise, e);
            e = SpawnEnemy(EnemyType.Tortoise, new Vector2(400, -80));
            e.CustomValue1 = 3f;
            scriptManager.Execute(EasyTortoise, e);

            yield return 3f;

            for (int i = 0; i < 6; i++)
            {
                e = SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(25, -50), (float)Math.PI / 10 * 3f, 180f);
                e.LerpVelocity(90f, 3.5f);
                e.LerpRotation(VectorMathHelper.GetAngleTo(new Vector2(0, 0), manager.thisScene.player.InnerHitbox.Center), 3.5f);
                yield return .5f;
            }

            // Time remaining: 44 seconds
            yield return 1f;

            // Time remaining: 43 seconds

            // Spawn several stabbing slicers at the side
            for (int i = 0; i < 5; i++)
            {
                e = SpawnEnemyAtAngle(EnemyType.Slicer, new Vector2(700, new Random().Next(25, 600)), 0f, 100f);
                scriptManager.Execute(StabbingSlicer, e);
                yield return 1f;
            }
            
            // Time remaining 36 seconds
            Enemy firstKomodo = SpawnEnemy(EnemyType.Komodo, new Vector2(350, -50));
            firstKomodo.Health = 60;
            scriptManager.Execute(FirstKomodo, firstKomodo);

            yield return 8f;

            if (firstKomodo == null || firstKomodo.Health <= 0)
            {
                e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(-50, 500), 0f, 55f);
                e.CustomValue2 = ((float)Math.PI / 8f) * 15;
                e.DeletionBoundary = new Vector2(200, 200);
                scriptManager.Execute(EasyTortoise, e);
                yield return 2f;
                e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(730, 500), (float)Math.PI, 55f);
                e.CustomValue2 = ((float)Math.PI / 8f) * 9;
                e.DeletionBoundary = new Vector2(200, 200);
                scriptManager.Execute(EasyTortoise, e);
                yield return 2f;
            }
            else
            {
                yield return 4f;
            }

            // Remaining time: 24
            if (firstKomodo != null || firstKomodo.Health > 0)
            {
                scriptManager.AbortObjectScripts(firstKomodo);
                firstKomodo.Rotation = ((float)Math.PI / 2f) * 3;
                firstKomodo.LerpVelocity(70f, 2f);
            }

            for (int i = 0; i < 6; i++)
            {
                e = SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(730, 25), (float)Math.PI / 8 * 7f, 180f);
                e.LerpVelocity(90f, 3.5f);
                e.LerpRotation(VectorMathHelper.GetAngleTo(new Vector2(700, 0), manager.thisScene.player.InnerHitbox.Center), 3.5f);
                yield return .5f;
            }

            yield return 3f;

            // Remaining time 18

            e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(-50, 500), 0f, 55f);
            e.CustomValue2 = ((float)Math.PI / 8f) * 15;
            e.DeletionBoundary = new Vector2(200, 200);
            scriptManager.Execute(EasyTortoise, e);
            yield return 2f;
            e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(730, 500), (float)Math.PI, 55f);
            e.CustomValue2 = ((float)Math.PI / 8f) * 9;
            e.DeletionBoundary = new Vector2(200, 200);
            scriptManager.Execute(EasyTortoise, e);
            
            yield return 4f;

            Enemy secondKomodo = SpawnEnemy(EnemyType.Komodo, new Vector2(350, -50f));
            scriptManager.Execute(SecondKomodo, secondKomodo);
            // Remaining time 12

            yield return 10f;

            manager.thisScene.PlayBossWarning();
            yield return 6.6f;
            //SPAWN THE BOSS

            Boss boss = new PhantomBoss(manager.thisScene, new Vector2(300f, 100f));
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
            yield return 2f;
            manager.thisScene.fader.LerpColor(Color.Transparent, 3f);
            yield return 3f;

            manager.thisScene.fader.LerpColor(Color.Black, 2f);


            yield return 2f;
            manager.thisScene.player.Phasing = false;
            manager.SetLevel(3);

        }

        IEnumerator<float> ColumnDragonfly(GameObject go)
        {
            go.DrawAtTrueRotation = true;
            go.Rotation = ((float)Math.PI / 2) * 3;
            go.Velocity = 150f;
            yield return 1f;
            go.LerpVelocity(50f, 2f);
            yield return 2f;
            go.Rotation = VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center);
            go.LerpVelocity(150f, 2f);
        }

        public Enemy SpawnStrafingSlicer(Vector2 position, float rotation, float startVelocity = 35f, float endVelocity = 50f)
        {
            Enemy e = new Slicer(manager.thisScene, position);
            e.DrawAtTrueRotation = true;
            e.Rotation = rotation;
            e.Velocity = startVelocity;
            e.LerpVelocity(endVelocity, 5f);

            return e;
        }

        public IEnumerator<float> SimpleStrafe(GameObject go)
        {
            while (true)
            {
                yield return 1.3f;
                if (manager.thisScene.PointOnScreen(go.Center))
                    AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .7f, .5f);
                new BulletEmitter(go, go.Center, true).FireBulletCluster(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center), 1, 20f, 100f, 0f, Color.Orange);
            }
        }

        public IEnumerator<float> StabbingSlicer(GameObject go)
        {
            go.Rotation = VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center);
            yield return .5f;

            while (true)
            {
                AudioManager.PlaySoundEffect(GameScene.Shot3Sound, .7f, .5f);
                new BulletEmitter(go, go.Center, true).FireBulletSpread(go.Rotation, 3, 70f, 130f, Color.Orange);
                yield return 1.5f;
            }
        }

        public IEnumerator<float> EasyTortoise(GameObject go)
        {
            if (go.CustomValue2 == 0)
                go.Rotation = (float)Math.PI / 2f;
            else
                go.Rotation = go.CustomValue2;
            go.Velocity = 75f;
            go.LerpVelocity(45f, 5f);

            yield return go.CustomValue1;
            while (true)
            {
                AudioManager.PlaySoundEffect(GameScene.Shot8Sound, .7f, 0f);
                new BulletEmitter(go, go.Center, true).FireBulletExplosion(12, 80f, Color.DeepSkyBlue);
                yield return 2f;
            }
        }

        public IEnumerator<float> FirstKomodo(GameObject go)
        {
            go.Rotation = (float)Math.PI / 2f;
            go.Velocity = 35f;
            yield return 1.5f;
            go.LerpVelocity(0f, 1.5f);
            yield return 1.5f;

            BulletEmitter emitter = new BulletEmitter(go, go.Center);
            while (true)
            {
                int shots = 0;
                while(shots < 10)
                {
                    shots++;

                    emitter.Rotation += .2f;
                    AudioManager.PlaySoundEffect(GameScene.Shot8Sound, .7f, .5f);
                    foreach(Bullet b in emitter.FireBulletExplosion(20, 200f, Color.DeepSkyBlue))
                    {
                        b.LerpVelocity(40f, 4f);
                    }

                    yield return .2f;
                }

                yield return 4f;
            }
        }

        public IEnumerator<float> SecondKomodo(GameObject go)
        {
            go.Rotation = (float)Math.PI / 2f;
            go.Velocity = 40f;
            yield return 1.5f;

            BulletEmitter emitter = new BulletEmitter(go, go.Center);
            while (true)
            {
                int shots = 0;
                while (shots < 10)
                {
                    shots++;

                    emitter.Center = go.Center;
                    emitter.Rotation += .2f;
                    AudioManager.PlaySoundEffect(GameScene.Shot8Sound, .7f, -.4f);
                    foreach (Bullet b in emitter.FireBulletExplosion(10, 100f, Color.DeepSkyBlue))
                    {
                        b.LerpVelocity(40f, 4f);
                    }

                    yield return .3f;
                }

                yield return 2f;
            }
        }
    }
}
