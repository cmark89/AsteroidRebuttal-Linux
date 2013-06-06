using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace AsteroidRebuttal.Controls
{
    public static class KeyboardManager
    {
        #region Fields


        // Stores last frame's KeyboardState
        private static KeyboardState lastState;

        //Stores this frame's KeyboardState
        private static KeyboardState currentState;

        #endregion

        // Update the keyboard state
        public static void Update(GameTime gameTime)
        {
            lastState = currentState;
            currentState = Keyboard.GetState();
        }


        // Returns true if the given key is being held down
        public static bool KeyDown(Keys key)
        {
            return currentState.IsKeyDown(key);
        }


        // Returns true if the given key is not being held down
        public static bool KeyUp(Keys key)
        {
            return currentState.IsKeyUp(key);
        }


        // Returns true if the given key was down last frame but is up this frame, triggering on key release.
        public static bool KeyPressedUp(Keys key)
        {
            return (lastState.IsKeyDown(key) && currentState.IsKeyUp(key));
        }

        // Returns true if the given key was down last frame but is up this frame, triggering on key release.
        public static bool KeyPressedDown(Keys key)
        {
            return (lastState.IsKeyUp(key) && currentState.IsKeyDown(key));
        }
    }
}
