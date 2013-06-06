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
    public class Komodo : Enemy
    {
        BulletEmitter mainEmitter;
        public Komodo(GameScene newScene, Vector2 position = new Vector2()) : base(newScene, position) { }

        public override void Initialize()
        {
            Health = 50;
            Texture = komodoTexture;
            Color = Color.White;

            DrawLayer = .3f;

            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2 };

            Origin = new Vector2(32.5f, 32.5f);

            Hitbox = new Circle(Center, 12f);

            CollidesWithLayers = new int[1] { 2 } ;
            CollidedObjects = new List<GameObject>();
            DeletionBoundary = new Vector2(200, 200);

            mainEmitter = new BulletEmitter(this, Vector2.Zero, false);
            mainEmitter.LockedToParentPosition = true;
            mainEmitter.LockPositionOffset = (Origin / 2);

            pointValue = 8000;

            scriptManager = thisScene.scriptManager;

            OnOuterCollision += CollisionHandling;

            base.Initialize();
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