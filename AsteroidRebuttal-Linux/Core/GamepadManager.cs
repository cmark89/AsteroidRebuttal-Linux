using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AsteroidRebuttal.Core
{
	public class GamepadManager
	{
		// Stores last frame's state
        private static GamePadState lastState;	// Why am I even storing this?

        //Stores this frame's state
        private static GamePadState currentState;

		public static bool Connected 
		{ 
			get { return currentState.IsConnected; } 
		}

        // Update the gamepad state
        public static void Update(GameTime gameTime)
        {
            lastState = currentState;
            currentState = GamePad.GetState(PlayerIndex.One);
        }

		// Why aren't these properties?
		public static bool AButtonDown()
		{
			return (currentState.Buttons.A == ButtonState.Pressed);
		}
		public static bool BButtonDown()
		{
			return (currentState.Buttons.B == ButtonState.Pressed);
		}
		public static bool XButtonDown()
		{
			return (currentState.Buttons.X == ButtonState.Pressed);
		}
		public static bool YButtonDown()
		{
			return (currentState.Buttons.Y == ButtonState.Pressed);
		}
		public static bool LeftButtonDown()
		{
			return (currentState.DPad.Left == ButtonState.Pressed);
		}
		public static bool UpButtonDown()
		{
			return (currentState.DPad.Up == ButtonState.Pressed);
		}
		public static bool RightButtonDown()
		{
			return (currentState.DPad.Right == ButtonState.Pressed);
		}
		public static bool DownButtonDown()
		{
			return (currentState.DPad.Down == ButtonState.Pressed);
		}
		public static bool StartButtonDown()
		{
			return (currentState.Buttons.Start == ButtonState.Pressed);
		}
		public static bool AnyShoulderButtonDown()
		{
			return (currentState.Buttons.LeftShoulder == ButtonState.Pressed || currentState.Buttons.RightShoulder == ButtonState.Pressed);
		}

		public static bool ProceedButtonDown()
		{
			return (XButtonDown() || YButtonDown() || AButtonDown() || BButtonDown() || StartButtonDown());
		}

		public static bool LeftButtonPressedUp()
		{
			return (currentState.DPad.Left == ButtonState.Released && lastState.DPad.Left == ButtonState.Pressed);
		}
		public static bool UpButtonPressedUp()
		{
			return (currentState.DPad.Up == ButtonState.Released && lastState.DPad.Up == ButtonState.Pressed);
		}
		public static bool RightButtonPressedUp()
		{
			return (currentState.DPad.Right == ButtonState.Released && lastState.DPad.Right == ButtonState.Pressed);
		}
		public static bool DownButtonPressedUp()
		{
			return (currentState.DPad.Down == ButtonState.Released && lastState.DPad.Down == ButtonState.Pressed);
		}


		public static float LeftStickX()
		{
			return(currentState.ThumbSticks.Left.X);
		}
		public static float LeftStickY()
		{
			return(currentState.ThumbSticks.Left.Y);
		}
	}
}

