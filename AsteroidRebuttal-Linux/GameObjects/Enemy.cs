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
using Microsoft.Xna.Framework.Content;

namespace AsteroidRebuttal.Enemies
{
    public class Enemy : GameObject, ICollidable
    {
        protected List<Bullet> bullets;

        public float Health { get; set; }

        public List<GameObject> CollidedObjects { get; set; }
        public int[] CollidesWithLayers { get; set; }
        public event CollisionEventHandler OnOuterCollision;
        public event CollisionEventHandler OnInnerCollision;

        protected ScriptManager scriptManager;

        protected static Texture2D slicerTexture;
        protected static Texture2D tortoiseTexture;
        protected static Texture2D dragonflyTexture;
        protected static Texture2D komodoTexture;
        protected static Texture2D phantomTexture;

        protected int pointValue;
        protected bool usesSimpleExplosion = true;

        public Enemy(GameScene newScene, Vector2 position = new Vector2())
        {
            thisScene = newScene;
            Center = position;

            Initialize();
        }
        
        public override void Initialize()
        {
            if(Hitbox == null)
                Hitbox = new Circle(Center, 15f);

            if(scriptManager == null)
                scriptManager = thisScene.scriptManager;

            if (CollidesWithLayers == null)
                CollidesWithLayers = new int[0];

            CollidedObjects = new List<GameObject>();

            if (DeletionBoundary == null)
                DeletionBoundary = new Vector2(Hitbox.Radius + 20, Hitbox.Radius + 20);

            Rotation = (float)Math.PI / 2;

            base.Initialize();
        }

        public static void LoadContent(ContentManager content)
        {
            if (slicerTexture == null)
            {
                slicerTexture = content.Load<Texture2D>("Graphics/Ships/Enemy1");
            }
            if (tortoiseTexture == null)
            {
                tortoiseTexture = content.Load<Texture2D>("Graphics/Ships/Enemy2");
            }
            if (dragonflyTexture == null)
            {
                dragonflyTexture = content.Load<Texture2D>("Graphics/Ships/Enemy3");
            }
            if (komodoTexture == null)
            {
                komodoTexture = content.Load<Texture2D>("Graphics/Ships/Enemy4");
            }
            if (phantomTexture == null)
            {
                phantomTexture = content.Load<Texture2D>("Graphics/Ships/Boss2");
            }
        }

        public override void Update(GameTime gameTime)
        {
            CollidedObjects.Clear();
            base.Update(gameTime);
        }


        public void OuterCollision(GameObject sender, CollisionEventArgs e)
        {
            if (OnOuterCollision != null)
                OnOuterCollision(sender, e);
        }
        public void InnerCollision(GameObject sender, CollisionEventArgs e)
        {
            if (OnOuterCollision != null)
                OnOuterCollision(sender, e);
        }

        public void SetTexture(Texture2D newTexture)
        {
            Texture = newTexture;
        }

        // Put explosion here
        public void CheckForDeath()
        {
            if (Health < 1)
            {
                thisScene.Score += pointValue * thisScene.ScoreMultiplier;
                thisScene.GainExperience(Math.Min(pointValue / 75, 5));

                // Explosion
                if (usesSimpleExplosion)
                {
                    thisScene.PlayAnimation(new Animation(GameScene.ExplosionTexture, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 48, 20f, new Vector2(Center.X - 24, Center.Y - 25), false));
                    AudioManager.PlaySoundEffect(GameScene.Explosion1Sound, .6f);
                    Destroy();
                }
                else
                {
                    scriptManager.Execute(CustomExplosion, this);
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

    public enum EnemyType
    {
        Slicer,
        Tortoise,
        Dragonfly,
        Komodo,
        Phantom
    }
}
