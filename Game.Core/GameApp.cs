using System;
using PortableGL;
using Blarg.GameFramework;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Graphics.Helpers;
using Blarg.GameFramework.Support;

namespace Game
{
	public class GameApp : IGameApp
	{
		FlatWireframeGrid _grid;
		FreeMovementCamera _camera;

		public GameApp()
		{
		}

		public void OnAppGainFocus()
		{
		}

		public void OnAppLostFocus()
		{
		}

		public void OnAppPause()
		{
		}

		public void OnAppResume()
		{
		}

		public void OnLoad()
		{
			_camera = new FreeMovementCamera(Platform.GraphicsDevice.ViewContext);
			_camera.Position = new Vector3(0.0f, 5.0f, 0.0f);
			Platform.GraphicsDevice.ViewContext.Camera = _camera;

			_grid = new FlatWireframeGrid(Platform.GraphicsDevice, 32, 32);
		}

		public void OnUnload()
		{
			Platform.GraphicsDevice.ViewContext.Camera = null;
		}

		public void OnLostContext()
		{
		}

		public void OnNewContext()
		{
		}

		public void OnRender(float delta)
		{
			Platform.GraphicsDevice.Clear(0.25f, 0.5f, 1.0f, 1.0f);

			var shader = Platform.GraphicsDevice.SimpleColorShader;
			Platform.GraphicsDevice.BindShader(shader);
			shader.SetModelViewMatrix(Platform.GraphicsDevice.ViewContext.ModelViewMatrix);
			shader.SetProjectionMatrix(Platform.GraphicsDevice.ViewContext.ProjectionMatrix);
			_grid.Render();
			Platform.GraphicsDevice.UnbindShader();
		}

		public void OnResize(ScreenOrientation orientation, Rect size)
		{
		}

		public void OnUpdate(float delta)
		{
			if (Platform.Keyboard.IsPressed(Blarg.GameFramework.Input.Key.Escape))
				Platform.Application.Quit();
			_camera.OnUpdate(delta);
		}

		public void Dispose()
		{
		}
	}
}

