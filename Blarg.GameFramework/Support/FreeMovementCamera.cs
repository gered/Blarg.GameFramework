using System;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Input;

namespace Blarg.GameFramework.Support
{
	public class FreeMovementCamera : Camera
	{
		Vector3 _movement;

		public FreeMovementCamera(ViewContext viewContext)
			: base(viewContext)
		{
		}

		public void Move(float x, float y, float z)
		{
			Position = new Vector3(Position.X + x, Position.Y + y, Position.Z + z);
		}

		public void Orient(float x, float y, float z)
		{
			Orientation = new Vector3(Orientation.X + x, Orientation.Y + y, Orientation.Z + z);
		}

		public override void OnUpdate(float delta)
		{
			float pointerDeltaX = 0.0f;
			float pointerDeltaY = 0.0f;
			bool isPointerTouching = false;

			if (Platform.Mouse != null && Platform.Mouse.IsDown(MouseButton.Left))
			{
				pointerDeltaX = Platform.Mouse.DeltaX;
				pointerDeltaY = Platform.Mouse.DeltaY;
				isPointerTouching = true;
			}
			else if (Platform.TouchScreen != null && Platform.TouchScreen.IsTouching)
			{
				pointerDeltaX = Platform.TouchScreen.PrimaryPointer.DeltaX;
				pointerDeltaY = Platform.TouchScreen.PrimaryPointer.DeltaY;
				isPointerTouching = true;
			}


			if (isPointerTouching)
			{
				var orientation = Orientation;

				orientation.Y += pointerDeltaX * 0.01f;
				orientation.X += pointerDeltaY * 0.01f;

				orientation.Y = MathHelpers.RolloverClamp(orientation.Y, MathConstants.Radians0, MathConstants.Radians360);
				orientation.X = MathHelpers.RolloverClamp(orientation.X, MathConstants.Radians0, MathConstants.Radians360);

				Orientation = orientation;
			}

			_movement = Vector3.Zero;

			if (Platform.Keyboard.IsDown(Key.Up))
				_movement.Z -= delta * 6.0f;
			if (Platform.Keyboard.IsDown(Key.Down))
				_movement.Z += delta * 6.0f;
			if (Platform.Keyboard.IsDown(Key.Left))
				_movement.X -= delta * 6.0f;
			if (Platform.Keyboard.IsDown(Key.Right))
				_movement.X += delta * 6.0f;

			UpdateLookAtMatrix(ref _movement);
		}
	}
}
