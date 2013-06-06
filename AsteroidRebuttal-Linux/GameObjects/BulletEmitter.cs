using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.Core;

namespace AsteroidRebuttal.GameObjects
{
    public class BulletEmitter : GameObject
    {
        BulletEmitterType Type;
        Random rand;

        static Texture2D EyeFamiliarTexture;

        public BulletEmitter(GameObject newParent, Vector2 newPosition, bool oneshot = false, BulletEmitterType type = BulletEmitterType.Invisible)
        {
            Parent = newParent;
            thisScene = newParent.thisScene;
            Center = newPosition;
            Type = type;

            if (oneshot)
                FlaggedForRemoval = true;

            Initialize();
        }

        public override void Initialize()
        {
            Hitbox = new Circle(Center, 0f);

            // Set phasing to true by default.
            Phasing = true;
            CollisionLayer = 8;

            rand = new Random();

            DeletionBoundary = Parent.DeletionBoundary;

            // Switch on the type to set its graphics
            if (Type != BulletEmitterType.Invisible)
            {
                switch (Type)
                {
                    case (BulletEmitterType.Eye):
                        Texture = EyeFamiliarTexture;
                        Origin = new Vector2(24.5f, 24.5f);
                        break;
                    default:
                        break;
                }
            }

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public static void LoadContent(ContentManager content)
        {
            if (EyeFamiliarTexture == null)
                EyeFamiliarTexture = content.Load<Texture2D>("Graphics/BulletEmitters/eyefamiliar");
        }


        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (Type != BulletEmitterType.Invisible)
                spriteBatch.Draw(Texture, Center, Texture.Bounds, Color.White, Rotation, Origin, 1f, SpriteEffects.None, 0f);
        }

        // Simple method that creates a bullet and shoots it in the direction the emitter is facing
        public Bullet FireBullet(float velocity, Color newColor, BulletType type = BulletType.Circle)
        {
            if (AsteroidRebuttal.ManicMode)
                velocity *= 2;

            return new Bullet(Parent, Center, Rotation, velocity, newColor, type);
        }

        // Method that creates a bullet and shoots it in any direction
        public Bullet FireBullet(float rotation, float velocity, Color newColor, BulletType type = BulletType.Circle, bool overridemanicmode = false)
        {
            if (AsteroidRebuttal.ManicMode && !overridemanicmode)
                velocity *= 2;

            return new Bullet(Parent, Center, rotation, velocity, newColor, type);
        }

        // Method that creates a bullet and shoots it in any direction
        public Bullet[] FireBulletExplosion(int numberOfBullets, float velocity, Color newColor, BulletType type = BulletType.Circle)
        {
            if (AsteroidRebuttal.ManicMode)
                velocity *= 2;

            List<Bullet> bullets = new List<Bullet>();
            
            float spread = ((float)(Math.PI * 2) / (float)numberOfBullets);

            for (int i = 0; i < numberOfBullets; i++ )
            {
                bullets.Add(FireBullet(Rotation + spread * i, velocity, newColor, type));
            }

            return bullets.ToArray();
        }


        // Method that creates a bullet and shoots it in any direction
        public Bullet[] FireBulletSpread(float rotation, int numberOfBullets, float spread, float velocity, Color newColor, BulletType type = BulletType.Circle)
        {
            if (AsteroidRebuttal.ManicMode)
                velocity *= 2;

            List<Bullet> bullets = new List<Bullet>();
            spread = VectorMathHelper.DegreesToRadians(spread);

            float angleDifference = spread / ((float)numberOfBullets - 1);
            float startAngle = rotation - (spread / 2);

            for (int i = 0; i < numberOfBullets; i++)
            {
                bullets.Add(FireBullet(startAngle + (angleDifference * i), velocity, newColor, type));
            }

            return bullets.ToArray();
        }


        // Method that creates a cluster of bullets in a random spread and at variable velocity.
        public Bullet[] FireBulletCluster(float rotation, int numberOfBullets, float spread, float baseVelocity, float randomVelocity, Color newColor, BulletType type = BulletType.Circle)
        {
            if (AsteroidRebuttal.ManicMode)
                baseVelocity *= 2;

            List<Bullet> bullets = new List<Bullet>();
            spread = VectorMathHelper.DegreesToRadians(spread);
            float launchRotation;
            float velocity;

            for (int i = 0; i < numberOfBullets; i++)
            {
                velocity = baseVelocity + ((float)rand.NextDouble() * randomVelocity);
                launchRotation = rotation + ((((float)rand.NextDouble() * 2) - 1) * (spread / 2));
                bullets.Add(FireBullet(launchRotation, velocity, newColor, type));
            }

            return bullets.ToArray();
        }

        // Fire a number of bullets at once in the same direction, with each bullet's speed reduced by the speed variance 
        public Bullet[] FireBulletWave(float rotation, int numberOfBullets, float startVelocity, float velocityVariance, Color newColor, BulletType type = BulletType.Circle)
        {
            if (AsteroidRebuttal.ManicMode)
                startVelocity *= 2;

            List<Bullet> bullets = new List<Bullet>();

            for (int i = 0; i < numberOfBullets; i++)
            {
                bullets.Add(FireBullet(rotation, startVelocity - (velocityVariance * i), newColor, type));
            }

            return bullets.ToArray();
        }
    }

    public enum BulletEmitterType
    {
        Invisible = 0,
        Eye
    }
}
