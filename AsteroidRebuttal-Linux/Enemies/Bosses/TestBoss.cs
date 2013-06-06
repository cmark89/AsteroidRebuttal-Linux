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
    public class TestBoss : Boss
    {
        public int phase = 1;
        int animFrame = 1;
        float animTime = 0f;
        BulletEmitter subEmitter1;
        BulletEmitter subEmitter2;

        public TestBoss(GameScene newScene, Vector2 newPos = new Vector2()) : base(newScene, newPos)
        {
            thisScene = newScene;
            Center = newPos;

            Initialize();
        }

        public override void Initialize()
        {
            Health = 400;
            DrawLayer = .3f;

            // Get the actual origin.
            Origin = new Vector2(23.5f, 23.5f);
            Hitbox = new Circle(Center, 15f);
            Texture = boss1Texture;

            DeletionBoundary = new Vector2(1500, 1500);
            Color = Color.White;

            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2 };

            scriptManager = thisScene.scriptManager;
            scriptManager.Execute(MainScriptPhase1, this);

            OnOuterCollision += CollisionHandling;

            PhaseChangeValues = new List<int>()
            {
                200
            };
            MaxHealth = (int)Health;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            animTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(animTime > .12f)
            {
                animTime = 0f;
                if (animFrame == 1)
                {
                    animFrame = 2;
                    Texture = boss1Texture2;
                }
                else
                {
                    animFrame = 1;
                    Texture = boss1Texture;
                }
            }

            if(phase == 1 && Health < 200)
            {
                phase = 2;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(MainScriptPhase2, this);
            }

            if (phase == 2)
            {
                CustomValue1 += (((float)Math.PI * 2) / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds * .75f;
                CustomValue2 -= (((float)Math.PI * 2) / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds * .75f;
            }

            base.Update(gameTime);
        }

        // Main script
        public IEnumerator<float> MainScriptPhase1(GameObject thisShip)
        {
            BulletEmitter mainEmitter = new BulletEmitter(this, this.Center, false);
            mainEmitter.LockedToParentPosition = true;
            
            int bossPhase = 1;
            LerpPosition(new Vector2(300f, 125f), 2f);
            yield return 3f;
            Vulnerable = true;

            bool alive = true;
            while (alive)
            {
                int cycles = 0;

                while (bossPhase == 1)
                {
                    int shots = 0;

                    while (shots < 15)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .8f, .4f);
                        mainEmitter.FireBulletSpread(((float)Math.PI / 2), 7, 90f, 140f, Color.Lerp(Color.White, Color.Orange, .45f), BulletType.Circle);
                        shots++;

                        yield return .25f;
                        AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .8f, .2f);
                        mainEmitter.FireBulletSpread(((float)Math.PI / 2), 6, 70f, 140f, Color.Lerp(Color.White, Color.Orange, .45f), BulletType.Circle);
                        shots++;
                        yield return .25f;
                    }

                    shots = 0;
                    LerpPosition(new Vector2(25, 125), 1.5f);
                    yield return 1.5f;

                    while (shots < 20)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .3f, -.6f);
                        mainEmitter.FireBulletSpread(((float)Math.PI / 2 * .6f) + (float)Math.Sin(currentGameTime * 3) / 4, 3, 25f, 220f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                        shots++;
                        yield return .06f;
                    }

                    shots = 0;
                    LerpPosition(new Vector2(600, 125), 2.5f);
                    yield return 2.5f;

                    while (shots < 20)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .3f, -.6f);
                        mainEmitter.FireBulletSpread(((float)Math.PI / 2 * 1.4f) + (float)Math.Sin(currentGameTime * 3) / 4, 3, 25f, 220f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                        shots++;
                        yield return .06f;
                    }

                    LerpPosition(new Vector2(300f, 125f), 1.5f);
                    yield return 2.2f;
                    cycles++;
                    if (cycles > 1)
                        bossPhase++;
                }

                subEmitter1 = new BulletEmitter(this, this.Center, false);
                subEmitter1.LockedToParentPosition = true;
                subEmitter1.LockPositionOffset = new Vector2(-28f, -10f);
                subEmitter2 = new BulletEmitter(this, this.Center, false);
                subEmitter2.LockedToParentPosition = true;
                subEmitter2.LockPositionOffset = new Vector2(28f, -10f);

                scriptManager.Execute(Subemitter1Script, subEmitter1);
                scriptManager.Execute(Subemitter2Script, subEmitter2);
                scriptManager.Execute(SubemitterFirePhase2, subEmitter1);
                scriptManager.Execute(SubemitterFirePhase2, subEmitter2);

                yield return 1f;

                cycles = 0;
                while (bossPhase == 2)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, 0f);
                    mainEmitter.FireBulletExplosion(15, 100f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .5f;
                    LerpPosition(new Vector2(50, 400), 3f);
                    yield return 1.5f;
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, 0f);
                    mainEmitter.FireBulletExplosion(15, 100f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .5f;
                    LerpPosition(new Vector2(550, 400), 3f);
                    yield return 1.5f;
                    LerpPosition(new Vector2(300f, 250f), 2f);
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, 0f);
                    mainEmitter.FireBulletExplosion(15, 100f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .5f;
                    LerpPosition(new Vector2(300f, 125f), 4f);
                    yield return .25f;
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, 0f);
                    mainEmitter.FireBulletExplosion(15, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .25f;
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, 0f);
                    mainEmitter.FireBulletExplosion(13, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .25f;
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, 0f);
                    mainEmitter.FireBulletExplosion(11, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .25f;
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, 0f);
                    mainEmitter.FireBulletExplosion(15, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .25f;
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, 0f);
                    mainEmitter.FireBulletExplosion(13, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .25f;
                    AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .6f, 0f);
                    mainEmitter.FireBulletExplosion(11, 150f, Color.Lerp(Color.White, Color.DeepSkyBlue, .45f), BulletType.Circle);
                    yield return .5f;

                    cycles++;
                    if (cycles > 2)
                    {
                        bossPhase = 3;
                    }
                }

                cycles = 0;
                subEmitter1.Destroy();
                subEmitter2.Destroy();

                LerpPosition(new Vector2(300f, 125f), 3.5f);
                while (bossPhase == 3)
                {
                    int shots = 0;
                    while (shots < 25)
                    {
                        shots++;
                        AudioManager.PlaySoundEffect(GameScene.Shot3Sound, .5f, -.6f);
                        mainEmitter.FireBulletSpread(VectorMathHelper.GetAngleTo(this.Hitbox.Center, thisScene.player.InnerHitbox.Center), 5, 80f, 200f, Color.Lerp(Color.White, Color.Orange, .4f), BulletType.Circle);
                        yield return .175f;
                    }

                    int i = 1;

                    AudioManager.PlaySoundEffect(GameScene.Shot8Sound, 1f, 0f);
                    foreach (Bullet b in mainEmitter.FireBulletWave(0f, 4, 200f, 50f, Color.Lerp(Color.White, Color.Orange, .3f)))
                    {
                        b.CustomValue1 = i++;
                        scriptManager.Execute(ClusterBombs, b);
                    }

                    i = 1;
                    foreach (Bullet b in mainEmitter.FireBulletWave((float)Math.PI, 4, 200f, 50f, Color.Lerp(Color.White, Color.Orange, .3f)))
                    {
                        b.CustomValue1 = i++;
                        scriptManager.Execute(ClusterBombs, b);
                    }

                    yield return 3.5f;
                    cycles++;

                    if (cycles > 2)
                    {
                        bossPhase = 1;
                    }
                }
            }
        }


        public IEnumerator<float> MainScriptPhase2(GameObject thisShip)
        {
            if(subEmitter1 != null)
                subEmitter1.Destroy();

            if(subEmitter2 != null)
                subEmitter2.Destroy();

            BulletEmitter mainEmitter1 = new BulletEmitter(this, this.Center, false);
            mainEmitter1.CustomValue1 = 1;       // Rotate this emitter counter clockwise
            mainEmitter1.CustomValue2 = 35;       // Rotate at this distance.
            BulletEmitter mainEmitter2 = new BulletEmitter(this, this.Center, false);
            mainEmitter2.CustomValue2 = 35f;        // This is the distance to orbit.

            scriptManager.Execute(RotatingEmitters, mainEmitter1);
            scriptManager.Execute(RotatingEmitters, mainEmitter2);

            

            while (true)
            {
                LerpPosition(new Vector2(350, 255), 2f);
                yield return .9f;
                CustomValue1 = (float)Math.PI / 2f * 3;
                CustomValue2 = CustomValue1;
                yield return .075f;

                int shots = 0;

                while (shots < 120)
                {
                    int subshots = 0;

                    while (subshots < 4)
                    {
                        AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .29f, .5f);
                        mainEmitter1.FireBullet(VectorMathHelper.GetAngleTo(this.Center, mainEmitter1.Center), 170f, Color.Lerp(Color.White, Color.DeepSkyBlue, .6f));
                        mainEmitter2.FireBullet(VectorMathHelper.GetAngleTo(this.Center, mainEmitter2.Center), 170f, Color.Lerp(Color.White, Color.DeepSkyBlue, .6f));
                        subshots++;
                        shots++;
                        yield return .03f;
                    }
                    subshots = 0;
                    yield return .03f;
                }

                LerpPosition(new Vector2(350, 135), 1.5f);
                shots = 0;
                // Randomize the rotation
                CustomValue1 = (float)(new Random().NextDouble() * ((float)Math.PI*2));
                CustomValue2 = CustomValue1;

                while (shots < 10)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .8f, -.15f);
                    foreach (Bullet b in (mainEmitter1.FireBulletExplosion(15, 170f, Color.Lerp(Color.White, Color.Orange, .4f))))
                    {
                        b.LerpVelocity(90f, 3f);
                    }
                    foreach (Bullet b in (mainEmitter2.FireBulletExplosion(15, 190f, Color.Lerp(Color.White, Color.Red, .4f), BulletType.CircleSmall)))
                    {
                        b.LerpVelocity(90f, 3f);
                    }
                    shots++;
                    yield return .15f;
                }

                yield return 1.5f;
            }
        }

        public IEnumerator<float> RotatingEmitters(GameObject go)
        {
            BulletEmitter thisEmitter = (BulletEmitter) go;
            while(true)
            {
                float angle = 0;

                if (thisEmitter.CustomValue1 == 1)
                {
                    angle = thisEmitter.Parent.CustomValue2;
                }
                else
                {
                    angle = thisEmitter.Parent.CustomValue1; 
                }

                float x = (float)Math.Cos(angle) * thisEmitter.CustomValue2;
                float y = (float)Math.Sin(angle) * thisEmitter.CustomValue2;
                go.Position = go.Parent.Center + new Vector2(x, y);

                yield return 0f;
            }
        }

        public IEnumerator<float> Subemitter1Script(GameObject go)
        {
            BulletEmitter thisEmitter = (BulletEmitter)go;
            thisEmitter.Rotation = (float)Math.PI;
            while (thisEmitter != null)
            {
                thisEmitter.LerpRotation(((float)Math.PI / 2) * 1.5f, 5f);
                yield return 7f;
                thisEmitter.LerpRotation((float)Math.PI, 5f);
                yield return 7f;
            }
        }

        public IEnumerator<float> Subemitter2Script(GameObject go)
        {
            BulletEmitter thisEmitter = (BulletEmitter)go;

            thisEmitter.Rotation = 0f;

            while (thisEmitter != null)
            {
                thisEmitter.LerpRotation(((float)Math.PI / 2) * .5f, 5f);
                yield return 7f;
                thisEmitter.LerpRotation(0f, 5f);
                yield return 7f;
            }

            yield return 0f;
        }

        public IEnumerator<float> SubemitterFirePhase2(GameObject go)
        {
            
            BulletEmitter thisEmitter = (BulletEmitter)go;
            while (true)
            {
                AudioManager.PlaySoundEffect(GameScene.Shot3Sound, .2f, -.5f);
                thisEmitter.FireBullet(thisEmitter.Rotation, 350f, Color.Lerp(Color.White, Color.Orange, .7f), BulletType.DiamondSmall);
                yield return .03f;
            }
        }

        public IEnumerator<float> ClusterBombs(GameObject go)
        {
            Bullet thisBullet = (Bullet)go;
            thisBullet.LerpVelocity(0f, 3f);
            yield return 3f + (thisBullet.CustomValue1 * .6f);

            BulletEmitter explosion = new BulletEmitter(this, thisBullet.Center);

            AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .7f, 0f);
            AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .7f, 0f);
            explosion.FireBulletExplosion(6, 160f, Color.Lerp(Color.White, Color.DeepSkyBlue, .4f));
            explosion.FireBulletCluster(VectorMathHelper.GetAngleTo(thisBullet.Center, thisScene.player.InnerHitbox.Center), 3, 40f, 160f, 40f, Color.Lerp(Color.White, Color.DeepSkyBlue, .4f), BulletType.Circle);
            explosion.FireBulletCluster(VectorMathHelper.GetAngleTo(thisBullet.Center, thisScene.player.InnerHitbox.Center), 4, 50f, 140f, 50f, Color.Lerp(Color.White, Color.DeepSkyBlue, .4f), BulletType.CircleSmall);
            explosion.Destroy();

            thisBullet.Destroy();
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

        public virtual IEnumerator<float> CustomExplosion(GameObject go)
        {
            Texture = null;
            Rotation = 0;
            Velocity = 0;

            yield return 0f;
            Destroy();
        }
    }
}
