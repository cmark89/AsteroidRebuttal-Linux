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
    public class ParasiteOrb : Enemy
    {
        ParasiteBoss parentBoss;
        float nextLeftBounceTime = 0;
        float nextRightBounceTime = 0;
        float nextUpBounceTime = 0;
        float nextDownBounceTime = 0;

        public ParasiteOrb(GameScene newScene, Vector2 newPos = new Vector2()) 
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

        public override void Update(GameTime gameTime)
        {
            if (Center.X - Hitbox.Radius < thisScene.ScreenArea.X && nextRightBounceTime < gameTime.TotalGameTime.TotalSeconds)
            {
                // Bounce right
                Rotation = MathHelper.WrapAngle(Rotation = VectorMathHelper.GetRandomBetween((float)Math.PI / 4f, (float)Math.PI / -4f));
                Center = new Vector2(thisScene.ScreenArea.X + Hitbox.Radius, Center.Y);

                nextRightBounceTime =  (float)gameTime.TotalGameTime.TotalSeconds + .1f;
            }
            else if (Center.X + Hitbox.Radius > thisScene.ScreenArea.X + thisScene.ScreenArea.Width && nextLeftBounceTime < gameTime.TotalGameTime.TotalSeconds)
            {
                // Bounce left
                Rotation = MathHelper.WrapAngle(VectorMathHelper.GetRandomBetween(((float)Math.PI / 4) * 3f, ((float)Math.PI / 4f) * 6f));
                Center = new Vector2(thisScene.ScreenArea.X + thisScene.ScreenArea.Width - Hitbox.Radius, Center.Y);

                nextLeftBounceTime = (float)gameTime.TotalGameTime.TotalSeconds + .1f;
            }
            else if (Center.Y - Hitbox.Radius < thisScene.ScreenArea.Y && nextDownBounceTime < gameTime.TotalGameTime.TotalSeconds)
            {
                // Bounce down
                Rotation = MathHelper.WrapAngle(Rotation = VectorMathHelper.GetRandomBetween((float)Math.PI / 4f, ((float)Math.PI / 4f) * 3f));
                Center = new Vector2(Center.X, thisScene.ScreenArea.Y + Hitbox.Radius);

                nextDownBounceTime = (float)gameTime.TotalGameTime.TotalSeconds + .1f;
            }
            else if (Center.Y + Hitbox.Radius > thisScene.ScreenArea.Y + thisScene.ScreenArea.Height && nextUpBounceTime < gameTime.TotalGameTime.TotalSeconds)
            {
                // Bounce up
                Rotation = MathHelper.WrapAngle((Rotation = VectorMathHelper.GetRandomBetween((float)Math.PI / -4f, ((float)Math.PI / -4f) * 3f)));
                Center = new Vector2(Center.X, thisScene.ScreenArea.Y + thisScene.ScreenArea.Height - Hitbox.Radius);

                nextUpBounceTime = (float)gameTime.TotalGameTime.TotalSeconds + .1f;
            }

            base.Update(gameTime);
        }

        public void Explode()
        {
            AudioManager.PlaySoundEffect(GameScene.Explosion1Sound, .9f, -.5f);
            AudioManager.PlaySoundEffect(GameScene.Shot7Sound, .9f, .5f);
            foreach (Bullet b in new BulletEmitter(parentBoss, Center, true).FireBulletExplosion(15, 80f, Color.Red))
            {
                b.Phasing = true;
                b.LerpColor(Color.Transparent, 1.5f);
            }
            foreach (Bullet b in new BulletEmitter(parentBoss, Center, true).FireBulletExplosion(20, 120f, Color.Red, BulletType.CircleSmall))
            {
                b.Phasing = true;
                b.LerpColor(Color.Transparent, 1.7f);
            }

            //Damage the boss here.
            parentBoss.TakeDamage(1, this);
            Destroy();
        }

        public void SetParent(ParasiteBoss parasiteParent)
        {
            parentBoss = parasiteParent;
        }

        public void SetHealth(int newHealth)
        {
            Health = newHealth;
        }
    }
}
