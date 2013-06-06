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
    // The basic game object class from which all objects inherit
    public abstract class GameObject
    {

        #region Core Fields
        // The texture of the game object
        public Texture2D Texture { get; protected set; }
        public Color Color { get; set; }
        public GameScene thisScene;
        public List<GameObject> ChildObjects { get; protected set; }
        public float DrawLayer { get; set; }
        public static int ObjectCount = 0;
        #endregion

        #region Position, Hitboxes, Collision
        // The parent object, if it exists
        public GameObject Parent { get; set; }

        // The position of the game object in world space
        private Vector2 _position;
        public virtual Vector2 Position 
        {
            get { return _position; }

            set
            {
                _position = value;
                if (Hitbox != null)
                {
                    Hitbox.Center = Center;
                }
                if (InnerHitbox != null)
                {
                    InnerHitbox.Center = Position + _innerHitboxOffset;
                }
            }
        }

        public Vector2 Origin { get; set; }

        public Vector2 Center
        {
            get { return Position + Origin; }
            set
            {
                Position = value - Origin;
            }
        }

        public Circle Hitbox { get; protected set; }

        public bool UsesInnerHitbox { get; protected set; }
        protected Vector2 _innerHitboxOffset;
        private Circle _innerHitbox;
        public Circle InnerHitbox 
        {
            get
            {
                return _innerHitbox;
            }
            protected set
            {
                _innerHitbox = value;
                _innerHitboxOffset = value.Center;
            }
        }

        // Stores the object's position on the last frame.
        public Vector2 LastPosition { get; protected set; }

        // Stores the vector of movement
        public Vector2 MovementVector { get; protected set; }

        // Phasing prevents collision occuring with this object.
        public bool Phasing;

        // This is the layer on which this object will send collision.
        public int CollisionLayer;

        public bool LockedToParentPosition { get; set; }
        public Vector2 LockPositionOffset { get; set; }
        public bool LockedToParentRotation { get; set; }

        public Vector2 DeletionBoundary { get; set; }

        #endregion

        #region Game Physics
        // Angle of rotation in radians
        private float _rotation;
        public float Rotation 
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                if (DrawAtTrueRotation)
                    DrawRotation = value;
            }
        }

        // Angle of the object's draw rotation.
        public virtual float DrawRotation { get; set; }

        // If this is true, DrawRotation will be updated to match Rotation.  Otherwise it must be set separately.
        public bool DrawAtTrueRotation { get; set; }

        // How fast the object is moving
        public float Velocity { get; set; }
        // How fast the object is spinning
        public float AngularVelocity { get; set; }
        // How much Velocity is changing over time
        public float Acceleration { get; set; }
        // How much AngularVelocity is changing over time
        public float AngularAcceleration { get; set; }
        #endregion

        #region Private Lerping Properties
        // I know there's a better way to do this, but for now this will suffice
        bool lerpingPosition;
        double positionLerpElapsedTime;
        double positionLerpDuration;
        Vector2 startPosition;
        Vector2 targetPosition;

        bool lerpingRotation;
        double rotationLerpElapsedTime;
        double rotationLerpDuration;
        float startRotation;
        float targetRotation;

        bool lerpingVelocity;
        double velocityLerpElapsedTime;
        double velocityLerpDuration;
        float startVelocity;
        float targetVelocity;

        bool lerpingAngularVelocity;
        double angularVelocityLerpElapsedTime;
        double angularVelocityLerpDuration;
        float startAngularVelocity;
        float targetAngularVelocity;

        bool lerpingColor;
        double colorLerpElapsedTime;
        double colorLerpDuration;
        Color startColor;
        Color targetColor;

        #endregion

        #region Management
        // Fields for Add/Remove management
        public bool FlaggedForRemoval { get; set; }
        public bool IsNewObject = true;
        #endregion

        #region Miscellaneous
        public float CustomValue1 { get; set; }
        public float CustomValue2 { get; set; }
        public float CustomValue3 { get; set; }
        public float CustomValue4 { get; set; }

        // Controls the order objects are drawn in
        public int DrawPriority { get; protected set; }
        #endregion

        public virtual void Initialize()
        {
            ChildObjects = new List<GameObject>();

            // If an outerhitbox does not yet exist, set it equal to the "true" hitbox.
            if (UsesInnerHitbox && InnerHitbox == null)
            {
                InnerHitbox = Hitbox;
            }

            if (thisScene != null)
            {
                thisScene.AddGameObject(this);
            }

            if (Texture != null && Origin == null)
            {
                Origin = new Vector2(Texture.Width/2, Texture.Height/2);
            }

            if (DeletionBoundary == null)
            {
                DeletionBoundary = new Vector2(Hitbox.Radius, Hitbox.Radius);
            }

            if (Color == null)
            {
                Color = Color.White;
            }

            ObjectCount++;
            DrawLayer -= ObjectCount * .0000001f;
        }


        public virtual void Update(GameTime gameTime)
        {
            // Check if object is off screen and remove it if it is.
            CheckIfOffScreen();

            // Update the last position no matter what.
            LastPosition = Position;

            if (Parent != null && LockedToParentRotation)
            {
                Rotation = Parent.Rotation;
            }
            else
            {
                // If angular acceleration is not 0, change the angular velocity of the object
                if (AngularAcceleration != 0)
                {
                    AngularVelocity += AngularAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                // If angular velocity is not 0, change the rotation of the object
                if (AngularVelocity != 0)
                {
                    Rotation += AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            // If the object has a parent and its position is locked to it...
            if (Parent != null && LockedToParentPosition)
            {
                // Set the position to the parent.
                Center = Parent.Center + LockPositionOffset;
            }
            else
            {
                // If acceleration exists, modify velocity by it
                if (Acceleration != 0)
                {
                    Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                // Move the object forward by its velocity if it is not equal to 0
                if (Velocity != 0)
                {
                    // Create a new vector to store movement
                    Vector2 movement = new Vector2();

                    movement.X = (float)Math.Cos(Rotation) * (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    movement.Y = (float)Math.Sin(Rotation) * (Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    // Set the movement vector
                    MovementVector = movement;

                    // Actually move the object
                    Position += MovementVector;
                }
            }
            

            if (lerpingPosition || lerpingRotation || lerpingVelocity || lerpingAngularVelocity || lerpingColor)
                LerpUpdate(gameTime);
        }


        private void CheckIfOffScreen()
        {
            // Check if object is off screen and remove it if it is.
            if (Hitbox == null)
                return;

            if (Hitbox.Center.X + Hitbox.Radius < thisScene.ScreenArea.X - DeletionBoundary.X ||
                Hitbox.Center.X - Hitbox.Radius > thisScene.ScreenArea.Width + DeletionBoundary.X ||
                Hitbox.Center.Y + Hitbox.Radius < thisScene.ScreenArea.Y - DeletionBoundary.Y ||
                Hitbox.Center.Y - Hitbox.Radius > thisScene.ScreenArea.Height + DeletionBoundary.Y)
            {
                Destroy();
            }
        }

        // Lerp values that are changing over time
        public void LerpUpdate(GameTime gameTime)
        {
            if (lerpingPosition)
            {
                positionLerpElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 newPos = new Vector2();
                newPos.X = MathHelper.Lerp(startPosition.X, targetPosition.X, (float)positionLerpElapsedTime / (float)positionLerpDuration);
                newPos.Y = MathHelper.Lerp(startPosition.Y, targetPosition.Y, (float)positionLerpElapsedTime / (float)positionLerpDuration);

                Center = newPos;

                if (positionLerpElapsedTime >= positionLerpDuration)
                    lerpingPosition = false;
            }

            if (lerpingRotation)
            {
                rotationLerpElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                Rotation = MathHelper.Lerp(startRotation, targetRotation, (float)rotationLerpElapsedTime / (float)rotationLerpDuration);              

                if (rotationLerpElapsedTime >= rotationLerpDuration)
                    lerpingRotation = false;
            }

            if (lerpingVelocity)
            {
                velocityLerpElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                Velocity = MathHelper.Lerp(startVelocity, targetVelocity, (float)velocityLerpElapsedTime / (float)velocityLerpDuration);


                if (velocityLerpElapsedTime >= velocityLerpDuration)
                    lerpingVelocity = false;
            }

            if (lerpingAngularVelocity)
            {
                angularVelocityLerpElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                AngularVelocity = MathHelper.Lerp(startAngularVelocity, targetAngularVelocity, (float)angularVelocityLerpElapsedTime / (float)angularVelocityLerpDuration);


                if (angularVelocityLerpElapsedTime >= angularVelocityLerpDuration)
                    lerpingAngularVelocity = false;
            }

            if (lerpingColor)
            {
                colorLerpElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                Color = Color.Lerp(startColor, targetColor, (float)colorLerpElapsedTime / (float)colorLerpDuration);

                if (colorLerpElapsedTime >= colorLerpDuration)
                    lerpingColor = false;
            }
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Draw the sprite simply.
            if (!DrawAtTrueRotation)
            {
                spriteBatch.Draw(Texture, Position, null, Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, DrawLayer);
            }
            else
            {
                spriteBatch.Draw(Texture, Center, null, Color, Rotation - (float)Math.PI / 2, Origin, 1f, SpriteEffects.None, DrawLayer);
            }
        }

        public void LerpPosition(Vector2 newPosition, float duration)
        {
            lerpingPosition = true;
            startPosition = Center;
            targetPosition = newPosition;
            positionLerpDuration = duration;
            positionLerpElapsedTime = 0f;
        }

        public void LerpRotation(float newRotation, float duration)
        {
            lerpingRotation = true;
            startRotation = Rotation;
            targetRotation = newRotation;
            rotationLerpDuration = duration;
            rotationLerpElapsedTime = 0f;
        }

        public void LerpVelocity(float newVelocity, float duration)
        {
            if (AsteroidRebuttal.ManicMode && this is Bullet)
                newVelocity *= 2;

            lerpingVelocity = true;
            startVelocity = Velocity;
            targetVelocity = newVelocity;
            velocityLerpDuration = duration;
            velocityLerpElapsedTime = 0f;
        }

        public void LerpAngularVelocity(float newAngularVelocity, float duration)
        {
            lerpingAngularVelocity = true;
            startAngularVelocity = AngularVelocity;
            targetAngularVelocity = newAngularVelocity;
            angularVelocityLerpDuration = duration;
            angularVelocityLerpElapsedTime = 0f;
        }

        public void LerpColor(Color newColor, float duration)
        {
            lerpingColor = true;
            startColor = Color;
            targetColor = newColor;
            colorLerpDuration = duration;
            colorLerpElapsedTime = 0f;
        }

        
        public void SetCollisionLayer(int newLayer)
        {
            CollisionLayer = newLayer;
        }


        public virtual void Destroy()
        {
            ObjectCount--;
            FlaggedForRemoval = true;
            foreach (GameObject go in ChildObjects)
                go.Destroy();
        }

        public void SetParent(GameObject newParent)
        {
            Parent = newParent;
        }
    }
}
