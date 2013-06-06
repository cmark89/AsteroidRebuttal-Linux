using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AsteroidRebuttal;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Scripting;

namespace AsteroidRebuttal.Enemies.Bosses
{
    class CrablordEgg : Enemy
    {
        public CrablordEgg(GameScene newScene, Vector2 newPos = new Vector2()) 
            : base(newScene, newPos)
        {
            thisScene = newScene;
            Center = newPos;

            Initialize();
        }


        public override void Initialize()
        {
            Health = 5;
            DrawLayer = .3f;

            // Get the actual origin.
            Origin = new Vector2(16.5f, 16.5f);
            Hitbox = new Circle(Center, 10f);

            usesSimpleExplosion = false;

            DeletionBoundary = new Vector2(100, 100);

            Color = Color.White;

            CollisionLayer = 1;
            CollidesWithLayers = new int[] { 2 };

            scriptManager = thisScene.scriptManager;

            OnOuterCollision += CollisionHandling;


            base.Initialize();
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
                Health--;

                if (Health <= 0)
                    Explode();
            }
        }

        public void Explode()
        {
            BulletEmitter emitter = new BulletEmitter(this, Center, true);
            AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .9f, -.2f);

            foreach (Bullet b in emitter.FireBulletExplosion(100, 250, Color.DarkGreen))
                scriptManager.Execute(CrablordBoss.ToxicBulletEffect, b);

            Destroy();
        }
    }
}
