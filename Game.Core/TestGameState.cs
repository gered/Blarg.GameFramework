using System;
using Blarg.GameFramework;
using Blarg.GameFramework.Events;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Graphics.Helpers;
using Blarg.GameFramework.Graphics.ScreenEffects;
using Blarg.GameFramework.States;
using Blarg.GameFramework.Support;

namespace Game
{
	public class TestGameState : GameState
	{
		FlatWireframeGrid _grid;
		FreeMovementCamera _camera;

		public TestGameState(StateManager stateManager, EventManager eventManager)
			: base(stateManager, eventManager)
		{
		}

		public override void OnPush()
		{
			_camera = new FreeMovementCamera(Framework.GraphicsDevice.ViewContext);
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
			_camera.OnUpdate(delta);
		}
	}
}

