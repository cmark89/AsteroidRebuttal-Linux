using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidRebuttal.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AsteroidRebuttal.Core
{
    public class QuadTree
    {
        // Maximum number of objects that can exist in this tree before it splits
        private int maxObjects = 10;
        // Maximum number of times the tree can split
        private int maxLevel = 7;

        // The current level of the tree
        private int level;
        // List that stores the objects in this level of the tree
        private List<GameObject> gameObjects;
        // The area of this level of the tree
        private Rectangle bounds;
        // Subtrees; 0 - 3 from top left (reading style)
        private QuadTree[] subnodes;

        public static Texture2D texture;
        static SpriteFont spriteFont;

        public QuadTree(int newLevel, Rectangle newBounds)
        {
            level = newLevel;
            bounds = newBounds;

            gameObjects = new List<GameObject>();
            subnodes = new QuadTree[4];
        }

        // Clears the QuadTree and all subnodes recursively.
        public void Clear()
        {
            gameObjects.Clear();

            if (subnodes[0] != null)
            {
                for (int i = 0; i < subnodes.Length; i++)
                {
                    subnodes[i].Clear();
                    subnodes[i] = null;
                }
            }
             
        }


        // Splits the tree into four subnodes
        public void Split()
        {
            int subWidth = bounds.Width / 2;
            int subHeight = bounds.Height / 2;
            int subX = bounds.X;
            int subY = bounds.Y;

            subnodes[0] = new QuadTree(level + 1, new Rectangle(subX, subY, subWidth, subHeight));
            subnodes[1] = new QuadTree(level + 1, new Rectangle(subX + subWidth, subY, subWidth, subHeight));
            subnodes[2] = new QuadTree(level + 1, new Rectangle(subX, subY + subHeight, subWidth, subHeight));
            subnodes[3] = new QuadTree(level + 1, new Rectangle(subX + subWidth, subY + subHeight, subWidth, subHeight));
        }


        // This may need to be cleaned when hitboxes are updated, but...
        // Gets the index where the object should be placed.  Returns -1 if it cannot fit entirely within a child node and belongs in the parent
        public int GetIndex(GameObject go)
        {
            int index = -1;

            // Find the midpoints of the current section of the tree
            double verticalMidpoint = bounds.X + (bounds.Width / 2);
            double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

            bool inTop = (go.Hitbox.Center.Y - go.Hitbox.Radius < horizontalMidpoint && go.Hitbox.Center.Y + go.Hitbox.Radius < horizontalMidpoint);
            bool inBottom = (go.Hitbox.Center.Y - go.Hitbox.Radius > horizontalMidpoint);
            bool inLeft = (go.Hitbox.Center.X - go.Hitbox.Radius < verticalMidpoint && go.Hitbox.Center.X + go.Hitbox.Radius < verticalMidpoint);
            bool inRight = (go.Hitbox.Center.X - go.Hitbox.Radius > verticalMidpoint);

            if (inTop && inLeft)
                index = 0;
            else if (inTop && inRight)
                index = 1;
            else if (inBottom && inLeft)
                index = 2;
            else if (inBottom && inRight)
                index = 3;

            return index;
        }


        // Inserts an object into the current level of the quadtree.  If the limit is exceeded, it will split and insert all items into the new tree.
        public void Insert(GameObject go)
        {
            // If the tree has already split, see where the object should go.
            if (subnodes[0] != null)
            {
                int index = GetIndex(go);

                // If the object fits entirely within a subtree
                if (index != -1)
                {
                    // So let that node figure it out.
                    subnodes[index].Insert(go);
                    return;
                }
            }

            // Add the object, because either the tree has not yet split or the object can't fit in any subnode.
            gameObjects.Add(go);

            // If it's time to split!
            if (gameObjects.Count > maxObjects && level < maxLevel)
            {
                // Split the tree if it has not yet split
                if (subnodes[0] == null)
                {
                    Split();

                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        int index = GetIndex(gameObjects[i]);

                        if (index != -1)
                        {
                            subnodes[index].Insert(gameObjects[i]);
                            gameObjects.RemoveAt(i);
                        }
                    }
                }
            }
        }

        // Returns all gameObjects that the object could possible collide with.
        public List<GameObject> Retrieve(GameObject go)
        {
            List<GameObject> returnedObjects = new List<GameObject>();

            int index = GetIndex(go);

            // The tree has split and the object is completely enclosed...
            if (index != -1 && subnodes[0] != null)
            {
                returnedObjects = subnodes[index].Retrieve(go);
            }

            if (index == -1 && subnodes[0] != null)
            {
                for (int i = 0; i < subnodes.Length; i++)
                    returnedObjects.AddRange(subnodes[i].Retrieve(go));
            }

            returnedObjects.AddRange(gameObjects);

            return returnedObjects;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont, gameObjects.Count.ToString(), new Vector2(bounds.X + (bounds.Width / 2f), bounds.Y + (bounds.Height / 2f)), Color.Red);
            spriteBatch.Draw(texture, bounds, Color.White);

            if (subnodes[0] != null)
            {
                foreach (QuadTree qt in subnodes)
                {
                    if(qt != null)
                        qt.Draw(spriteBatch);
                }
            }   
        }

        

        public static void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Graphics/quadtree");
            spriteFont = content.Load<SpriteFont>("Graphics/QuadtreeFont");
        }
    }
}
