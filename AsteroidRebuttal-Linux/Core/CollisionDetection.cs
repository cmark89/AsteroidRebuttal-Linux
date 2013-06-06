using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.GameObjects;

namespace AsteroidRebuttal.Core
{
    public class CollisionDetection
    {
        GameScene thisScene;

        public CollisionDetection(GameScene newScene)
        {
            thisScene = newScene;
        }

        // Performs a broad sweep of objects in the game.
        public void BroadphaseCollisionDetection()
        {
            int checks = 0;
            List<CollisionPair> PossibleCollisions = new List<CollisionPair>();

            // Enumerate through ICollidable objects that are not phasing.
            foreach (GameObject ic in thisScene.gameObjects.FindAll(x => x is ICollidable && !x.Phasing))
            {
                ICollidable thisIC = (ICollidable)ic;
                // Next, get all the objects from that object's sector of the quadtree.
                // Exclude all objects further away than 3 times the object's hitbox radius.
                List<GameObject> collisionCandidates = thisScene.quadtree.Retrieve(ic).FindAll(x => !x.Phasing && Vector2.Distance(ic.Hitbox.Center, x.Hitbox.Center) < ic.Hitbox.Radius * 3);

                // For each object returned that is not phasing and that the ICollidable is able to collide with...
                foreach (GameObject go in collisionCandidates.FindAll(x => !x.Phasing && thisIC.CollidesWithLayers.Contains(x.CollisionLayer)))
                {
                    checks++;
                    // Add the pair of objects to the list of narrow checks to be performed after the broad phase.
                    PossibleCollisions.Add(new CollisionPair(thisIC, go));
                }

                // Run the narrow phase if at least one collision is possible.
                if (PossibleCollisions.Count > 0)
                {
                    NarrowphaseCollisionDetection(PossibleCollisions);
                }
            }
        }


        public void NarrowphaseCollisionDetection(List<CollisionPair> collisionPairs)
        {
            foreach (CollisionPair pair in collisionPairs)
            {
                GameObject main = (GameObject)pair.MainCollider;
                GameObject other = pair.SecondaryCollider;

                // Check if the objects overlap
                if (CirclesOverlap(main.Hitbox, other.Hitbox) || LineSegementOverlapsCircle(new LineSegment(other.LastPosition, other.Position), main.Hitbox))
                {
                    // A collision was detected!  Report the collision to the first collision event.
                    pair.MainCollider.OuterCollision(pair.SecondaryCollider, new CollisionEventArgs(pair.SecondaryCollider, pair.SecondaryCollider.CollisionLayer));

                    // Only ICollidables are allowed to implement an inner hitbox, so if it uses it check to see if that collision registers.
                    if (main.UsesInnerHitbox)
                    {
                        if (CirclesOverlap(main.InnerHitbox, other.Hitbox) || LineSegementOverlapsCircle(new LineSegment(other.LastPosition, other.Position), main.InnerHitbox))
                        {
                            // A collision was detected!  Report the collision to the second collision event.
                            pair.MainCollider.InnerCollision(pair.SecondaryCollider, new CollisionEventArgs(pair.SecondaryCollider, pair.SecondaryCollider.CollisionLayer));
                        }
                    }
                }
            }
        }

        public void PrimaryCollisionDetected()
        {
            // See if either of the objects require a more specific check...
        }





        public Vector2 ProjectVector(Vector2 from, Vector2 onto)
        {
            // Gets the squared length of the vector to project to.
            float distance = Vector2.Dot(onto, onto);

            // As long as the distance is greater than 0.
            if (0 < distance)
            {
                // Find the actual dot product of the two values.
                float dotProduct = Vector2.Dot(from, onto);

                // Return the original target vector multiplied by the dotProduct divided by squared distance.
                // Figure out the maths later.
                return onto * (dotProduct / distance);
            }

            // Otherwise, any projection will have length 0, so return the original target vector.
            return onto;
        }



        public bool CirclesOverlap(Circle circle1, Circle circle2)
        {
            return (Vector2.Distance(circle1.Center, circle2.Center) < (circle1.Radius + circle2.Radius));
        }

        public bool LineSegementOverlapsCircle(LineSegment segment, Circle circle)
        {
            if (Vector2.Distance(circle.Center, segment.FromPoint) < circle.Radius || Vector2.Distance(circle.Center, segment.ToPoint) < circle.Radius)
                return true;

            Vector2 segmentLength = segment.FromPoint - segment.ToPoint;
            Vector2 circleLine = circle.Center - segment.FromPoint;
            Vector2 projection = ProjectVector(circleLine, segmentLength);
            Vector2 nearest = segment.FromPoint + projection;

            return (Vector2.Distance(circle.Center, nearest) < circle.Radius && projection.Length() <= segmentLength.Length() && 0 <= Vector2.Dot(projection, segmentLength));
        }
    }



    public class Circle
    {
        public float Radius;
        public Vector2 Center;

        public Circle()
        {
            Center = Vector2.Zero;
            Radius = 0f;
        }

        public Circle(Vector2 newCenter, float radius)
        {
            Radius = radius;
            Center = newCenter;
        }
    }

    public class LineSegment
    {
        public Vector2 FromPoint;
        public Vector2 ToPoint;

        public LineSegment(Vector2 from, Vector2 to)
        {
            FromPoint = from;
            ToPoint = to;
        }
    }


    public class CollisionPair
    {
        public ICollidable MainCollider { get; private set; }
        public GameObject SecondaryCollider { get; private set; }

        public CollisionPair(ICollidable main, GameObject second)
        {
            MainCollider = main;
            SecondaryCollider = second;
        }
    }
}
