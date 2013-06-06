using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AsteroidRebuttal;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Scripting;

namespace AsteroidRebuttal.Enemies.Bosses
{
    public class FinalBoss : Boss
    {
        BulletEmitter mainEmitter;
        List<BulletEmitter> ring1Emitters;
        List<BulletEmitter> ring2Emitters;

        List<Bullet> rotatingBullets;

        Enemy ring1;
        Enemy ring2;
        Enemy topLayer;

        int phase = 1;

        float nextSpreadShotTime = 0f;

        public FinalBoss(GameScene newScene, Vector2 newPos = new Vector2()) : base(newScene, newPos)
        {
            thisScene = newScene;
            Center = newPos;

            Initialize();
        }

        public override void Initialize()
        {
            Health = 1800;
            DrawLayer = .425f;

            // Get the actual origin.
            Origin = new Vector2(255.5f, 234.5f);
            Hitbox = new Circle(Center, 120f);
            Texture = boss5BaseTexture;
            DeletionBoundary = new Vector2(1500, 1500);

            Color = Color.White;

            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2, 3 };

            InitializeParts();

            scriptManager = thisScene.scriptManager;
            scriptManager.Execute(Phase1Script, this);

            OnOuterCollision += CollisionHandling;

            PhaseChangeValues = new List<int>()
            {
                1350,
                1100,
                750,
                450
            };
            MaxHealth = (int)Health;

            base.Initialize();
        }

        public void InitializeParts()
        {
            mainEmitter = new BulletEmitter(this, Origin, false);
            mainEmitter.LockedToParentPosition = true;
            mainEmitter.LockPositionOffset = Vector2.Zero;

            ring1 = new Enemy(thisScene, Origin);
            ring1.Initialize();
            ring1.DeletionBoundary = DeletionBoundary;
            ring1.DrawAtTrueRotation = true;
            ring1.Color = Color.White;
            ring1.Origin = Origin;
            ring1.SetParent(this);
            ring1.SetTexture(boss5Ring1Texture);
            ring1.LockedToParentPosition = true;
            ring1.Center = this.Center;
            ring1.DrawLayer = .32f;

            ring2 = new Enemy(thisScene, Origin);
            ring2.Initialize();
            ring2.DeletionBoundary = DeletionBoundary;
            ring2.DrawAtTrueRotation = true;
            ring2.Color = Color.White;
            ring2.Origin = Origin;
            ring2.SetParent(this);
            ring2.SetTexture(boss5Ring2Texture);
            ring2.LockedToParentPosition = true;
            ring2.Center = this.Center;
            ring2.DrawLayer = .31f;

            ring1Emitters = new List<BulletEmitter>()
            {
                new BulletEmitter(ring1, new Vector2(130, 175)),
                new BulletEmitter(ring1, new Vector2(130, 294)),
                new BulletEmitter(ring1, new Vector2(381, 175)),
                new BulletEmitter(ring1, new Vector2(381, 294))
            };

            ring2Emitters = new List<BulletEmitter>()
            {
                new BulletEmitter(ring2, new Vector2(195, 110)),
                new BulletEmitter(ring2, new Vector2(117, 234.5f)),
                new BulletEmitter(ring2, new Vector2(195, 359)),
                new BulletEmitter(ring2, new Vector2(316, 110)),
                new BulletEmitter(ring2, new Vector2(394, 234.5f)),
                new BulletEmitter(ring2, new Vector2(316, 359))
            };

            // Setup the ring emitter properties.
            foreach (BulletEmitter be in ring1Emitters)
            {
                be.Center = Center + (be.Center - Origin);
                be.DeletionBoundary = new Vector2(99999, 99999);
                be.CustomValue1 = Vector2.Distance(Center, be.Center);
                be.Rotation = VectorMathHelper.GetAngleTo(Center, be.Center);
            }

            // Setup the ring emitter properties.
            foreach (BulletEmitter be in ring2Emitters)
            {
                be.Center = Center + (be.Center - Origin);
                be.DeletionBoundary = new Vector2(99999, 99999);
                be.CustomValue1 = Vector2.Distance(Center, be.Center);
                be.Rotation = VectorMathHelper.GetAngleTo(Center, be.Center);
            }
            
            topLayer = new Enemy(thisScene, Origin);
            topLayer.Initialize();
            topLayer.DeletionBoundary = DeletionBoundary;
            topLayer.Color = Color.White;
            topLayer.Origin = Origin;
            topLayer.SetTexture(boss5TopTexture);
            topLayer.SetParent(this);
            topLayer.LockedToParentPosition = true;
            topLayer.Center = this.Center;
            topLayer.DrawLayer = .3f;

            rotatingBullets = new List<Bullet>();
        }

