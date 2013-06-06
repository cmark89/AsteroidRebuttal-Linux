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
    public class PhantomBoss : Boss
    {
        BulletEmitter mainEmitter;
        BulletEmitter leftWingCannon;
        BulletEmitter leftInnerWingCannon;
        BulletEmitter rightWingCannon;
        BulletEmitter rightInnerWingCannon;

        int phase = 1;

        public PhantomBoss(GameScene newScene, Vector2 newPos = new Vector2()) : base(newScene, newPos)
        {
            thisScene = newScene;
            Center = newPos;

            Initialize();
        }


        public override void Initialize()
        {
            Health = 500;
            DrawLayer = .425f;

            // Get the actual origin.
            Origin = new Vector2(127.5f, 101.5f);
            Hitbox = new Circle(Center, 15f);
            Texture = boss2Texture;


            DeletionBoundary = new Vector2(1500, 1500);

            // Set transparent for the fade in
            Color = Color.Transparent;

            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2 };

            InitializeParts();

            scriptManager = thisScene.scriptManager;
            scriptManager.Execute(Phase1Script, this);

            OnOuterCollision += CollisionHandling;

            PhaseChangeValues = new List<int>()
            {
                400,
                200,
                100
            };

            MaxHealth = (int)Health;

            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {            
            if (phase == 1 && Health < 400)
            {
                phase = 2;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase2Script, this);
            }

            if (phase == 2 && Health < 200)
            {
                phase = 3;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.AbortObjectScripts(leftWingCannon);
                scriptManager.AbortObjectScripts(rightWingCannon);
                scriptManager.Execute(Phase3Script, this);
            }

            if (phase == 3 && Health < 100)
            {
                phase = 4;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase4Script, this);
            }

            base.Update(gameTime);
        }

        public void InitializeParts()
        {
            mainEmitter = new BulletEmitter(this, Origin, false);
            mainEmitter.LockedToParentPosition = true;
            mainEmitter.LockPositionOffset = Vector2.Zero;

            leftWingCannon = new BulletEmitter(this, Origin, false);
            leftWingCannon.LockedToParentPosition = true;
            leftWingCannon.LockPositionOffset = new Vector2(26, 154) - Origin;

            leftInnerWingCannon = new BulletEmitter(this, Origin, false);
            leftInnerWingCannon.LockedToParentPosition = true;
            leftInnerWingCannon.LockPositionOffset = new Vector2(65.5f, 141) - Origin;

            rightInnerWingCannon = new BulletEmitter(this, Origin, false);
            rightInnerWingCannon.LockedToParentPosition = true;
            rightInnerWingCannon.LockPositionOffset = new Vector2(189.5f, 141) - Origin;

            rightWingCannon = new BulletEmitter(this, Origin, false);
            rightWingCannon.LockedToParentPosition = true;
            rightWingCannon.LockPositionOffset = new Vector2(229, 154) - Origin;

            mainEmitter.Rotation = (float)(Math.PI / 2f);
            leftWingCannon.Rotation = (float)(Math.PI / 2f);
            leftInnerWingCannon.Rotation = (float)(Math.PI / 2f);
            rightInnerWingCannon.Rotation = (float)(Math.PI / 2f);
            rightWingCannon.Rotation = (float)(Math.PI / 2f);
        }


        public IEnumerator<float> Phase1Script(GameObject thisShip)
        {
            Center = new Vector2(350, 100);
            AudioManager.PlaySoundEffect(GameScene.PhaseInSound, .8f, 0f);
            LerpColor(Color.White, 2f);
            yield return 3f;

            Vulnerable = true;

            int repeats = 0;
            while (true)
            {
                int shots = 0;

                while (shots < 5)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .5f, -.5f);
                    rightInnerWingCannon.FireBulletSpread((float)Math.PI / 2f, 5, 90, 200, Color.Lerp(Color.White, Color.Orange, .7f));
                    leftInnerWingCannon.FireBulletSpread((float)Math.PI / 2f, 5, 90, 200, Color.Lerp(Color.White, Color.Orange, .7f));
                    yield return .2f;
                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .5f, -.5f);
                    leftWingCannon.FireBulletSpread((float)Math.PI / 2f, 5, 90, 200, Color.Lerp(Color.White, Color.Orange, .7f));
                    rightWingCannon.FireBulletSpread((float)Math.PI / 2f, 5, 90, 200, Color.Lerp(Color.White, Color.Orange, .7f));

                    shots++;
                    yield return .2f;                    
                }

                Phasing = true;
                AudioManager.PlaySoundEffect(GameScene.PhaseOutSound, .8f, 0f);
                LerpColor(Color.Transparent, 1f);
                yield return 1f;

                // Make it a random position instead of this exacty position...
                if(repeats % 2 == 0)
                    LerpPosition(new Vector2(450, 400), .3f);
                else
                    LerpPosition(new Vector2(250, 400), .3f);

                yield return .3f;

                AudioManager.PlaySoundEffect(GameScene.PhaseInSound, .8f, 0f);
                LerpColor(Color.White, 1f);
                yield return 1f;
                Phasing = false;

                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .8f, 0f);
                mainEmitter.FireBulletExplosion(30, 300f, Color.DeepSkyBlue);
                yield return .2f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .8f, 0f);
                leftInnerWingCannon.FireBulletExplosion(40, 300f, Color.DeepSkyBlue);
                yield return .2f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .8f, 0f);
                rightInnerWingCannon.FireBulletExplosion(40, 300f, Color.DeepSkyBlue);
                yield return .2f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .8f, 0f);
                leftInnerWingCannon.FireBulletExplosion(40, 300f, Color.DeepSkyBlue);
                yield return .2f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .8f, 0f);
                rightInnerWingCannon.FireBulletExplosion(40, 300f, Color.DeepSkyBlue);
                yield return .2f;

                Phasing = true;
                AudioManager.PlaySoundEffect(GameScene.PhaseOutSound, .8f, 0f);
                LerpColor(Color.Transparent, 1f);
                yield return 1f;

                LerpPosition(new Vector2(350, 100), .2f);
                yield return .2f;

                AudioManager.PlaySoundEffect(GameScene.PhaseInSound, .8f, 0f);
                LerpColor(Color.White, 1f);
                yield return 1f;
                Phasing = false;

                yield return 1f;

                repeats++;
            }
        }



        public IEnumerator<float> Phase2Script(GameObject thisShip)
        {
            Phasing = true;
            AudioManager.PlaySoundEffect(GameScene.PhaseOutSound, .8f, 0f);
            LerpColor(Color.Transparent, 1.5f);
            yield return 2f;

            Center = new Vector2(300, 25);

            AudioManager.PlaySoundEffect(GameScene.PhaseInSound, .8f, 0f);
            LerpColor(Color.White, 1.5f);
            yield return 2;
            Phasing = false;

            leftWingCannon.Rotation = (float)Math.PI;
            rightWingCannon.Rotation = 0f;
            scriptManager.Execute(Phase2PrisonEmitter, leftWingCannon);
            scriptManager.Execute(Phase2PrisonEmitter, rightWingCannon);
            rightWingCannon.CustomValue1 = -1;
            leftWingCannon.CustomValue1 = 1;
            yield return .5f;
            LerpPosition(new Vector2(350, 425), 2.2f);
            yield return 2.7f;

            // Begin the spray and pray phase
            while(true)
            {
                int shots = 0;
                int cycles = 0;
                int shotsToFire = 30;
                float spread = 15f;

                while (cycles < 4)
                {
                    Random rand = new Random();
                    mainEmitter.CustomValue1 = 1;
                    
                    
                    while (shots < shotsToFire)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .35f, -.8f);
                        shots++;
                        float angle = VectorMathHelper.GetAngleTo(thisScene.player.InnerHitbox.Center, mainEmitter.Center, spread);

                        Bullet newBullet = mainEmitter.FireBullet(angle, 80 + (float)rand.NextDouble() * 20, Color.DeepSkyBlue, BulletType.Circle);
                        newBullet.LerpVelocity(25, 5f);
                        scriptManager.Execute(SeekingHailBullets, newBullet);

                        yield return .04f;
                    }
                    cycles++;
                    shots = 0;
                    spread += 15f;
                    shotsToFire += 10;
                    yield return 2.2f;
                }

                LerpPosition(new Vector2(350, 300), 2.2f);
                yield return 1.2f;
                shots = 0;

                while (shots < 10)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .7f, .2f);
                    leftInnerWingCannon.FireBulletSpread(leftInnerWingCannon.Rotation - .33f, 4, 40f, 130f, Color.DeepSkyBlue);
                    yield return .35f;
                    AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .7f, .2f);
                    rightInnerWingCannon.FireBulletSpread(rightInnerWingCannon.Rotation + .33f, 4, 40f, 130f, Color.DeepSkyBlue);
                    yield return .35f;

                    shots++;
                }

                LerpPosition(new Vector2(350, 425), 2.2f);
            }
        }


        public IEnumerator<float> Phase3Script(GameObject thisShip)
        {
            Phasing = true;
            AudioManager.PlaySoundEffect(GameScene.PhaseOutSound, .8f, 0f);
            LerpColor(Color.Transparent, 1.2f);
            yield return 2f;

            LerpPosition(new Vector2(350f, 45f), .1f);
            AudioManager.PlaySoundEffect(GameScene.PhaseInSound, .8f, 0f);
            LerpColor(Color.White, 1.2f);
            yield return 1.2f;
            Phasing = false;

            Vector2[] positions = new Vector2[3]
            {
                new Vector2(350f, 45f),
                new Vector2(125f, 45f),
                new Vector2(550f, 45f)
            };

            Random rand = new Random();
            int cycles = 0;

            while (true)
            {
                while (cycles < 4)
                {
                    int shots = 0;

                    Vector2 target = thisScene.player.InnerHitbox.Center;
                    while (shots < 15)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot2Sound, .3f, 0f);
                        leftWingCannon.FireBullet(VectorMathHelper.GetAngleTo(leftWingCannon.Center, target), 325f, Color.Lerp(Color.White, Color.Orange, .7f));
                        rightWingCannon.FireBullet(VectorMathHelper.GetAngleTo(rightWingCannon.Center, target), 325f, Color.Lerp(Color.White, Color.Orange, .7f));
                        shots++;
                        yield return .03f;
                    }

                    shots = 0;
                    yield return .2f;

                    float randomSpray = .7f;
                    while (shots < 10)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .35f, -.3f);
                        Bullet newBullet;
                        newBullet = leftInnerWingCannon.FireBullet(VectorMathHelper.GetAngleTo(leftInnerWingCannon.Center, thisScene.player.InnerHitbox.Center), 80f, Color.DeepSkyBlue);
                        newBullet.LerpRotation(VectorMathHelper.GetAngleTo(newBullet.Center, thisScene.player.InnerHitbox.Center) + (((float)rand.NextDouble() * 2) - 1f) * randomSpray, 4f);
                        newBullet = rightInnerWingCannon.FireBullet(VectorMathHelper.GetAngleTo(rightInnerWingCannon.Center, thisScene.player.InnerHitbox.Center), 80f, Color.DeepSkyBlue);
                        newBullet.LerpRotation(VectorMathHelper.GetAngleTo(newBullet.Center, thisScene.player.InnerHitbox.Center) + (((float)rand.NextDouble() * 2) - 1f) * randomSpray, 4f);
                        shots++;
                        yield return .06f;
                    }

                    shots = 0;
                    cycles++;
                    yield return .5f;
                }

                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .7f, -.2f);
                mainEmitter.FireBulletExplosion(25, 225f, Color.Lerp(Color.White, Color.Orange, .7f));
                yield return .1f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .7f, -.2f);
                mainEmitter.FireBulletExplosion(24, 225f, Color.Lerp(Color.White, Color.Orange, .7f));
                yield return .1f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .7f, -.2f);
                mainEmitter.FireBulletExplosion(23, 225f, Color.Lerp(Color.White, Color.Orange, .7f));
                yield return .1f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .7f, -.2f);
                mainEmitter.FireBulletExplosion(22, 225f, Color.Lerp(Color.White, Color.Orange, .7f));
                yield return .1f;
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .7f, -.2f);
                mainEmitter.FireBulletExplosion(21, 225f, Color.Lerp(Color.White, Color.Orange, .7f));
                yield return .1f;

                // Lerp to the next position!!
                Phasing = true;
                AudioManager.PlaySoundEffect(GameScene.PhaseOutSound, .8f, 0f);
                LerpColor(Color.Transparent, 1.2f);
                yield return 1.5f;
                Center = positions[rand.Next(0, 3)];
                yield return .03f;

                AudioManager.PlaySoundEffect(GameScene.PhaseInSound, .8f, 0f);
                LerpColor(Color.White, 1.2f);
                yield return 1.2f;
                Phasing = false;
                //LerpPosition(positions[rand.Next(0, 3)], 2f);
                cycles = 0;
            }
        }

        public IEnumerator<float> Phase4Script(GameObject thisObject)
        {
            AudioManager.PlaySoundEffect(GameScene.PhaseInSound, .8f, 0f);
            LerpColor(Color.White, 1f);
            yield return 2f;

            LerpPosition(new Vector2(350, 200), 4f);
            scriptManager.Execute(Phase4BulletWave, this);

            scriptManager.Execute(ModulatePhasingLong, this);

            // Now just move around the screen, cool dude.
        }


        public IEnumerator<float> Phase2PrisonEmitter(GameObject thisObject)
        {
            BulletEmitter thisEmitter = (BulletEmitter)thisObject;

            thisEmitter.LerpRotation((float)Math.PI/2f, 2.5f);
            int shots = 0;
            Color bulletColor;
            while (true)
            {
                thisEmitter.Rotation += (float)Math.Sin(currentGameTime * 12) / 300f * thisEmitter.CustomValue1;

                if (shots % 2 == 0)
                    bulletColor = Color.Goldenrod;
                else
                    bulletColor = Color.Lerp(Color.White, Color.Goldenrod, .5f);

                // Prevent infinite grazing on these bullets
                AudioManager.PlaySoundEffect(GameScene.Shot3Sound, .35f, -.3f);
                thisEmitter.FireBullet(200f, bulletColor, BulletType.Circle).Grazed = true;
                shots++;
                yield return .03f;
            }
        }

        public IEnumerator<float> SeekingHailBullets(GameObject thisObject)
        {
            Bullet thisBullet = (Bullet)thisObject;
            yield return 3f;
            AudioManager.PlaySoundEffect(GameScene.Shot6Sound, .5f, 0f);
            thisBullet.Rotation = VectorMathHelper.GetAngleTo(thisBullet.Center, thisScene.player.InnerHitbox.Center);
            thisBullet.LerpVelocity(350f, 0.3f);
        }

        public IEnumerator<float> Phase4BulletWave(GameObject go)
        {
            Random rand = new Random();
            float speed;

            while (true)
            {
                speed = rand.Next(40, 120);
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, 1f, -.8f);
                foreach (Bullet b in mainEmitter.FireBulletExplosion(100, speed, Color.Lerp(Color.White, Color.Orange, .5f)))
                    scriptManager.Execute(ModulatePhasing, b);

                yield return 1.8f;

                speed /= 2f;

                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, 1f, -.6f);
                foreach (Bullet b in mainEmitter.FireBulletExplosion(100, speed, Color.DeepSkyBlue))
                    scriptManager.Execute(ModulatePhasing, b);

                yield return 1.8f;
            }
            
        }

        public IEnumerator<float> ModulatePhasing(GameObject thisObject)
        {
            Color thisColor = thisObject.Color;
            while (true)
            {
                yield return 1f;
                thisObject.Phasing = true;
                thisObject.LerpColor(Color.Transparent, 1f);
                float startTime = currentGameTime;


                yield return 2.5f;

                float deltaTime = currentGameTime - startTime;
                thisObject.LerpColor(thisColor, 1f);
                yield return 1f;
                thisObject.Phasing = false;
                yield return 2f;
            }
        }

        public IEnumerator<float> Metronome()
        {
            while (true)
            {
                Console.WriteLine("Wait two seconds!");
                float startTime = currentGameTime;

                yield return 2f;

                float deltaTime = currentGameTime - startTime;

                Console.WriteLine("Waited " + deltaTime + " seconds.");
            }
        }


        public IEnumerator<float> ModulatePhasingLong(GameObject thisObject)
        {
            Random rand = new Random();
            Color thisColor = thisObject.Color;

            Vector2[] positions = new Vector2[]
            {
                new Vector2(350, 200),
                new Vector2(350, 80),
                new Vector2(90, 200),
                new Vector2(90, 80),
                new Vector2(550, 200),
                new Vector2(550, 80)
            };

            while (true)
            {
                yield return 5f;
                thisObject.Phasing = true;
                if(this is PhantomBoss)
                    AudioManager.PlaySoundEffect(GameScene.PhaseOutSound, .8f, 0f);
                thisObject.LerpColor(Color.Transparent, 2f);
                yield return 2f;

                // New position!
                LerpPosition(positions[rand.Next(0, 6)], 4.5f);

                yield return 5f;

                if (this is PhantomBoss)
                    AudioManager.PlaySoundEffect(GameScene.PhaseInSound, .8f, 0f);

                thisObject.LerpColor(thisColor, 2f);
                yield return 2f;
                thisObject.Phasing = false;
                yield return 2f;
            }
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
                if (Vulnerable)
                {
                    // Flash the thing here.
                    Health--;
                }
            }
        }
    }
}
