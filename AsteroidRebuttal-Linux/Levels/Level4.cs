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
    public class Level4 : Level
    {
        // Content
        public static Texture2D Level4GroundTexture;
        public static Texture2D Level4Text1_1;
		public static Texture2D Level4Text1_2;
		public static Texture2D Level4Text1_3;
		public static Texture2D Level4Text1_4;
		public static Texture2D Level4Text1_5;
		public static Texture2D Level4Text1_6;
		public static Texture2D Level4Text2_1;
		public static Texture2D Level4Text2_2;
		public static Texture2D Level4Text2_3;
		public static Texture2D Level4Text2_4;
		public static Texture2D Level4Text2_5;
		public static Texture2D Level4Text3_1;
		public static Texture2D Level4Text3_2;
		public static Texture2D Level4Text3_3;
		public static Texture2D Level4Text4_1;
		public static Texture2D Level4Text4_2;
		public static Texture2D Level4Text4_3;

        public static Texture2D Level4Warning1Texture;
        public static Texture2D Level4Warning2Texture;

        public static Texture2D Level4TitleTexture;

        public static SoundEffect Level4Theme;
        bool WarningFlashing;


        public Level4(LevelManager thisManager) : base(thisManager) { }

        public override void Initialize()
        {
            SetupBackground();
            TitleTexture = Level4TitleTexture;
        }

        public override void SetupBackground()
        {
            scrollingBackground = new List<ScrollingBackgroundLayer>();

            // Individually add each layer to the scrolling background...
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4GroundTexture, 30f, Color.White));

			// Oh boy...
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text1_1, 45f, Color.White, 90));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text1_1, 45f, Color.White, 265));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text1_1, 45f, Color.White, 307));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text1_1, 45f, Color.White, 383));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text1_1, 45f, Color.White, 555));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text1_1, 45f, Color.White, 569));

			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text2_1, 65f, Color.White, 17));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text2_2, 65f, Color.White, 74));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text2_3, 65f, Color.White, 137));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text2_4, 65f, Color.White, 361));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text2_5, 65f, Color.White, 614));

			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text3_1, 90f, Color.White, 238));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text3_2, 90f, Color.White, 489));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text3_3, 90f, Color.White, 670));

			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text4_1, 90f, Color.White, 164));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text4_2, 90f, Color.White, 410));
			scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Text4_3, 90f, Color.White, 714));

            // Add the warning layers but hide them until needed.
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Warning1Texture, -160f, Color.Transparent, 67));
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level4Warning2Texture, -45f, Color.Transparent, 502));

            scrollingBackground[0].DrawLayer = .99f;	//Base BG

            scrollingBackground[1].DrawLayer = .98f;	// Text layer 1
            scrollingBackground[2].DrawLayer = .98f;
            scrollingBackground[3].DrawLayer = .98f;
            scrollingBackground[4].DrawLayer = .98f;
            scrollingBackground[5].DrawLayer = .98f;
            scrollingBackground[6].DrawLayer = .98f;

			scrollingBackground[7].DrawLayer = .97f;	// Text layer 2
			scrollingBackground[8].DrawLayer = .97f;
			scrollingBackground[9].DrawLayer = .97f;
			scrollingBackground[10].DrawLayer = .97f;
			scrollingBackground[11].DrawLayer = .97f;

			scrollingBackground[12].DrawLayer = .96f;	// Text layer 3
			scrollingBackground[13].DrawLayer = .96f;
			scrollingBackground[14].DrawLayer = .96f;

			scrollingBackground[15].DrawLayer = .95f;	// Text layer 4
			scrollingBackground[16].DrawLayer = .95f;
			scrollingBackground[17].DrawLayer = .95f;

			scrollingBackground[18].DrawLayer = .94f;	// Warnings
			scrollingBackground[19].DrawLayer = .94f;

        }

        public override IEnumerator<float> LevelScript()
        {
            // Time remaining: 57 seconds.
            manager.thisScene.fader.LerpColor(Color.Transparent, 1f);
            Enemy e;

            AudioManager.PlaySong(Level4Theme, false, .5f);

            TitleShown = true;
            manager.thisScene.LerpTitleColor(Color.White, 1f);
            yield return 1f;

            yield return 2f;

            manager.thisScene.LerpTitleColor(Color.Transparent, 1f);
            yield return 1f;
            TitleShown = false;

            // Time remaining: 53 seconds.

            // For the beginning, spawn two lines of firing dragonflies
            for (int i = 0; i < 20; i++)
            {
                e = SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(100 + (i * 27), -35), (float)Math.PI / 2f, 160f);
                e.CustomValue1 = 1.5f;
                e.CustomValue2 = (i / 20f) * 2f;
                scriptManager.Execute(DragonflyDelayedShoot, e);
                e = SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(600 - (i * 27), 685), (float)(Math.PI / 2f) * 3, 160f);
                e.CustomValue1 = 1.5f;
                e.CustomValue2 = (i / 20f) * 2f;
                scriptManager.Execute(DragonflyDelayedShoot, e);
                yield return .5f;
            }

            yield return 3f;

            // Time remaining 40

            // Drop down turtles...
            e = SpawnEnemy(EnemyType.Tortoise, new Vector2(350, -35f));
            e.Rotation = (float)Math.PI / 2f;
            e.CustomValue1 = 1f;
            e.CustomValue2 = (float)Math.PI / 2f;
            e.Velocity = 40f;
            scriptManager.Execute(TurtleBarrage, e);

            e = SpawnEnemy(EnemyType.Tortoise, new Vector2(320, -65f));
            e.Rotation = (float)Math.PI / 2f;
            e.CustomValue1 = 2f;
            e.CustomValue2 = (float)(Math.PI / 4f) * 3;
            e.Velocity = 40f;
            scriptManager.Execute(TurtleBarrage, e);

            e = SpawnEnemy(EnemyType.Tortoise, new Vector2(380, -65f));
            e.Rotation = (float)Math.PI / 2f;
            e.CustomValue1 = 2f;
            e.CustomValue2 = (float)(Math.PI / 4f) * 1;
            e.Velocity = 40f;
            scriptManager.Execute(TurtleBarrage, e);

            yield return 6f;
            // Remaining time: 36
            // Spawn dragonflies from the sides
            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(-30, 400), 0f, 160f);
            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(730, 550), (float)Math.PI, 160f);
            yield return 1f;

            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(-30, 450), 0f, 160f);
            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(730, 300), (float)Math.PI, 160f);
            yield return 1f;

            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(-30, 200), 0f, 160f);
            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(730, 150), (float)Math.PI, 160f);
            yield return 1f;

            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(-30, 350), 0f, 160f);
            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(730, 250), (float)Math.PI, 160f);
            yield return 1f;

            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(-30, 570), 0f, 160f);
            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(730, 600), (float)Math.PI, 160f);
            yield return 1f;

            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(-30, 320), 0f, 160f);
            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(730, 470), (float)Math.PI, 160f);
            yield return 1f;

            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(-30, 210), 0f, 160f);
            SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(730, 350), (float)Math.PI, 160f);
            yield return 1f;

            // Time remaining: 30

            yield return 1f;

            // Time remaining: 28

            // Spawn a strafing Komodo from below
            e = SpawnEnemyAtAngle(EnemyType.Komodo, new Vector2(150, 680), (float)Math.PI / 2 * 3, 60f);
            scriptManager.Execute(KomodoStrafe, e);
            yield return 4f;
            e = SpawnEnemyAtAngle(EnemyType.Komodo, new Vector2(550, -30), (float)Math.PI / 2, 60f);
            scriptManager.Execute(KomodoStrafeBlue, e);
            yield return 4f;

			// Turtles removed

            yield return 4f;

            // Now have the enemies zoom in from the bottom
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(0 + j * 80, 680), (float)(Math.PI / 2f) * 3, 90f);
                }

                yield return .5f;

                for (int j = 0; j < 15; j++)
                {
                    SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(40 + j * 80, 680), (float)(Math.PI / 2f) * 3, 90f);
                }

                yield return .5f;
            }

            yield return .7f;

            // Lerp the background to INFECTION COLORS!
            scrollingBackground[0].LerpColor(Color.Lerp(Color.Yellow, Color.White, .5f), 6f);

            scrollingBackground[1].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[1].LerpSpeed(150f, 6f);
			scrollingBackground[2].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[2].LerpSpeed(150f, 6f);
			scrollingBackground[3].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[3].LerpSpeed(150f, 6f);
			scrollingBackground[4].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[4].LerpSpeed(150f, 6f);
			scrollingBackground[5].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[5].LerpSpeed(150f, 6f);
			scrollingBackground[6].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[6].LerpSpeed(150f, 6f);

            scrollingBackground[7].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[7].LerpSpeed(200f, 6f);
			scrollingBackground[8].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[8].LerpSpeed(200f, 6f);
			scrollingBackground[9].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[9].LerpSpeed(200f, 6f);
			scrollingBackground[10].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[10].LerpSpeed(200f, 6f);
			scrollingBackground[11].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[11].LerpSpeed(200f, 6f);

            scrollingBackground[12].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[12].LerpSpeed(250f, 6f);
			scrollingBackground[13].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[13].LerpSpeed(250f, 6f);
			scrollingBackground[14].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[14].LerpSpeed(250f, 6f);

            scrollingBackground[15].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[15].LerpSpeed(300f, 6f);
			scrollingBackground[16].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[16].LerpSpeed(300f, 6f);
			scrollingBackground[17].LerpColor(Color.DarkRed, 6f);
            scrollingBackground[17].LerpSpeed(300f, 6f);

            manager.thisScene.PlayBossWarning();
            scriptManager.Execute(Warning1Flash);
            scriptManager.Execute(Warning2Flash);
            yield return 6.6f;
            //SPAWN THE BOSS

            Boss boss = new ParasiteBoss(manager.thisScene, new Vector2(300f, -150f));
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

            

            yield return 1f;

            WarningFlashing = false;
            scrollingBackground[0].LerpColor(Color.White, 1f);
            scrollingBackground[1].LerpColor(Color.White, 1f);
            scrollingBackground[2].LerpColor(Color.White, 1f);
            scrollingBackground[3].LerpColor(Color.White, 1f);
            scrollingBackground[4].LerpColor(Color.White, 1f);
			scrollingBackground[5].LerpColor(Color.White, 1f);
			scrollingBackground[6].LerpColor(Color.White, 1f);
			scrollingBackground[7].LerpColor(Color.White, 1f);
			scrollingBackground[8].LerpColor(Color.White, 1f);
			scrollingBackground[9].LerpColor(Color.White, 1f);
			scrollingBackground[10].LerpColor(Color.White, 1f);
			scrollingBackground[11].LerpColor(Color.White, 1f);
			scrollingBackground[12].LerpColor(Color.White, 1f);
			scrollingBackground[13].LerpColor(Color.White, 1f);
			scrollingBackground[14].LerpColor(Color.White, 1f);
			scrollingBackground[15].LerpColor(Color.White, 1f);
			scrollingBackground[16].LerpColor(Color.White, 1f);
			scrollingBackground[17].LerpColor(Color.White, 1f);


            scrollingBackground[18].LerpColor(Color.Transparent, 1f);
            scrollingBackground[19].LerpColor(Color.Transparent, 1f);

            yield return 1f;

            manager.thisScene.fader.LerpColor(Color.Transparent, 3f);
            yield return 3f;

            manager.thisScene.fader.LerpColor(Color.Black, 2f);


            yield return 2f;
            manager.thisScene.player.Phasing = false;
            manager.SetLevel(5);
        }

        public IEnumerator<float> DragonflyDelayedShoot(GameObject go)
        {
            yield return go.CustomValue2;

            while (true)
            {
                AudioManager.PlaySoundEffect(GameScene.Shot5Sound, .6f, 0f);
                new BulletEmitter(go, go.Center, true).FireBulletCluster(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center), 1, 10f, 130f, 150f, Color.Orange);
                yield return go.CustomValue1;
            }
        }

        public IEnumerator<float> TurtleBarrage(GameObject go)
        {
            yield return go.CustomValue1;

            while (true)
            {
                int shots = 0;
                while (shots < 20)
                {
                    if(manager.thisScene.PointOnScreen(go.Center))
                        AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .15f, 0f);

                    new BulletEmitter(go, go.Center, true).FireBulletCluster(go.CustomValue2, 1, 25f, 130f, 30f, Color.DeepSkyBlue);
                    shots++;
                    yield return .06f;
                }

                yield return 4f;
            }
        }

        public IEnumerator<float> KomodoStrafe(GameObject go)
        {
            while (true)
            {
                if(manager.thisScene.PointOnScreen(go.Center))
                    AudioManager.PlaySoundEffect(GameScene.Shot6Sound, .45f, .4f);

                new BulletEmitter(go, go.Center, true).FireBullet(0f, 150f, Color.Orange).LerpVelocity(50f, 3f);
                new BulletEmitter(go, go.Center, true).FireBullet((float)Math.PI, 150f, Color.Orange).LerpVelocity(50f, 3f);
                yield return .24f;
            }
        }
        public IEnumerator<float> KomodoStrafeBlue(GameObject go)
        {
            while (true)
            {
                new BulletEmitter(go, go.Center, true).FireBullet(0f, 150f, Color.DeepSkyBlue).LerpVelocity(50f, 3f);
                new BulletEmitter(go, go.Center, true).FireBullet((float)Math.PI, 150f, Color.DeepSkyBlue).LerpVelocity(50f, 3f);
                yield return .24f;
            }
        }

        public IEnumerator<float> Warning1Flash()
        {
            WarningFlashing = true;
            while (WarningFlashing)
            {
                scrollingBackground[18].LerpColor(Color.Lerp(Color.Transparent, Color.White, .5f), 3f);
                yield return 4f;
                scrollingBackground[18].LerpColor(Color.Lerp(Color.Transparent, Color.White, .1f), 3f);
                yield return 4f;
            }
        }

        public IEnumerator<float> Warning2Flash()
        {
            WarningFlashing = true;
            while (WarningFlashing)
            {
                scrollingBackground[19].LerpColor(Color.Lerp(Color.Transparent, Color.White, .4f), 2.1f);
                yield return 2.7f;
                scrollingBackground[19].LerpColor(Color.Lerp(Color.Transparent, Color.White, .08f), 2.1f);
                yield return 2.7f;
            }
        }
    }
}