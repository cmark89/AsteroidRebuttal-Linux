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
    public class Level5 : Level
    {
        // Content
        public static Texture2D Level5SpaceTexture;
        public static Texture2D Level5NebulaTexture;
        public static Texture2D Level5Stars1Texture;
        public static Texture2D Level5Stars2Texture;
        public static Texture2D Level5Stars3Texture;
        public static Texture2D Level5DebrisTexture;

        public static Texture2D Level5TitleTexture;

        public static SoundEffect Level5Theme;
        public static SoundEffect FinalAttackTheme;

        List<Enemy> circleSquad = new List<Enemy>();
        Vector2 circleSquadOrigin;
        bool circleSquadLeaving = false;

        public Level5(LevelManager thisManager) : base(thisManager) { }

        public override void Initialize()
        {
            SetupBackground();
            TitleTexture = Level5TitleTexture;
        }

        public override void SetupBackground()
        {
            scrollingBackground = new List<ScrollingBackgroundLayer>();
            
            // Individually add each layer to the scrolling background...
            //scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level5SpaceTexture, 10f, Color.White));
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level5NebulaTexture, 20f, Color.White));
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level5Stars1Texture, 30f, Color.White));
            scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level5Stars2Texture, 36f, Color.White));
            //scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level5Stars3Texture, 42f, Color.White));
            //scrollingBackground.Add(new ScrollingBackgroundLayer(manager.thisScene, Level5DebrisTexture, 80f, Color.White));

            scrollingBackground[0].DrawLayer = .99f;
            scrollingBackground[1].DrawLayer = .98f;
            scrollingBackground[2].DrawLayer = .97f;
            //scrollingBackground[3].DrawLayer = .96f;
            //scrollingBackground[4].DrawLayer = .95f;
            //scrollingBackground[5].DrawLayer = .94f;
        }

        public override void OnUpdate(GameTime gameTime)
        {
            if (circleSquad.Count > 0)
            {
                foreach (Enemy e in circleSquad)
                {
                    if (!circleSquadLeaving && e.CustomValue2 > 200)
                    {
                        e.CustomValue2 -= 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if (circleSquadLeaving && e.CustomValue2 < 1500)
                    {
                        e.CustomValue2 += 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (e.CustomValue2 >= 1500)
                        {
                            e.Destroy();
                        }
                    }

                    e.Rotation += e.CustomValue3 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    float angleOfRotation = e.Rotation - (float)Math.PI;
                    float x = (float)Math.Cos(angleOfRotation) * e.CustomValue2;
                    float y = (float)Math.Sin(angleOfRotation) * e.CustomValue2;
                    e.Center = circleSquadOrigin + new Vector2(x, y);
                }
            }
        }

        public override IEnumerator<float> LevelScript()
        {
            manager.thisScene.fader.LerpColor(Color.Transparent, 1f);
            Enemy e;

            AudioManager.PlaySong(Level5Theme, false, .5f);

            yield return 2f;
            TitleShown = true;
            manager.thisScene.LerpTitleColor(Color.White, 1.5f);
            yield return 1.5f;

            yield return 2.5f;

            manager.thisScene.LerpTitleColor(Color.Transparent, 1f);
            yield return 1f;
            TitleShown = false;

            List<Enemy> tortoiseRaid = new List<Enemy>();

            // Spawn the initial turtle raid.
            e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(100, -40), (float)(Math.PI / 8f) * 3, 30f);
            e.CustomValue1 = .6f;
            e.CustomValue2 = 45;
            e.CustomValue3 = .06f;
            e.CustomValue4 = 2.8f;
            e.LerpVelocity(0f, 10f);
            scriptManager.Execute(TortoiseBarage, e);
            tortoiseRaid.Add(e);

            e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(600, -40), (float)(Math.PI / 8f) * 5, 30f);
            e.CustomValue1 = .6f;
            e.CustomValue2 = 45;
            e.CustomValue3 = .06f;
            e.CustomValue4 = 2.8f;
            e.LerpVelocity(0f, 10f);
            scriptManager.Execute(TortoiseBarage, e);
            tortoiseRaid.Add(e);

            e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(-40, 200), (float)(Math.PI / 8f) * 1, 30f);
            e.CustomValue1 = 1.2f;
            e.CustomValue2 = 45;
            e.CustomValue3 = .06f;
            e.CustomValue4 = 2.8f;
            e.LerpVelocity(0f, 10f);
            scriptManager.Execute(TortoiseBarage, e);
            tortoiseRaid.Add(e);

            e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(740, 200), (float)(Math.PI / 8f) * 7, 30f);
            e.CustomValue1 = 1.2f;
            e.CustomValue2 = 45;
            e.CustomValue3 = .06f;
            e.CustomValue4 = 2.8f;
            e.LerpVelocity(0f, 10f);
            scriptManager.Execute(TortoiseBarage, e);
            tortoiseRaid.Add(e);

            // Time remaining: 148 seconds.

            yield return 4f;

            // Time remaining: 144 seconds.

            for(int i = 0; i < 8; i++)
            {
                int x;
                if(i % 2 == 0)
                    x = 200;
                else
                    x = 500;

                e = SpawnEnemy(EnemyType.Slicer, new Vector2(x, -40));
                e.Rotation = (float)Math.PI/2f;
                e.Velocity = 110f;
                scriptManager.Execute(SlicerSimpleShot, e);

                yield return .75f;
            }

            // Time remaining: 138 seconds. 

            yield return 2f;

            // Time remaining: 136 seconds. 

            for(int i = 0; i < 6; i++)
            {
                e = SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(600, -40), (float)Math.PI/2f, 130f);
                scriptManager.Execute(SeekerDragonfly, e);
                yield return .5f;
            }

            // Time remaining: 133

            e = SpawnEnemy(EnemyType.Komodo, new Vector2(250, -40));
            e.Rotation = (float)Math.PI / 2f;
            e.Velocity = 45f;
            e.LerpVelocity(0f, 10f);
            scriptManager.Execute(FirstKomodo, e);

            Enemy komodo1 = e;

            if (tortoiseRaid.Count > 0)
            {
                foreach(Tortoise t in tortoiseRaid)
                {
                    scriptManager.AbortObjectScripts(t);
                    t.Rotation = VectorMathHelper.GetAngleTo(manager.thisScene.player.InnerHitbox.Center, t.Center);
                    t.LerpVelocity(100f, 3f);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                e = SpawnEnemyAtAngle(EnemyType.Dragonfly, new Vector2(600, -40), (float)Math.PI / 2f, 130f);
                scriptManager.Execute(SeekerDragonfly, e);
                yield return .5f;
            }

            // Time remaining 127 (adjust for adding 3 seconds at the beginning)

            yield return 7f;

            // Time remaining 120 seconds

            for (int i = 0; i < 10; i++)
            {
                float x = 0;
                float angle = 0f;
                if (i % 2 == 0)
                {
                    x = -30;
                    angle = (float)(Math.PI / 5f) * 1;
                }
                else
                {
                    x = 730;
                    angle = (float)(Math.PI / 5f) * 4;
                }

                e = SpawnEnemyAtAngle(EnemyType.Slicer, new Vector2(x, 50), angle, 100f);
                scriptManager.Execute(OneSpreadShotSlicer, e);

                yield return .5f;
            }

            if (komodo1 != null && komodo1.Health > 0)
            {
                // Komodo is alive, so have it leave.
                scriptManager.AbortObjectScripts(komodo1);
                komodo1.Rotation = (float)Math.PI / 2f;
                komodo1.LerpVelocity(100f, 4f);
            }

            // Time remaining: 115 seconds

            yield return 4f;

            Phantom phantom1 = SpawnEnemy(EnemyType.Phantom, new Vector2(325, 50)) as Phantom;
            scriptManager.Execute(Phantom1Script, phantom1);

            // Time remaining: 111 seconds

            scrollingBackground[0].LerpSpeed(scrollingBackground[0].layerSpeed * 3, 4f);
            scrollingBackground[1].LerpSpeed(scrollingBackground[1].layerSpeed * 3, 4f);
            scrollingBackground[2].LerpSpeed(scrollingBackground[2].layerSpeed * 3, 4f);
            //scrollingBackground[3].LerpSpeed(scrollingBackground[3].layerSpeed * 3, 4f);
            //scrollingBackground[4].LerpSpeed(scrollingBackground[4].layerSpeed * 3, 4f);
            //scrollingBackground[5].LerpSpeed(scrollingBackground[5].layerSpeed * 3, 4f);
            
            scrollingBackground[0].LerpColor(Color.GreenYellow, 4f);

            yield return 30f;

            // Time remaining: 81 seconds

            float timeRemaining = 20f;

            int spawns = 0;
            while (timeRemaining > 0)
            {
                timeRemaining -= .5f;
                if (phantom1 == null || phantom1.Health <= 0)
                {
                    int x;
                    if (spawns % 2 == 0)
                        x = 200;
                    else
                        x = 500;

                    spawns++;

                    e = SpawnEnemy(EnemyType.Slicer, new Vector2(x, -40));
                    e.Rotation = (float)Math.PI / 2f;
                    e.Velocity = 110f;
                    scriptManager.Execute(SlicerSimpleShot, e);
                }
                yield return .5f;
            }
            
            // Time remaining: 61

            if (phantom1 != null && phantom1.Health > 0)
            {
                // Have the phantom leave if it is still alive.
                scriptManager.AbortObjectScripts(phantom1);
                scriptManager.Execute(DisposePhantom, phantom1);
            }

            yield return 1f;

            // Time remaining 60;

            // Now bring in the circle squad!
            circleSquadOrigin = manager.thisScene.player.InnerHitbox.Center;
            for (int i = 0; i < 18; i++)
            {
                float x = (float)Math.Cos((float)(Math.PI * 2 / 18f) * i) * 1000;
                float y = (float)Math.Sin((float)(Math.PI * 2 / 18f) * i) * 1000;
                e = SpawnEnemyAtAngle(EnemyType.Komodo, circleSquadOrigin + new Vector2(x, y), VectorMathHelper.GetAngleTo(circleSquadOrigin + new Vector2(x, y), circleSquadOrigin), 0f);
                e.DeletionBoundary = new Vector2(1000, 1000);

                e.CustomValue1 = i;

                // Store its start distance from the player
                e.CustomValue2 = 1000f;
                // Store its rate of angle change
                e.CustomValue3 = .8f;

                circleSquad.Add(e);
                scriptManager.Execute(CircleSquadKomodo, e);
            }

            yield return 30f;

            foreach (Enemy k in circleSquad)
            {
                scriptManager.AbortObjectScripts(k);
                scriptManager.Execute(CircleSquadDispose, k);
            }
            
            // Time remaining 25

            yield return 2f;

            // Time remaining 23

            for (int i = 0; i < 12; i++)
            {
                int x = 0;
                float angle = 0f;
                if (i % 2 == 0)
                {
                    x = 300;
                    angle = (float)Math.PI / 6f * 4;
                }
                else
                {
                    x = 450;
                    angle = (float)Math.PI / 6f * 2;
                }

                e = SpawnEnemyAtAngle(EnemyType.Tortoise, new Vector2(x, -40), angle, 80f);
                e.LerpVelocity(40f, 4f);
                scriptManager.Execute(FocusFireTortoise, e);

                yield return 1f;
            }

            // Time remaining 11

            yield return 8f;

            scrollingBackground[0].LerpSpeed(scrollingBackground[0].layerSpeed / 3, 4f);
            scrollingBackground[1].LerpSpeed(scrollingBackground[1].layerSpeed / 3, 4f);
            scrollingBackground[2].LerpSpeed(scrollingBackground[2].layerSpeed / 3, 4f);
            //scrollingBackground[3].LerpSpeed(scrollingBackground[3].layerSpeed / 3, 4f);
            //scrollingBackground[4].LerpSpeed(scrollingBackground[4].layerSpeed / 3, 4f);
            //scrollingBackground[5].LerpSpeed(scrollingBackground[5].layerSpeed / 3, 4f);

            scrollingBackground[0].LerpColor(Color.Red, 5.7f);

            yield return 7f;

            manager.thisScene.PlayBossWarning();
            yield return 6.6f;
            //SPAWN THE BOSS

            Boss boss = new FinalBoss(manager.thisScene, new Vector2(250f, -450f));
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
            yield return 5f;

            scriptManager.Execute(manager.thisScene.ShowJammedWarning);
            yield return 6f;
            AudioManager.PlaySong(FinalAttackTheme, false);
            // Dispose of the health bar.

            // Do the final phase... then destroy the enemy!
            yield return 64f;

            AudioManager.PlaySoundEffect(GameScene.Explosion4Sound, 1f);
            AudioManager.PlaySoundEffect(GameScene.Explosion1Sound, .6f);
            manager.thisScene.fader.LerpColor(Color.White, .3f);
            yield return .3f;
            boss.Destroy();

            yield return 1.5f;
            manager.thisScene.fader.LerpColor(Color.Black, 4f);

            yield return 4f;
            // THE END
            manager.thisScene.Game.ChangeScene(new TitleScene(manager.thisScene.Game));
        }

        // CustomValue1: Initial wait time
        // CustomValue2: Number of shots per barrage
        // CustomValue3: Wait time between shots
        // CustomValue4: Wait time between barrages
        public IEnumerator<float> TortoiseBarage(GameObject go)
        {
            // Wait custom value 1 seconds
            yield return go.CustomValue1;

            BulletEmitter be = new BulletEmitter(go, go.Center);
            while (true)
            {
                int shots = 0;
                while (shots < go.CustomValue2)
                {
                    shots++;
                    be.Center = go.Center;
                    be.FireBulletCluster(go.Rotation, 1, 35f, 140f, 40f, Color.DeepSkyBlue);
                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .12f);
                    yield return go.CustomValue3;
                }

                yield return go.CustomValue4;
            }
        }

        public IEnumerator<float> SlicerSimpleShot(GameObject go)
        {
            yield return .6f;

            while (true)
            {
                new BulletEmitter(go, go.Center, true).FireBullet(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center), 140f, Color.Orange);
                yield return 1.5f;
            }
        }

        public IEnumerator<float> OneSpreadShotSlicer(GameObject go)
        {
            yield return .7f;
            new BulletEmitter(go, go.Center, true).FireBulletSpread(go.Rotation, 3, 80f, 140f, Color.Orange);
            AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .5f);
        }

        public IEnumerator<float> SeekerDragonfly(GameObject go)
        {
            yield return 1.5f;

            go.Rotation = MathHelper.WrapAngle(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center));
        }

        public IEnumerator<float> FirstKomodo(GameObject go)
        {
            yield return 2f;

            BulletEmitter be = new BulletEmitter(go, go.Center);
            be.LockedToParentPosition = true;
            be.LockPositionOffset = Vector2.Zero;

            while(true)
            {
                int shots = 0;

                while (shots < 15)
                {
                    be.FireBulletSpread(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center), 5, 70f, 220f, Color.Orange);
                    AudioManager.PlaySoundEffect(GameScene.Shot3Sound, .5f);
                    shots++;
                    yield return .3f;
                }

                yield return .5f;
                go.LerpPosition(new Vector2(500, go.Center.Y), 1.5f);
                yield return 2f;

                shots = 0;
                float turnMod = 1;
                while (shots < 20)
                {
                    if (shots > 10)
                        turnMod = -1f;

                    be.FireBulletExplosion(15, 120f, Color.DeepSkyBlue);
                    AudioManager.PlaySoundEffect(GameScene.Shot6Sound, .5f);
                    be.Rotation += .1f * turnMod;

                    shots++;
                    yield return .1f;
                }

                yield return .5f;
                go.LerpPosition(new Vector2(300, go.Center.Y), 1.5f);
                yield return 2f;
            }
        }

        public IEnumerator<float> Phantom1Script(GameObject go)
        {
            Random rand = new Random();

            Phantom phantom = (Phantom)go;
            List<Vector2> randomTeleportLocations = new List<Vector2>()
            {
                new Vector2(325, 250),
                new Vector2(125, 50),
                new Vector2(400, 50),
                new Vector2(125, 250),
                new Vector2(400, 250),
                new Vector2(400, 450),
                new Vector2(125, 450)
            };

            while (true)
            {
                scriptManager.Execute(phantom.PhaseIn, go);
                yield return 2f;

                int shots = 0;
                while (shots < 50)
                {
                    phantom.leftInnerWingCannon.FireBulletSpread((float)Math.PI / 2f, 6, 160, 200f, Color.DeepSkyBlue, BulletType.CircleSmall);
                    phantom.rightInnerWingCannon.FireBulletSpread((float)Math.PI / 2f, 6, 160, 200f, Color.DeepSkyBlue, BulletType.CircleSmall);
                    
                    if (shots % 4 == 0)
                    {
                        phantom.mainEmitter.Rotation += .45f;
                        phantom.mainEmitter.FireBulletExplosion(25, 140f, Color.DeepSkyBlue);
                        AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .8f);
                    }

                    yield return .09f;
                    shots++;
                }

                scriptManager.Execute(phantom.PhaseOut, phantom);
                yield return 2f;
                phantom.Position = randomTeleportLocations[new Random().Next(0, randomTeleportLocations.Count)];
                scriptManager.Execute(phantom.PhaseIn, phantom);
                yield return 2f;
                int cycles = 0;

                while (cycles < 4)
                {
                    shots = 0;

                    Vector2 target = manager.thisScene.player.InnerHitbox.Center;


                    while (shots < 15)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot2Sound, .3f);
                        phantom.leftWingCannon.FireBullet(VectorMathHelper.GetAngleTo(phantom.leftWingCannon.Center, target), 350f, Color.Lerp(Color.White, Color.Orange, .7f));
                        phantom.rightWingCannon.FireBullet(VectorMathHelper.GetAngleTo(phantom.rightWingCannon.Center, target), 350f, Color.Lerp(Color.White, Color.Orange, .7f));
                        shots++;
                        yield return .06f;
                    }


                    shots = 0;
                    yield return .2f;

                    while (shots < 10)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot5Sound, .25f);
                        Bullet newBullet;
                        newBullet = phantom.leftInnerWingCannon.FireBullet(VectorMathHelper.GetAngleTo(phantom.leftInnerWingCannon.Center, manager.thisScene.player.InnerHitbox.Center), 100f, Color.DeepSkyBlue);
                        newBullet.LerpRotation(VectorMathHelper.GetAngleTo(newBullet.Center, manager.thisScene.player.InnerHitbox.Center) + (((float)rand.NextDouble() * 2) - 1f) * .7f, 4f);
                        newBullet = phantom.rightInnerWingCannon.FireBullet(VectorMathHelper.GetAngleTo(phantom.rightInnerWingCannon.Center, manager.thisScene.player.InnerHitbox.Center), 100f, Color.DeepSkyBlue);
                        newBullet.LerpRotation(VectorMathHelper.GetAngleTo(newBullet.Center, manager.thisScene.player.InnerHitbox.Center) + (((float)rand.NextDouble() * 2) - 1f) * .7f, 4f);
                        shots++;
                        yield return .03f;
                    }

                    shots = 0;
                    cycles++;
                    yield return .5f;
                }

                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f);
                phantom.mainEmitter.FireBulletExplosion(25, 225f, Color.Lerp(Color.White, Color.Orange, .7f));
                yield return .1f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f);
                phantom.mainEmitter.FireBulletExplosion(24, 225f, Color.Lerp(Color.White, Color.Orange, .7f));
                yield return .1f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f);
                phantom.mainEmitter.FireBulletExplosion(23, 225f, Color.Lerp(Color.White, Color.Orange, .7f));
                yield return .1f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f);
                phantom.mainEmitter.FireBulletExplosion(22, 225f, Color.Lerp(Color.White, Color.Orange, .7f));
                yield return .1f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f);
                phantom.mainEmitter.FireBulletExplosion(21, 225f, Color.Lerp(Color.White, Color.Orange, .7f));
                yield return .1f;

                scriptManager.Execute(phantom.PhaseOut, phantom);
                yield return 2f;
                phantom.Position = new Vector2(325, 50);
            }
        }

        public IEnumerator<float> DisposePhantom(GameObject go)
        {
            Phantom phantom = (Phantom)go;

            scriptManager.Execute(phantom.PhaseOut, go);
            yield return 2f;
            phantom.Destroy();
        }

        public IEnumerator<float> CircleSquadKomodo(GameObject go)
        {
            if (go.CustomValue2 < 200)
                go.CustomValue2 = 200;

            yield return 2f + (go.CustomValue1 * 1.1f);

            while (true)
            {
                if (Vector2.Distance(manager.thisScene.player.InnerHitbox.Center, circleSquadOrigin) > go.CustomValue2)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot6Sound, .8f);
                    new BulletEmitter(go, go.Center, true).FireBulletCluster(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center), 15, 35f, 200f, 50f, Color.DeepSkyBlue);
                }
                else
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot3Sound, .3f);
                    new BulletEmitter(go, go.Center, true).FireBullet(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center), 100f, Color.DeepSkyBlue);
                }
                yield return 2f;
            }
        }

        public IEnumerator<float> CircleSquadDispose(GameObject go)
        {
            circleSquadLeaving = true;
            yield return .0f;
        }

        public IEnumerator<float> FocusFireTortoise(GameObject go)
        {
            yield return .6f;

            while (true)
            {
                int shots = 0;
                while (shots < 60f)
                {
                    shots++;
                    new BulletEmitter(go, go.Center, true).FireBulletCluster(VectorMathHelper.GetAngleTo(go.Center, manager.thisScene.player.InnerHitbox.Center), 1, 10f, 150f, 25f, Color.DeepSkyBlue);
                    if (go.Center.X > manager.thisScene.ScreenArea.X && go.Center.X < manager.thisScene.ScreenArea.X + manager.thisScene.ScreenArea.Width && go.Center.Y > manager.thisScene.ScreenArea.Y && go.Center.Y < manager.thisScene.ScreenArea.Y + manager.thisScene.ScreenArea.Height)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .05f);
                    }
                    yield return .03f;
                }
                yield return 3f;
            }
        }
    }
}