using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ObjectivelyRadical.Controls
{
    public static class MouseManager
    {
        #region Fields

        // Stores last frame's MouseState.
        private static MouseState lastState;

        // Stores this frame's MouseState
        private static MouseState currentState;

        // Exposes the mouse's current X and Y position
        public static Vector2 MousePosition
        {
            get { return new Vector2(currentState.X, currentState.Y); }
            set { return; }
        }

        #endregion

        // Updates the MouseStates
        public static void Update(GameTime gameTime)
        {
            lastState = currentState;
            currentState = Mouse.GetState();
        }


        // Returns true if the left mouse button is currently down
        public static bool LeftButtonDown()
        {
            return (currentState.LeftButton == ButtonState.Pressed);
        }


        // Returns true if the right mouse button is currently down
        public static bool RightButtonDown()
        {
            return (currentState.RightButton == ButtonState.Pressed);
        }


        // Returns true if the left mouse button was down during the last frame and is up this frame
        public static bool LeftClick()
        {
            return (lastState.LeftButton == ButtonState.Pressed && currentState.LeftButton == ButtonState.Released);
        }


        // Returns true if the right mouse button was down during the last frame and is up this frame
        public static bool RightClick()
        {
            return (lastState.RightButton == ButtonState.Pressed && currentState.RightButton == ButtonState.Released);
        }
    }
}
