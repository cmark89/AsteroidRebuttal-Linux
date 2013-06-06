using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Core;

namespace AsteroidRebuttal.GameObjects
{
    public class Bullet : GameObject
    {
        #region Bullet Graphics
        public static Texture2D CircleBulletTexture;
        public static Texture2D CircleSmallBulletTexture;
        public static Texture2D ArrowBulletTexture;
        public static Texture2D DiamondSmallBulletTexture;
        public static Texture2D DiamondBulletTexture;

        #endregion

        BulletType bulletType;

        // Stores whether or not the bullet has already been grazed.
        public bool Grazed { get; set; }

        // Stores the GameObject that produced this bullet.
        GameObject parent;

        // Stores the script manager that belongs to the parent if it exists

        public Bullet(GameObject newParent, Vector2 newPosition, float newRotation, float newVelocity, Color newColor, BulletType type = BulletType.CircleSmall)
        {
            parent = newParent;

            // Because the Origin does not exist yet, set Position here and update it later once Origin exists.
            Position = newPosition;

            Rotation = newRotation;
            Velocity = newVelocity;

            Color = newColor;
            bulletType = type;

            Initialize();
        }

        public override void Initialize()
        {
            thisScene = parent.thisScene;
            CollisionLayer = 0;
            DrawLayer = .12f;


            // Switch on the bullet ty
            switch (bulletType)
            {
                case (BulletType.Circle):
                    Texture = CircleBulletTexture;
                    Origin = new Vector2(7.5f, 7.5f);
                    Hitbox = new Circle(Center, 4.5f);
                    break;
                case(BulletType.CircleSmall):
                    Texture = CircleSmallBulletTexture;
                    Origin = new Vector2(4.5f, 4.5f);
                    Hitbox = new Circle(Center, 3f);
                    break;
                case (BulletType.Diamond):
                    Texture = DiamondBulletTexture;
                    Origin = new Vector2(15.5f, 15.5f);
                    Hitbox = new Circle(Center, 7f);
                    break;
                case(BulletType.DiamondSmall):
                    Texture = DiamondSmallBulletTexture;
                    Origin = new Vector2(7.5f, 7.5f);
                    Hitbox = new Circle(Center, 4f);
                    break;
                case(BulletType.Arrow):
                    Texture = ArrowBulletTexture;
                    Origin = new Vector2(15f, 8f);
                    Hitbox = new Circle(Center, 8f);
                    break;
                default:
                    break;
            }

            Center = Position;
            DeletionBoundary = new Vector2(24, 24);
             
            base.Initialize();
        }

        public static void LoadContent(ContentManager content)
        {
            if (CircleBulletTexture == null)
            {
                CircleBulletTexture = content.Load<Texture2D>("Graphics/Bullets/BulletCircle16");
            }
            if (CircleSmallBulletTexture == null)
            {
                CircleSmallBulletTexture = content.Load<Texture2D>("Graphics/Bullets/BulletCircleSmall");
            }
            if (DiamondBulletTexture == null)
            {
                DiamondBulletTexture = content.Load<Texture2D>("Graphics/Bullets/Diamond32");
            }
            if (DiamondSmallBulletTexture == null)
            {
                DiamondSmallBulletTexture = content.Load<Texture2D>("Graphics/Bullets/DiamondBullet");
            }
            if (ArrowBulletTexture == null)
            {
                ArrowBulletTexture = content.Load<Texture2D>("Graphics/Bullets/ArrowBullet");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Center, Texture.Bounds, Color, Rotation + ((float)Math.PI / 2), Origin, 1f, SpriteEffects.None, DrawLayer);
        }
    }


    public enum BulletType
    {
        Circle,
        CircleSmall,
        Diamond,
        DiamondSmall,
        Arrow
    }
}
