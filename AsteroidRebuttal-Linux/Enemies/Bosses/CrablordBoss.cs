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
    public class CrablordBoss : Boss
    {
        BulletEmitter mainEmitter;
        BulletEmitter arm1Emitter;
        BulletEmitter arm2Emitter;
        BulletEmitter arm3Emitter;
        BulletEmitter arm4Emitter;
        BulletEmitter arm5Emitter;
        BulletEmitter arm6Emitter;

        int phase = 1;
        Random rand = new Random();

        // Used for holding things
        Vector2[] arms;

        // Used for circle movement
        bool circleMoving;
        Vector2 circleCenter;
        float orbitAngle;
        float orbitDistance;
        bool lerpingOrbitDistance;
        float targetOrbitDistance;
        float orbitSpeed;
        float elapsedOrbitLerpTime;
        float orbitLerpDuration;
        float orbitalExcentricityX = 1f;
        float orbitalExcentricityY = 2.2f;

        List<GameObject> eggs = new List<GameObject>();

        public CrablordBoss(GameScene newScene, Vector2 newPos = new Vector2()) 
            : base(newScene, newPos)
        {
            thisScene = newScene;
            Center = newPos;

            Initialize();
        }


        public override void Initialize()
        {
            Health = 500;
            DrawLayer = .31f;

            // Get the actual origin.
            Origin = new Vector2(127.5f, 84.5f);
            Hitbox = new Circle(Center, 50f);
            Texture = boss3Texture;
            DeletionBoundary = new Vector2(1500, 1500);

            Color = Color.White;

            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2 };

            InitializeParts();

            scriptManager = thisScene.scriptManager;
            scriptManager.Execute(Phase1Script, this);

            OnOuterCollision += CollisionHandling;

            PhaseChangeValues = new List<int>()
            {
                250,
                150
            };
            MaxHealth = (int)Health;

            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            if (circleMoving)
            {
                if (lerpingOrbitDistance)
                {
                    elapsedOrbitLerpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    orbitDistance = MathHelper.Lerp(0f, targetOrbitDistance, elapsedOrbitLerpTime / orbitLerpDuration);
                    if (elapsedOrbitLerpTime >= orbitLerpDuration)
                        lerpingOrbitDistance = false;
                }

                // Modify this based on circle moving properties
                orbitAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * orbitSpeed;

                Vector2 orbitMovement;
                orbitMovement.X = (float)Math.Cos(orbitAngle) * orbitDistance/orbitalExcentricityX;
                orbitMovement.Y = (float)Math.Sin(orbitAngle) * orbitDistance/orbitalExcentricityY;

                Center = circleCenter + orbitMovement;
            }

            if (phase == 1 && Health < 250)
            {
                phase = 2;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase2Script, this);
            }

            if (phase == 2 && Health < 150)
            {
                phase = 3;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase3Script, this);
            }

            base.Update(gameTime);
        }

        public void InitializeParts()
        {
            arms = new Vector2[6]
            {
                new Vector2(38, 167),
                new Vector2(77, 155),
                new Vector2(110, 150),
                new Vector2(146, 150),
                new Vector2(179, 155),
                new Vector2(213, 167)
            };

            Hitbox = new Circle(Center, 50f);

            mainEmitter = new BulletEmitter(this, Center, false);
            mainEmitter.LockedToParentPosition = true;
            mainEmitter.LockPositionOffset = new Vector2(0f, 43f);

            arm1Emitter = new BulletEmitter(this, Center, false);
            arm1Emitter.LockedToParentPosition = true;
            arm1Emitter.LockPositionOffset = arms[0] - Origin;

            arm2Emitter = new BulletEmitter(this, Center, false);
            arm2Emitter.LockedToParentPosition = true;
            arm2Emitter.LockPositionOffset = arms[1] - Origin;

            arm3Emitter = new BulletEmitter(this, Center, false);
            arm3Emitter.LockedToParentPosition = true;
            arm3Emitter.LockPositionOffset = arms[2] - Origin;

            arm4Emitter = new BulletEmitter(this, Center, false);
            arm4Emitter.LockedToParentPosition = true;
            arm4Emitter.LockPositionOffset = arms[3] - Origin;

            arm5Emitter = new BulletEmitter(this, Center, false);
            arm5Emitter.LockedToParentPosition = true;
            arm5Emitter.LockPositionOffset = arms[4] - Origin;

            arm6Emitter = new BulletEmitter(this, Center, false);
            arm6Emitter.LockedToParentPosition = true;
            arm6Emitter.LockPositionOffset = arms[5] - Origin;
        }

        public IEnumerator<float> Phase1Script(GameObject thisShip)
        {
            while (true)
            {
                thisShip.LerpPosition(new Vector2(350f, 65f), 3f);
                yield return 3f;
                Vulnerable = true;

                int cycles = 0;

                CircleMovement(60f, 1f, 4f);

                while (cycles < 2)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .8f, .35f);
                    Bullet[] theseBullets = mainEmitter.FireBulletCluster((float)Math.PI / 2f, 40, 70f, 40, 100f, Color.DarkGreen);
                    foreach (Bullet b in theseBullets)
                    {
                        scriptManager.Execute(ToxicBulletEffect, b);
                    }

                    yield return 3f;

                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .6f, .35f);
                    theseBullets = mainEmitter.FireBulletCluster((float)Math.PI / 2f, 40, 70f, 40, 100f, Color.DarkGreen);
                    foreach (Bullet b in theseBullets)
                    {
                        scriptManager.Execute(ToxicBulletEffect, b);
                    }

                    yield return 2f;

                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .6f, .35f);
                    theseBullets = mainEmitter.FireBulletCluster((float)Math.PI / 2f + .75f, 40, 70f, 30, 90f, Color.DarkGreen);
                    foreach (Bullet b in theseBullets)
                    {
                        scriptManager.Execute(ToxicBulletEffect, b);
                    }

                    yield return .5f;

                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .6f, .35f);
                    theseBullets = mainEmitter.FireBulletCluster((float)Math.PI / 2f - .75f, 40, 70f, 30, 90f, Color.DarkGreen); ;
                    foreach (Bullet b in theseBullets)
                    {
                        scriptManager.Execute(ToxicBulletEffect, b);
                    }

                    yield return .5f;

                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .6f, .35f);
                    theseBullets = mainEmitter.FireBulletCluster((float)Math.PI / 2f - .45f, 20, 70f, 20, 80f, Color.DarkGreen); ;
                    foreach (Bullet b in theseBullets)
                    {
                        scriptManager.Execute(ToxicBulletEffect, b);
                    }

                    yield return .5f;

                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .6f, .35f);
                    theseBullets = mainEmitter.FireBulletCluster((float)Math.PI / 2f + .45f, 20, 70f, 20, 80f, Color.DarkGreen); ;
                    foreach (Bullet b in theseBullets)
                    {
                        scriptManager.Execute(ToxicBulletEffect, b);
                    }

                    yield return .5f;
                    cycles++;
                }

                circleMoving = false;
                LerpPosition(new Vector2(350f, 75f), .8f);
                yield return .8f;

                CircleMovement(30f, 2f, 2f);

                cycles = 0;

                while (cycles < 5)
                {
                    // Roll the angles of the shots
                    float angle1 = VectorMathHelper.GetAngleTo(arm1Emitter.Center, thisScene.player.InnerHitbox.Center, 90f);
                    float angle2 = VectorMathHelper.GetAngleTo(arm2Emitter.Center, thisScene.player.InnerHitbox.Center, 45f);
                    float angle3 = VectorMathHelper.GetAngleTo(arm3Emitter.Center, thisScene.player.InnerHitbox.Center, 10f);
                    float angle4 = VectorMathHelper.GetAngleTo(arm4Emitter.Center, thisScene.player.InnerHitbox.Center, 10f);
                    float angle5 = VectorMathHelper.GetAngleTo(arm5Emitter.Center, thisScene.player.InnerHitbox.Center, 45f);
                    float angle6 = VectorMathHelper.GetAngleTo(arm6Emitter.Center, thisScene.player.InnerHitbox.Center, 90f);

                    int shots = 0;
                    while (shots < 45)
                    {
                        Bullet b;

                        if (shots < 15)
                        {
                            AudioManager.PlaySoundEffect(GameScene.Shot2Sound, .25f, .8f);
                            b = arm1Emitter.FireBullet(angle1, 250f, Color.DarkGreen, BulletType.Arrow);
                            scriptManager.Execute(ToxicBulletEffect, b);
                            b = arm6Emitter.FireBullet(angle6, 250f, Color.DarkGreen, BulletType.Arrow);
                            scriptManager.Execute(ToxicBulletEffect, b);
                        }

                        if (shots >= 15 && shots < 30)
                        {
                            AudioManager.PlaySoundEffect(GameScene.Shot2Sound, .25f, .6f);
                            b = arm2Emitter.FireBullet(angle2, 250f, Color.DarkGreen, BulletType.Arrow);
                            scriptManager.Execute(ToxicBulletEffect, b);
                            b = arm5Emitter.FireBullet(angle5, 250f, Color.DarkGreen, BulletType.Arrow);
                            scriptManager.Execute(ToxicBulletEffect, b);
                        }

                        if (shots >= 30)
                        {
                            AudioManager.PlaySoundEffect(GameScene.Shot2Sound, .25f, .7f);
                            b = arm3Emitter.FireBullet(angle3, 250f, Color.DarkGreen, BulletType.Arrow);
                            scriptManager.Execute(ToxicBulletEffect, b);
                            b = arm4Emitter.FireBullet(angle4, 250f, Color.DarkGreen, BulletType.Arrow);
                            scriptManager.Execute(ToxicBulletEffect, b);
                        }

                        shots++;
                        yield return .03f;
                    }
                    
                    cycles++;

                    yield return .5f;

                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .6f, .35f);
                    Bullet[] theseBullets = mainEmitter.FireBulletCluster(VectorMathHelper.GetAngleTo(mainEmitter.Center, thisScene.player.InnerHitbox.Center), 50, 80f, 120, 40f, Color.DarkGreen);
                    foreach (Bullet b in theseBullets)
                    {
                        scriptManager.Execute(ToxicBulletEffect, b);
                    }

                }

                cycles = 0;
                circleMoving = false;
            }
        }


        public IEnumerator<float> Phase2Script(GameObject thisShip)
        {
            circleMoving = false;
            thisShip.LerpPosition(new Vector2(350f, 65f), 1f);
            yield return 1f;

            CircleMovement(200f, .4f, 5f, 1f, 5f);
            CreateEgg(1);
            CreateEgg(6);

            while (true)
            {
                // First, randomize a number of volleys to fire before releasing the next egg.
                int targetVolleys = rand.Next(2, 6);
                int volleys = 0;

                while (volleys < targetVolleys)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .6f, .35f);
                    Bullet[] theseBullets = mainEmitter.FireBulletCluster(VectorMathHelper.GetAngleTo(mainEmitter.Center, thisScene.player.InnerHitbox.Center), 40, 50f, 40, 50f, Color.DarkGreen);
                    foreach (Bullet b in theseBullets)
                    {
                        scriptManager.Execute(ToxicBulletEffect, b);
                    }

                    if (volleys == targetVolleys / 2)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot8Sound, .8f, .35f);
                        theseBullets = mainEmitter.FireBulletExplosion(45, 130f, Color.DarkGreen);
                        foreach (Bullet b in theseBullets)
                        {
                            scriptManager.Execute(ToxicBulletEffect, b);
                        }
                    }

                    yield return 2f;

                    volleys++;
                }

                // Release the eggs being carried.
                foreach(GameObject egg in eggs)
                {
                    ReleaseEgg(egg);
                    egg.Rotation = (float)Math.PI / 2f + (((float)rand.NextDouble() * 2) - 1) * .5f;
                    egg.Velocity = 20f;
                    egg.LerpVelocity(50f, 6f);
                }

                eggs.Clear();

                // Wait and then summon some more eggs.
                yield return 1.5f;

                switch (rand.Next(0, 3))
                {
                    case(0):
                        CreateEgg(1);
                        CreateEgg(6);
                        break;
                    case(1):
                        CreateEgg(2);
                        CreateEgg(5);
                        break;
                    case(2):
                     CreateEgg(3);
                     CreateEgg(4);
                        break;
                    default:
                        break;
                }
            }
        }

        public IEnumerator<float> Phase3Script(GameObject thisShip)
        {
            if (eggs.Count > 0)
            {
                foreach (GameObject go in eggs)
                {
                    ReleaseEgg(go);
                    go.Rotation = (float)Math.PI / 2f + (((float)rand.NextDouble() * 2) - 1) * .5f;
                    go.Velocity = 20f;
                    go.LerpVelocity(80f, 6f);
                }

                eggs.Clear();

            }
            circleMoving = false;
            thisShip.LerpPosition(new Vector2(350f, 65f), 1f);
            yield return 1f;

            //CircleMovement(200f, .1f, 5f, 1f, 5f);

            float[] waitTimes = new float[]
            {
                15,
                23,
                27,
                29,
                30,
                30.5f
            };

            while (true)
            {
                // Summon all the eggs
                eggs.Clear();
                CreateEgg(1);
                CreateEgg(6);
                yield return .3f;
                CreateEgg(2);
                CreateEgg(5);
                yield return .3f;
                CreateEgg(3);
                CreateEgg(4);

                List<GameObject> tempList = eggs;

                for (int i = 0; i < 6; i++)
                {
                    // Select a random egg from the temp list to get the first slot.
                    int index = rand.Next(0, tempList.Count);
                    GameObject thisEgg = tempList[index];
                    thisEgg.CustomValue1 = waitTimes[i];
                    scriptManager.Execute(DelayedReleaseEgg, thisEgg);
                    
                    tempList.RemoveAt(index);
                }

                // Shoot the player full of bullets for a while.
                float startTime = currentGameTime;

                while (currentGameTime < startTime + waitTimes[5])
                {
                    // Shoot
                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .6f, .35f);
                    foreach (Bullet b in mainEmitter.FireBulletCluster(VectorMathHelper.GetAngleTo(mainEmitter.Center, thisScene.player.InnerHitbox.Center), 18, 150f, 50f, 100f, Color.DarkGreen))
                    {
                        scriptManager.Execute(ToxicBulletEffect, b);
                        b.LerpVelocity(35f, 4f);
                    }

                    yield return .7f;
                }

                yield return 3f;
            }
        }


        public void CircleMovement(float targetDistance, float speed, float duration, float xEcc = 1f, float yEcc = 2.2f)
        {
            circleMoving = true;
            circleCenter = Center;
            orbitAngle = 0f;
            orbitDistance = 0f;

            lerpingOrbitDistance = true;
            targetOrbitDistance = targetDistance;
            orbitSpeed = speed;
            elapsedOrbitLerpTime = 0f;
            orbitLerpDuration = duration;

            orbitalExcentricityX = xEcc;
            orbitalExcentricityY = yEcc;
        }

        // Creates an egg in the arm indicated by the index
        public void CreateEgg(int index)
        {
            CrablordEgg newEgg = new CrablordEgg(thisScene, Center);
            newEgg.Phasing = true;
            newEgg.Parent = this;
            newEgg.SetTexture(boss3EggTexture);

            newEgg.LockedToParentPosition = true;
            newEgg.LockPositionOffset = arms[index - 1] - Origin;
            newEgg.Color = Color.Transparent;
            newEgg.LerpColor(Color.White, 1.5f);

            eggs.Add(newEgg);
            scriptManager.Execute(EggPhaseIn, newEgg);
        }

        public GameObject ReleaseEgg(GameObject go)
        {
            go.LockedToParentPosition = false;

            return go;
        }

        public IEnumerator<float> DelayedReleaseEgg(GameObject go)
        {
            GameObject thisEgg = go;

            yield return thisEgg.CustomValue1;

            ReleaseEgg(thisEgg);

            thisEgg.Rotation = (float)Math.PI / 2f + (((float)rand.NextDouble() * 2) - 1) * .5f;
            thisEgg.Velocity = 20f;
            thisEgg.LerpVelocity(80f, 6f);
        }

        public IEnumerator<float> EggPhaseIn(GameObject thisObject)
        {
            yield return 1.5f;
            thisObject.Phasing = false;
        }

        public static IEnumerator<float> ToxicBulletEffect(GameObject thisObject)
        {
            while (true)
            {
                thisObject.LerpColor(Color.LimeGreen, .7f);
                yield return .7f;
                thisObject.LerpColor(Color.DarkGreen, .7f);
                yield return .4f;
            }
        }

        public override void Destroy()
        {
            foreach (GameObject go in eggs)
            {
                go.Destroy();
            }

            base.Destroy();
        }


        public void CollisionHandling(GameObject sender, CollisionEventArgs e)
        {
            if (CollidedObjects.Contains(sender))
            {
                return;
            }
            else
                CollidedObjects.Add(sender);

            // If collision occured with a player bullet...
            if (e.collisionLayer == 2)
            {
                sender.Destroy();
                // Flash the thing here.
                Health--;
            }
        }
    }
}