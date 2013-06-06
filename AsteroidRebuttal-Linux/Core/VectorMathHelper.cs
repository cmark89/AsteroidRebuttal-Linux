using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidRebuttal.Scenes;
using Microsoft.Xna.Framework;

namespace AsteroidRebuttal.Core
{
    public class VectorMathHelper
    {
        static Random rand = new Random();
        public static float GetAngleTo(Vector2 from, Vector2 to)
        {
            if (from == to)
                return 0f;

            // First, normalize the two vectors.
            to -= from;
            from = new Vector2(1, 0);
            to.Normalize();

            float angle = (float)Math.Atan2(to.Y, to.X) - (float)Math.Atan2(from.Y, from.X);
            return angle;
        }

        public static float GetAngleTo(Vector2 from, Vector2 to, float randomSpread)
        {
            float spread = DegreesToRadians(randomSpread);
            // Gets a random between 1 and -1
            float randomAngle = (((float)rand.NextDouble() * 2) - 1f) * spread / 2;
        
            return GetAngleTo(from, to) + randomAngle;
        }

        // Returns a random angle.
        public static float GetRandom()
        {
            return (float)rand.NextDouble() * ((float)Math.PI * 2);
        }

        // Returns a random angle between the given angles.
        public static float GetRandomBetween(float ang1, float ang2)
        {
            float minAngle = Math.Min(ang1, ang2);
            float maxAngle = Math.Max(ang1, ang2);

            float difference = maxAngle - minAngle;
            difference *= (float)rand.NextDouble();

            return minAngle + difference;
        }

        // This is like the previous get angle to function, except that instead of getting a random spread, 
        // it will instead get a straight angle to a point given within a certain area of the target.
        public static float GetAngleToWithinArea(Vector2 from, Vector2 to, float randomRadius)
        {
            return 0;
        }

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }
    }
}