        public override void Update(GameTime gameTime)
        {
            // Move the subemitters with the rotation of the rings, and ensure they are facing outwards.
            foreach (BulletEmitter be in ring1Emitters)
            {
                // Update rotation.
                be.Rotation += ring1.AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 2;

                Vector2 newPos = new Vector2();
                newPos.X = (float)Math.Cos(be.Rotation) * be.CustomValue1;
                newPos.Y = (float)Math.Sin(be.Rotation) * be.CustomValue1;

                be.Center = Center + newPos;
            }

            foreach (BulletEmitter be in ring2Emitters)
            {
                // Update rotation.
                be.Rotation += ring2.AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 2;

                Vector2 newPos = new Vector2();
                newPos.X = (float)Math.Cos(be.Rotation) * be.CustomValue1;
                newPos.Y = (float)Math.Sin(be.Rotation) * be.CustomValue1;

                be.Center = Center + newPos;
            }

            foreach (Bullet b in rotatingBullets)
            {
                Vector2 shotOrigin = new Vector2(b.CustomValue3, b.CustomValue4);
                // Update rotation.
                b.Rotation += (b.CustomValue1 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                // Custom Value 1 is the angular velocity of rotation

                b.CustomValue2 = Vector2.Distance(b.Center, shotOrigin) + (b.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

                Vector2 newPos = new Vector2();
                newPos.X = (float)Math.Cos(b.Rotation) * b.CustomValue2;
                newPos.Y = (float)Math.Sin(b.Rotation) * b.CustomValue2;

                b.Center = shotOrigin + newPos;
            }


            if (phase == 1 && Health < 1350)
            {
                phase = 2;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase2Script, this);
            }

            if (phase == 2 && Health < 1100)
            {
                phase = 3;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase3Script, this);
            }

            if (phase == 3 && Health < 750)
            {
                phase = 4;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase4Script, this);
            }

