using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidRebuttal.GameObjects;

namespace AsteroidRebuttal.Core
{
    public delegate void CollisionEventHandler (GameObject sender, CollisionEventArgs e);

    public interface ICollidable
    {
        List<GameObject> CollidedObjects {get; set;}
        int[] CollidesWithLayers { get; set; }
        event CollisionEventHandler OnOuterCollision;
        event CollisionEventHandler OnInnerCollision;

        void OuterCollision(GameObject sender, CollisionEventArgs e);
        void InnerCollision(GameObject sender, CollisionEventArgs e);
    }


    public enum CollisionLayers
    {
        EnemyBullet = 0,
        EnemyShip = 1,
        PlayerBullet = 2,
        PlayerShip = 3
    }

    public class CollisionEventArgs
    {
        // The other object collided with
        public GameObject otherObject;
        // The layer on which the collision occured.
        public int collisionLayer;

        public CollisionEventArgs(GameObject other, int layer)
        {
            otherObject = other;
            collisionLayer = layer;
        }
    }
}
