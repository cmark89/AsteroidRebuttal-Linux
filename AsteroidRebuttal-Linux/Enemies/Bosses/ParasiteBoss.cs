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
    public class ParasiteBoss : Boss
    {
        BulletEmitter mainEmitter;
        BulletEmitter arm1Emitter;
        BulletEmitter arm2Emitter;
        BulletEmitter arm3Emitter;
        BulletEmitter arm4Emitter;
        BulletEmitter arm5Emitter;
        BulletEmitter arm6Emitter;

        int phase = 1;
        int thisPhaseOrbHealth = 45;
        Random rand = new Random();

        // Used for holding things
        Vector2[] arms;

        List<GameObject> orbs = new List<GameObject>();

        public ParasiteBoss(GameScene newScene, Vector2 newPos = new Vector2()) 
            : base(newScene, newPos)
        {
            thisScene = newScene;
            Center = newPos;

            Initialize();
        }


        public override void Initialize()
        {
            Health = 9;
            DrawLayer = .31f;

            // Get the actual origin.
            Origin = new Vector2(127.5f, 61.5f);
            Hitbox = new Circle(Center, 25f);
            Texture = boss4Texture;
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
                8,
                6,
                3
            };
            MaxHealth = (int)Health;


            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {

            if (phase == 1 && Health < 9)
            {
                phase = 2;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase2Script, this);
            }
            if (phase == 2 && Health < 7)
            {
                phase = 3;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase3Script, this);
            }
            if (phase == 3 && Health < 4)
            {
                thisPhaseOrbHealth = 25;
                phase = 4;
                thisScene.BossPhaseChange();
                scriptManager.AbortObjectScripts(this);
                scriptManager.Execute(Phase4Script, this);
            }            

            base.Update(gameTime);
        }

        public void InitializeParts()
        {
            arms = new Vector2[6]
            {
                new Vector2(31, 22),
                new Vector2(25, 65),
                new Vector2(55, 121),
                new Vector2(224, 22),
                new Vector2(231, 65),
                new Vector2(200, 121)
            };

            Hitbox = new Circle(Center, 25f);

            mainEmitter = new BulletEmitter(this, Center, false);
            mainEmitter.LockedToParentPosition = true;
            mainEmitter.LockPositionOffset = new Vector2(0f, 0f);

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
            LerpPosition(new Vector2(375, 100f), 4f);
            yield return 4f;

            SummonOrb();
            scriptManager.Execute(Phase1Orb, orbs[0]);

            yield return 2f;

            while (true)
            {
                AudioManager.PlaySoundEffect(GameScene.Shot2Sound, .15f, -.35f);
                mainEmitter.Rotation = ((float)Math.PI / 2f) + (((float)Math.PI/2f) * (float)Math.Sin(currentGameTime * 2f));
                mainEmitter.FireBullet(150f, Color.Orange, BulletType.DiamondSmall);
                yield return .03f;
            }
        }

        public IEnumerator<float> Phase1Orb(GameObject go)
        {
            while (true)
            {
                yield return 3f;
                new BulletEmitter(this, go.Center, true).FireBulletCluster(VectorMathHelper.GetAngleTo(go.Center, thisScene.player.InnerHitbox.Center), 14, 35f, 140f, 40f, Color.Purple);
                AudioManager.PlaySoundEffect(GameScene.Shot1Sound, .7f, -.5f);
                
            }
        }

        public IEnumerator<float> Phase2Script(GameObject go)
        {
            yield return 3f;

            int cycles = 0;
            while (true)
            {
                float waveEndTime = currentGameTime + 5f;
                while (currentGameTime < waveEndTime)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .25f, 0f);
                    Bullet[] firedBullets = mainEmitter.FireBulletSpread(VectorMathHelper.GetAngleTo(mainEmitter.Center, thisScene.player.InnerHitbox.Center), 5, 80f, 180f, Color.White, BulletType.DiamondSmall);
                    firedBullets[0].Color = Color.Orange;
                    firedBullets[1].Color = Color.Purple;
                    firedBullets[2].Color = Color.OrangeRed;
                    firedBullets[3].Color = Color.Purple;
                    firedBullets[4].Color = Color.Orange;

                    foreach(Bullet b in firedBullets)
                    {
                        scriptManager.Execute(Phase2BulletCurve, b);
                    }

                    yield return .03f;
                }
                cycles++;

                yield return 1.5f;

                if (cycles < 3)
                {
                    scriptManager.Execute(Phase2Orb, SummonOrb());
                }
                yield return 1.5f;
            }
        }

        public IEnumerator<float> Phase2Orb(GameObject go)
        {
            while (true)
            {
                yield return 2f;
                new BulletEmitter(this, go.Center, true).FireBulletWave(VectorMathHelper.GetAngleTo(go.Center, thisScene.player.InnerHitbox.Center), 3, 100f, 25f, Color.MediumPurple, BulletType.DiamondSmall);
                AudioManager.PlaySoundEffect(GameScene.Shot6Sound, .75f, 0f);
            }
        }

        public IEnumerator<float> Phase2BulletCurve(GameObject go)
        {
            go.CustomValue1 = go.Rotation;
            float amountToCurve = (float)Math.PI/5f;

            while (true)
            {
                yield return .35f;
                go.Rotation = go.CustomValue1 + amountToCurve;
                yield return .35f;
                go.Rotation = go.CustomValue1 - amountToCurve;
            }
        }

        public IEnumerator<float> Phase3Script(GameObject go)
        {
            yield return 2.5f;

            BulletEmitter mainEmitter2 = new BulletEmitter(this, mainEmitter.Center, false);
            BulletEmitter mainEmitter3 = new BulletEmitter(this, mainEmitter.Center, false);
            BulletEmitter mainEmitter4 = new BulletEmitter(this, mainEmitter.Center, false);

            orbs.Clear();
            int cycles = 0;

            while (true)
            {
                // do this for 10 seconds and then wait for 5.
                float finishTime = currentGameTime + 10f;
                while (currentGameTime < finishTime)
                {
                    AudioManager.PlaySoundEffect(GameScene.Shot2Sound, .15f, -.35f);
                    AudioManager.PlaySoundEffect(GameScene.Shot4Sound, .15f, 0f);

                    mainEmitter.Rotation = ((float)Math.PI / 2f) + (((float)Math.PI / 2f) * (float)Math.Sin(currentGameTime * 3f));
                    mainEmitter.FireBullet(150f, Color.Orange, BulletType.DiamondSmall);

                    mainEmitter2.Rotation = ((float)Math.PI / 2f) + (((float)Math.PI / 2f) * (float)Math.Sin(currentGameTime * 3f) / 1.5f);
                    mainEmitter2.FireBullet(130f, Color.OrangeRed, BulletType.DiamondSmall);

                    mainEmitter3.Rotation = ((float)Math.PI / 2f) + (((float)Math.PI / 2f) * (float)Math.Sin(currentGameTime * 2.5f) / 1.8f);
                    mainEmitter3.FireBullet(120f, Color.Purple, BulletType.DiamondSmall);

                    mainEmitter4.Rotation = mainEmitter.Rotation = ((float)Math.PI / 2f) + (((float)Math.PI / 2f) * (float)Math.Cos(currentGameTime * 1.6f));
                    mainEmitter4.FireBullet(110f, Color.MediumPurple, BulletType.DiamondSmall);

                    yield return .03f;
                }

                cycles++;

                if (cycles < 4)
                {
                    yield return 2f;

                    SummonOrb();
                    foreach (ParasiteOrb po in orbs)
                        scriptManager.Execute(Phase3Orb, po);
                }

                yield return 6f;
            }
        }

        public IEnumerator<float> Phase3Orb(GameObject go)
        {
            while(true)
            {
                yield return 4f;
                new BulletEmitter(this, go.Center, true).FireBulletExplosion(20, 75f, Color.MediumPurple, BulletType.DiamondSmall);
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .5f, .45f);
            }
        }

        public IEnumerator<float> Phase4Script(GameObject go)
        {

            yield return 3f;
            // Set the order the emitters will fire in.
            List<BulletEmitter> emitters = new List<BulletEmitter>()
            {
                arm3Emitter,
                arm4Emitter,
                arm1Emitter,
                arm6Emitter,
                arm2Emitter,
                arm5Emitter,
            };

            List<Color> colors = new List<Color>()
            {
                Color.Orange,
                Color.Purple,
                Color.MediumPurple,
                Color.Orange,
                Color.Purple,
                Color.MediumPurple
            };

            int orbsSummoned = 0;
            float nextSummonTime = currentGameTime + 6f;
            int currentArm = 0;
            while (true)
            {
                emitters[currentArm].Rotation = VectorMathHelper.GetAngleTo(emitters[currentArm].Center, thisScene.player.InnerHitbox.Center);
                AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .65f, .45f);
                foreach (Bullet b in emitters[currentArm].FireBulletExplosion(25, 190f, colors[currentArm], BulletType.DiamondSmall))
                {
                    b.LerpVelocity(100f, 3f);
                }

                currentArm++;
                if (currentArm > emitters.Count - 1)
                    currentArm = 0;

                if (currentGameTime > nextSummonTime && orbsSummoned < 3)
                {
                    orbsSummoned++;
                    nextSummonTime += 15f;
                    scriptManager.Execute(Phase4Orb, SummonOrb());
                }

                yield return .23f;
            }
        }

        public IEnumerator<float> Phase4Orb(GameObject go)
        {
            BulletEmitter thisEmitter = new BulletEmitter(this, go.Center, false);
            thisEmitter.Rotation = VectorMathHelper.GetAngleTo(thisEmitter.Center, thisScene.player.InnerHitbox.Center);
            thisEmitter.CustomValue1 = thisEmitter.Rotation;
            while (true)
            {
                yield return 3f;
                float waveEnd = currentGameTime + 1.8f;

                while (currentGameTime < waveEnd)
                {
                    thisEmitter.Center = go.Center;
                    thisEmitter.Rotation = thisEmitter.CustomValue1 + ((float)Math.Sin(currentGameTime * 2f)) / 3f;
                    AudioManager.PlaySoundEffect(GameScene.Shot5Sound, .8f, 0f);
                    Bullet b = thisEmitter.FireBullet(160f, Color.OrangeRed, BulletType.DiamondSmall);
                    b.LerpVelocity(80f, 4f);
                    yield return .06f;
                }

                yield return 3f;
            }   
        }

        public ParasiteOrb SummonOrb()
        {
            AudioManager.PlaySoundEffect(GameScene.PhaseInSound, .8f, 0f);
            ParasiteOrb orb = new ParasiteOrb(thisScene, Center);
            orb.SetTexture(boss4OrbTexture);
            orbs.Add(orb);
            orb.Rotation = VectorMathHelper.GetRandom();
            orb.Velocity = 55f;
            orb.Color = Color.Transparent;
            orb.LerpColor(Color.White, .5f);
            orb.SetParent(this);
            orb.SetHealth(thisPhaseOrbHealth);

            return orb;
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
            //if (e.collisionLayer == 2)
            //{
                //sender.Destroy();
                // Flash the thing here.
                //Health--;
            //}
        }

        public void TakeDamage(float damage, ParasiteOrb explodedOrb)
        {
            Health -= damage;
            orbs.Remove(explodedOrb);
        }
    }
}