            if (phase == 4 && Health < 450)
            {
                phase = 5;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase5Script, this);
            }

            if (phase == 5 && Health < 0)
            {
                phase = 6;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(FinalPhase, this);

                // Fake Explode the boss!!
                foreach (GameObject b in thisScene.gameObjects.FindAll(x => x is Bullet))
                {
                    scriptManager.AbortObjectScripts(b);
                    b.Phasing = true;
                    b.LerpColor(Color.Transparent, 1f);
                }

                scriptManager.Execute(thisScene.BossExplosion, this);
            }

            base.Update(gameTime);
        }

        public IEnumerator<float> Phase1Script (GameObject thisObject)
        {
            LerpPosition(new Vector2(350f, 220f), 10f);
            yield return 5f;
            Vulnerable = true;

            int cycles = 0;
            while (cycles < 4)
            {
                float rotateSpeed = 0f;
                if (cycles % 2 == 0)
                    rotateSpeed = .1f;
                else
                    rotateSpeed = -.1f;

                mainEmitter.Rotation = 0f;

                AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .6f, -.40f);
                foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 120f, Color.DeepSkyBlue))
                {
                    b.LerpVelocity(90f, 1f);
                }

                yield return .4f;

                mainEmitter.Rotation += rotateSpeed;
                AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .6f, -.40f);
                foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 120f, Color.DeepSkyBlue))
                {
                    b.LerpVelocity(90f, 1f);
                }

                yield return .4f;

                mainEmitter.Rotation += rotateSpeed;
                AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .6f, -.40f);
                foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 120f, Color.DeepSkyBlue))
                {
                    b.LerpVelocity(90f, 1f);
                }

                yield return .4f;

                mainEmitter.Rotation += rotateSpeed;
                AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .6f, -.40f);
                foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 120f, Color.DeepSkyBlue))
                {
                    b.LerpVelocity(90f, 1f);
                }

                yield return .4f;

                mainEmitter.Rotation += rotateSpeed;
                AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .6f, -.40f);
                foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 120f, Color.DeepSkyBlue))
                {
                    b.LerpVelocity(90f, 1f);
                }

                cycles++;
                yield return 4f;
            }

            LerpPosition(new Vector2(350f, 125f), 10f);
            yield return 1f;

            ring1.LerpAngularVelocity(.3f, 3f);
            yield return 1f;
            ring2.LerpAngularVelocity(-.45f, 3f);
            yield return 2f;

            int moveCount = 0;
            Vector2[] movePositions = new Vector2[]
            {
                new Vector2(200, 125f),
                new Vector2(350f, 125f),
                new Vector2(500f, 125f),
                new Vector2(350f, 125f)
            };

            // Cycle the power!
            while (true)
            {
                
                float nextMoveTime = currentGameTime + 7f;

                while (currentGameTime < nextMoveTime)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot5Sound, .45f, .40f);
                    foreach (BulletEmitter be in ring1Emitters)
                    {
                        be.FireBullet(180, Color.Red);
                    }
                    foreach (BulletEmitter be in ring2Emitters)
                    {
                        be.FireBullet(140, Color.DeepSkyBlue);
                    }
                    yield return .2f;
                }

                LerpPosition(movePositions[moveCount], 2f);
                scriptManager.Execute(Phase1MoveVolley, this);
                moveCount++;

                if (moveCount >= movePositions.Length)
                    moveCount = 0;
            }
        }

        public IEnumerator<float> Phase1MoveVolley(GameObject go)
        {
            mainEmitter.Rotation = 0;
            AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, .25f);
            mainEmitter.FireBulletExplosion(40, 160, Color.Lerp(Color.White, Color.Orange, .4f));
            yield return .2f;

            mainEmitter.Rotation += .2f;
            AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, .25f);
            mainEmitter.FireBulletExplosion(40, 160, Color.Lerp(Color.White, Color.Orange, .4f));
            yield return .2f;

            mainEmitter.Rotation += .2f;
            AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, .25f);
            mainEmitter.FireBulletExplosion(40, 160, Color.Lerp(Color.White, Color.Orange, .4f));
            yield return .2f;
        }

        public IEnumerator<float> Phase2Script(GameObject go)
        {
            LerpPosition(new Vector2(350f, 220f), 5f);
            ring1.LerpAngularVelocity(0, 3f);
            ring2.LerpAngularVelocity(0, 3f);
            yield return 3f;
            ring1.LerpAngularVelocity(-1f, 3f);
            ring2.LerpAngularVelocity(1f, 3f);
            yield return 3f;

            // Do the cool pattern!!
            int cycles = 0;
            while (true)
            {
                int shots = 0;

                while (shots < 40)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot6Sound, .45f, 0f);

                    foreach (BulletEmitter be in ring1Emitters)
                    {
                        Bullet b = be.FireBullet(20f, Color.Red);
                        b.CustomValue1 = -250f;
                        scriptManager.Execute(Phase2BulletSwarm, b);
                    }

                    foreach (BulletEmitter be in ring2Emitters)
                    {
                        Bullet b = be.FireBullet(20f, Color.DeepSkyBlue);
                        b.CustomValue1 = 150f;
                        scriptManager.Execute(Phase2BulletSwarm, b);
                    }

                    shots++;
                    yield return .09f;
                }

                yield return 4f;
                ring1.LerpAngularVelocity(ring1.AngularVelocity * -1f, 5f);
                ring2.LerpAngularVelocity(ring2.AngularVelocity * -1f, 5f);
                yield return 2.5f;
                
                // If this is the 5th time the cycle has repeated or more, fire radial bullets.
                if (cycles >= 2)
                    scriptManager.Execute(Phase1MoveVolley, this);

                cycles++;
                yield return 2.5f;
            }
        }

        public IEnumerator<float> Phase2BulletSwarm(GameObject go)
        {
            Bullet thisBullet = (Bullet)go;
            yield return 3f;

            AudioManager.PlaySoundEffect(GameScene.Shot8Sound, .4f, .5f);
            thisBullet.LerpVelocity(thisBullet.CustomValue1, 3f);
        }

        public IEnumerator<float> Phase3Script(GameObject go)
        {
            LerpPosition(new Vector2(350f, 140f), 5f);
            ring1.LerpAngularVelocity(0, 3f);
            ring2.LerpAngularVelocity(0, 3f);
            yield return 3f;
            ring1.LerpAngularVelocity(-.35f, 3f);
            ring2.LerpAngularVelocity(-.2f, 3f);
            yield return 3f;


            nextSpreadShotTime = currentGameTime + 1f;
            // Execute the movement script.
            scriptManager.Execute(Phase3MovementScript, this);

            // Execute the ring 1 script
            scriptManager.Execute(Phase3Ring1Script, this);

            // Execute the ring 2 script.
            scriptManager.Execute(Phase3Ring2Script, this);

            while(true)
            {
                if (nextSpreadShotTime < currentGameTime)
                {
                    int shots = 0;
                    
                    // Fire a slow moving, rotating spread shot from the center emitter.
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .7f, 1f);
                    foreach (Bullet b in mainEmitter.FireBulletExplosion(40, 50f, Color.Lerp(Color.White, Color.Orange, .4f)))
                    {
                        if (shots % 2 == 0)
                            b.AngularVelocity = .2f;
                        else
                            b.AngularVelocity = -.2f;
                        
                        b.LerpVelocity(100f, 3f);
                    }

                    shots++;
                    nextSpreadShotTime += 1.5f;
                }

                yield return .1f;
            }

        }

        public IEnumerator<float> Phase3Ring1Script(GameObject go)
        {
            int inrangeof = 0;
            float angleToPlayer;
            float thisRotation;

            while (true)
            {
                inrangeof = 0;

                foreach (BulletEmitter be in ring1Emitters)
                {
                    if (currentGameTime < be.CustomValue2)
                    {
                        continue;
                    }

                    angleToPlayer = VectorMathHelper.GetAngleTo(Center, thisScene.player.InnerHitbox.Center);
                    thisRotation = VectorMathHelper.GetAngleTo(Center, be.Center);

                    if (thisRotation < (angleToPlayer + .1f) && thisRotation > (angleToPlayer - .1f))
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .7f, .3f);
                        be.FireBulletCluster(be.Rotation, 10, 20f, 100f, 200f, Color.Red);

                        // Stop it from firing for .2 seconds
                        be.CustomValue2 = currentGameTime + .2f;
                        inrangeof++;
                    }
                }

                if (inrangeof > 0)
                {
                    // The player is in range of at least one cannon, so delay the next spreading shot from the center by .7 seconds.
                    nextSpreadShotTime += .7f;
                }

                yield return .03f;
            }
        }

        public IEnumerator<float> Phase3Ring2Script(GameObject go)
        {
            while (true)
            {
                foreach (BulletEmitter be in ring2Emitters)
                {
                    float angleToPlayer = VectorMathHelper.GetAngleTo(Center, thisScene.player.InnerHitbox.Center);
                    float thisRotation = VectorMathHelper.GetAngleTo(Center, be.Center);

                    if (thisRotation < (angleToPlayer + .1f) && thisRotation > (angleToPlayer - .1f))
                    {
                        // Fire a bomb that will explode after a few moments.
                        AudioManager.PlaySoundEffect(GameScene.Shot6Sound, .75f, .5f);
                        Bullet bomb = be.FireBullet(140f, Color.Orange);
                        bomb.LerpVelocity(0f, 2f);
                        scriptManager.Execute(BombExplosion, bomb);
                        
                        // Delay the spread.
                        nextSpreadShotTime += 1.5f;

                    }
                    else
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .5f, 0f);
                        foreach (Bullet b in be.FireBulletSpread(be.Rotation, 4, 25f, 130f, Color.DeepSkyBlue))
                        {
                            b.LerpVelocity(60f, 3f);
                        }
                    }
                }

                yield return 1.2f;
            }
        }

        public IEnumerator<float> BombExplosion(GameObject go)
        {
            yield return 2f;
            AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .9f, 0f);
            BulletEmitter explosion = new BulletEmitter(this, go.Center, true);
            explosion.FireBulletExplosion(20, 110f, Color.Lerp(Color.White, Color.Orange, .7f));
            explosion.FireBulletExplosion(30, 130f, Color.Lerp(Color.White, Color.Orange, .7f), BulletType.CircleSmall);
            go.Destroy();
        }

        public IEnumerator<float> Phase3MovementScript(GameObject go)
        {
            while (true)
            {
                yield return 6f;
                LerpPosition(new Vector2(100f, 140f), 5f);
                yield return 10f;
                LerpPosition(new Vector2(350f, 140f), 5f);
                yield return 10f;
                LerpPosition(new Vector2(500f, 140f), 5f);
                yield return 10f;
                LerpPosition(new Vector2(350f, 140f), 5f);
                yield return 10f;

                LerpPosition(new Vector2(500f, 400f), 15f);
                yield return 20f;
                LerpPosition(new Vector2(100f, 400f), 15f);
                yield return 20f;
                LerpPosition(new Vector2(350f, 140f), 15f);
                
                yield return 10f;
                ring1.LerpAngularVelocity(ring1.AngularVelocity * -1, 10f);
                ring2.LerpAngularVelocity(ring2.AngularVelocity * -1, 10f);
                yield return 10f;
            }
        }

        public IEnumerator<float> Phase4Script(GameObject go)
        {
            ring1.LerpAngularVelocity(-.8f, 3.5f);
            ring2.LerpAngularVelocity(.8f, 3.5f);

            scriptManager.Execute(Phase4MovementScript, this);
            LerpPosition(new Vector2(350f, 140f), 5f);
            yield return 3.5f;

            while (true)
            {
                int shots = 0;

                mainEmitter.Rotation = (float)Math.PI / 2f;

                while (shots < 50)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot5Sound, .35f, -.2f);
                    mainEmitter.Rotation += .1f;
                    mainEmitter.FireBulletExplosion(5, 170f, Color.DeepSkyBlue);
                    shots++;
                    yield return .06f;
                }

                shots = 0;
                mainEmitter.Rotation = 0;

                yield return 1f;

                while (shots < 75)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot5Sound, .35f, .2f);
                    mainEmitter.Rotation -= .138f;
                    mainEmitter.FireBulletExplosion(4, 65f, Color.Red);
                    shots++;
                    yield return .03f;
                }

                yield return .8f;

                shots = 0;
                mainEmitter.Rotation = (float)Math.PI / 2f;

                yield return 1.5f;
            }
        }

        public IEnumerator<float> Phase4MovementScript(GameObject go)
        {
            yield return 6f;
            while (true)
            {
                yield return 10f;

                LerpPosition(new Vector2(100f, 140f), 1.5f);
                yield return 6.5f;
                LerpPosition(new Vector2(350f, 140f), 1.5f);
                yield return 6.5f;
                LerpPosition(new Vector2(500f, 140f), 1.5f);
                yield return 6.5f;
                LerpPosition(new Vector2(350f, 140f), 1.5f);
                yield return 6.5f;

                LerpPosition(new Vector2(350f, 400f), 12f);
                yield return 18f;
                LerpPosition(new Vector2(350f, 90f), 12f);
                yield return 18f;
                //yield return 10f;
                //ring1.LerpAngularVelocity(ring1.AngularVelocity * -1, 10f);
                //ring2.LerpAngularVelocity(ring2.AngularVelocity * -1, 10f);
                //yield return 10f;
            }
        }

        public IEnumerator<float> Phase5Script(GameObject thisObject)
        {
            LerpPosition(new Vector2(350f, 125f), 3f);
            yield return 1f;

            ring1.LerpAngularVelocity(.6f, 3f);
            yield return 1f;
            ring2.LerpAngularVelocity(-.8f, 3f);
            yield return 2f;

            int moveCount = 0;
            Vector2[] movePositions = new Vector2[]
            {
                new Vector2(200, 125f),
                new Vector2(350f, 125f),
                new Vector2(500f, 125f),
                new Vector2(350f, 125f)
            };

            float fireTime = .12f;

            // Cycle the power!
            while (true)
            {
                float nextMoveTime = currentGameTime + 6f;

                while (currentGameTime < nextMoveTime)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot5Sound, .35f, .40f);
                    foreach (BulletEmitter be in ring1Emitters)
                    {
                        //be.FireBullet(200, Color.Red).LerpVelocity(90, 1f);
                        be.FireBullet(110, Color.Red);
                    }
                    foreach (BulletEmitter be in ring2Emitters)
                    {
                        //be.FireBullet(200, Color.DeepSkyBlue).LerpVelocity(60, 1f);
                        be.FireBullet(80, Color.DeepSkyBlue);
                    }
                    yield return fireTime;
                }

                LerpPosition(movePositions[moveCount], 2f);
                scriptManager.Execute(Phase5MoveVolley, this);

                moveCount++;

                if (moveCount >= movePositions.Length)
                    moveCount = 0;
            }
        }

        public IEnumerator<float> Phase5MoveVolley(GameObject go)
        {
            AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, .25f);
            mainEmitter.Rotation = 0;
            mainEmitter.FireBulletExplosion(40, 160, Color.Lerp(Color.White, Color.Orange, .4f));
            yield return .2f;

            AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, .25f);
            mainEmitter.Rotation += .2f;
            mainEmitter.FireBulletExplosion(40, 160, Color.Lerp(Color.White, Color.Orange, .4f));
            yield return .2f;

            AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, .25f);
            mainEmitter.Rotation += .2f;
            mainEmitter.FireBulletExplosion(40, 160, Color.Lerp(Color.White, Color.Orange, .4f));
            yield return .2f;

            foreach (BulletEmitter be in ring2Emitters)
            {
                AudioManager.PlaySoundEffect(GameScene.Shot6Sound, 1f, .25f);
                Bullet b = be.FireBullet(200, Color.Orange);
                b.LerpVelocity(0f, 1.5f);
                scriptManager.Execute(Phase5BombExplosion, b);
            }
        }

        public IEnumerator<float> Phase5BombExplosion(GameObject go)
        {
            yield return 2f;
            AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .9f, .25f);
            BulletEmitter explosion = new BulletEmitter(this, go.Center, true);
            explosion.FireBulletExplosion(10, 110f, Color.Lerp(Color.White, Color.Orange, .7f));
            explosion.FireBulletExplosion(15, 130f, Color.Lerp(Color.White, Color.Orange, .7f), BulletType.CircleSmall);
            go.Destroy();
        }

        public IEnumerator<float> FinalPhase(GameObject go)
        {
            // Do explosions and lerp the position dramatically.
            LerpPosition(new Vector2(350f, 75f), 5f);
            ring1.LerpAngularVelocity(0f, 3f);
            ring2.LerpAngularVelocity(0f, 4f);
            yield return 5f;
            // Lerp the position and disable the player's shot 
            thisScene.player.CanFire = false;
            thisScene.PlayerCanFire = false;
            ring1.LerpAngularVelocity(2.15f, 3f);
            ring2.LerpAngularVelocity(-3.25f, 3f);
            yield return 6f;

            // Start a 60 second countdown timer.
            float finalPhaseEndTime = currentGameTime + 58f;
            float nextShotTime = currentGameTime + 2.5f;

            //BEGIN!
            scriptManager.Execute(FinalPhaseMovement, this);

            while (finalPhaseEndTime - currentGameTime > 42f)
            {
                Bullet b;
                AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .25f, .25f);
                foreach (BulletEmitter be in ring2Emitters)
                {
                    b = be.FireBullet(270f, Color.DeepSkyBlue);
                    b.LerpVelocity(200f, 1.5f);
                }

                if (currentGameTime >= nextShotTime && finalPhaseEndTime - currentGameTime > 47f)
                {
                    foreach(BulletEmitter be in ring1Emitters)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot6Sound, .15f, 0f);
                        be.FireBulletSpread(be.Rotation, 5, 90f, 40f, Color.Red);
                        nextShotTime = currentGameTime + .7f;
                    }
                }

                yield return .03f;
            }

            yield return 2f;

            float rotationSpeed = .4f;
            float phase2StartTime = currentGameTime;

            while(finalPhaseEndTime - currentGameTime  > 22f)
            {
                mainEmitter.Rotation = VectorMathHelper.GetRandom();
                // Do some movementing while this happens
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .9f, .5f);

                foreach (Bullet b in mainEmitter.FireBulletExplosion(15, 100f, Color.DeepSkyBlue))
                {
                    b.CustomValue1 = rotationSpeed;
                    b.CustomValue2 = 0f;
                    b.CustomValue3 = mainEmitter.Center.X;
                    b.CustomValue4 = mainEmitter.Center.Y;

                    rotatingBullets.Add(b);
                }

                foreach (Bullet b in mainEmitter.FireBulletExplosion(15, 100f, Color.Red))
                {
                    b.CustomValue1 = rotationSpeed * -1f;
                    b.CustomValue2 = 0f;
                    b.CustomValue3 = mainEmitter.Center.X;
                    b.CustomValue4 = mainEmitter.Center.Y;

                    rotatingBullets.Add(b);
                }

                yield return .5f;
            }

            yield return 2f;

            int shots = 0;
            float waitTime = .06f;
            nextShotTime = currentGameTime + 2f;
            
            rotationSpeed = .3f;

            while (finalPhaseEndTime - currentGameTime > 0f)
            {
                if (shots < 10)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot3Sound, .3f, .5f);
                    Bullet[] tempbullets = mainEmitter.FireBulletExplosion(35, 40, Color.Orange);
                    foreach (Bullet b in tempbullets)
                    {
                        b.LerpVelocity(260f, 3.5f);
                    }

                    shots++;
                    waitTime = .06f;
                }
                else
                {
                    mainEmitter.Rotation = VectorMathHelper.GetRandom();
                    waitTime = .5f;
                    shots = 0;
                }

                if (currentGameTime > nextShotTime)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .9f, .5f);

                    foreach (Bullet b in mainEmitter.FireBulletExplosion(10, 60f, Color.DeepSkyBlue))
                    {
                        b.CustomValue1 = rotationSpeed;
                        b.CustomValue2 = 0f;
                        b.CustomValue3 = mainEmitter.Center.X;
                        b.CustomValue4 = mainEmitter.Center.Y;

                        rotatingBullets.Add(b);
                    }

                    foreach (Bullet b in mainEmitter.FireBulletExplosion(10, 60f, Color.Red))
                    {
                        b.CustomValue1 = rotationSpeed * -1f;
                        b.CustomValue2 = 0f;
                        b.CustomValue3 = mainEmitter.Center.X;
                        b.CustomValue4 = mainEmitter.Center.Y;

                        rotatingBullets.Add(b);
                    }

                    nextShotTime += .75f;
                }

                yield return waitTime;
            }

            yield return 3f;

            // Now explode for real
        }

        public IEnumerator<float> FinalPhaseMovement(GameObject go)
        {
            // Don't move at all during the first phase.
            yield return 20f;

            // For phase 2, move back and forth between the top locations.
            yield return 4f;

            LerpPosition(new Vector2(250f, 75f), 2f);
            yield return 4f;

            LerpPosition(new Vector2(350f, 75f), 2f);
            yield return 4f;

            LerpPosition(new Vector2(450f, 75f), 2f);
            yield return 4f;

            LerpPosition(new Vector2(350f, 75f), 2f);
            
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            ring1.Draw(spriteBatch);
            ring2.Draw(spriteBatch);
            topLayer.Draw(spriteBatch);
        }


        public void CollisionHandling(GameObject sender, CollisionEventArgs e)
        {
            if (CollidedObjects.Contains(sender))
                return;
            else
                CollidedObjects.Add(sender);

            // If collision occured with a player bullet...
            if (e.collisionLayer == 2)
            {
                sender.Destroy();
                // Flash the thing here.
                if (Vulnerable)
                {
                    // Flash the thing here.
                    Health--;
                }
            }

            // Collided with player; kill!
            if (e.collisionLayer == 3)
            {
                PlayerShip thisShip = (PlayerShip)e.otherObject;
                thisShip.ObjectCollidedWith(this, new CollisionEventArgs(this, 1));
            }
        }
    }
}
