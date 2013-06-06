using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AsteroidRebuttal;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Scripting;

namespace AsteroidRebuttal.Enemies
{
    public class Phantom : Enemy
    {
        public BulletEmitter mainEmitter;
        public BulletEmitter leftWingCannon;
        public BulletEmitter leftInnerWingCannon;
        public BulletEmitter rightWingCannon;
        public BulletEmitter rightInnerWingCannon;

        public Phantom(GameScene newScene, Vector2 position = new Vector2()) : base(newScene, position) { }

        public override void Initialize()
        {
            Health = 400;
            Texture = phantomTexture;
            Color = Color.Transparent;

            DrawLayer = .425f;

            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2 };

            Origin = new Vector2(127.5f, 101.5f);
            Hitbox = new Circle(Center, 15f);

            CollidesWithLayers = new int[1] { 2 } ;
            CollidedObjects = new List<GameObject>();
            DeletionBoundary = new Vector2(300, 300);

            InitializeParts();

            pointValue = 8000;

            scriptManager = thisScene.scriptManager;

            OnOuterCollision += CollisionHandling;

            base.Initialize();
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

        public IEnumerator<float> PhaseIn(GameObject go)
        {
            AudioManager.PlaySoundEffect(GameScene.PhaseInSound, .8f, 0f);
            LerpColor(Color.White, 2f);
            yield return 2f;
            Phasing = false;
        }

        public IEnumerator<float> PhaseOut(GameObject go)
        {
            AudioManager.PlaySoundEffect(GameScene.PhaseOutSound, .8f, 0f);
            Phasing = true;
            LerpColor(Color.Transparent, 2f);
            yield return 2f;
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
                CheckForDeath();
            }
            else if (e.collisionLayer == 3)
            {
                Health -= 2;
                CheckForDeath();
            }
        }
    }
}