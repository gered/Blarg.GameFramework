using System;
using Blarg.GameFramework;
using Blarg.GameFramework.Events;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Graphics.Helpers;
using Blarg.GameFramework.Graphics.ScreenEffects;
using Blarg.GameFramework.Input;
using Blarg.GameFramework.States;
using Blarg.GameFramework.Support;

namespace Game
{
	public class TestGameState : GameState
	{
		FlatWireframeGrid _grid;
		EulerPerspectiveCamera _camera;

		public TestGameState(StateManager stateManager, EventManager eventManager)
			: base(stateManager, eventManager)
		{
		}

		public override void OnPush()
		{
			_camera = new EulerPerspectiveCamera(Framework.GraphicsDevice.ViewContext);
			_camera.Position = new Vector3(0.0f, 5.0f, 0.0f);
			Framework.GraphicsDevice.ViewContext.Camera = _camera;

			_grid = new FlatWireframeGrid(Framework.GraphicsDevice, 32, 32);

			ProcessManager.Add<DebugInfoProcess>();
		}

		public override void OnPop()
		{
			Framework.GraphicsDevice.ViewContext.Camera = null;
		}

		public override void OnRender(float delta)
		{
			Framework.GraphicsDevice.Clear(0.25f, 0.5f, 1.0f, 1.0f);

			var shader = Framework.GraphicsDevice.SimpleColorShader;
			Framework.GraphicsDevice.BindShader(shader);
			shader.SetModelViewMatrix(Framework.GraphicsDevice.ViewContext.ModelViewMatrix);
			shader.SetProjectionMatrix(Framework.GraphicsDevice.ViewContext.ProjectionMatrix);
			_grid.Render();
			Framework.GraphicsDevice.UnbindShader();

			base.OnRender(delta);
		}

		public override void OnUpdate(float delta)
		{
			base.OnUpdate(delta);
			if (Framework.Keyboard.IsPressed(Blarg.GameFramework.Input.Key.Escape))
				Framework.Application.Quit();

			float pointerDeltaX = 0.0f;
			float pointerDeltaY = 0.0f;
			bool isPointerTouching = false;

			if (Framework.Mouse != null && Framework.Mouse.IsDown(MouseButton.Left))
			{
				pointerDeltaX = Framework.Mouse.DeltaX;
				pointerDeltaY = Framework.Mouse.DeltaY;
				isPointerTouching = true;
			}
			else if (Framework.TouchScreen != null && Framework.TouchScreen.IsTouching)
			{
				pointerDeltaX = Framework.TouchScreen.PrimaryPointer.DeltaX;
				pointerDeltaY = Framework.TouchScreen.PrimaryPointer.DeltaY;
				isPointerTouching = true;
			}


			if (isPointerTouching)
			{
				_camera.Yaw += pointerDeltaX * 0.01f;
				_camera.Pitch += pointerDeltaY * 0.01f;

				_camera.Pitch = MathHelpers.RolloverClamp(_camera.Pitch, MathConstants.Radians0, MathConstants.Radians360);
				_camera.Yaw = MathHelpers.RolloverClamp(_camera.Yaw, MathConstants.Radians0, MathConstants.Radians360);
			}

			var movement = Vector3.Zero;

			if (Framework.Keyboard.IsDown(Key.Up))
				movement.Z -= delta * 6.0f;
			if (Framework.Keyboard.IsDown(Key.Down))
				movement.Z += delta * 6.0f;
			if (Framework.Keyboard.IsDown(Key.Left))
				movement.X -= delta * 6.0f;
			if (Framework.Keyboard.IsDown(Key.Right))
				movement.X += delta * 6.0f;

			_camera.Position += Quaternion.Transform(_camera.Rotation, movement);
		}
	}
}

