using System;
using System.Text;
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
		SpriteBatch _spriteBatch;
		StringBuilder _sb = new StringBuilder(1024);

		public GameApp()
		{
		}

		public void OnInit()
		{
		}

		public void OnShutdown()
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

			_spriteBatch = new SpriteBatch(Platform.GraphicsDevice);
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

			long gcmem = GC.GetTotalMemory(false);
			_sb.Clear();
			_sb.Append("GC Mem Usage: ").AppendNumber((int)gcmem).Append('\n')
				.Append("FPS: ").AppendNumber(Platform.Application.FPS)
				.Append(", ").AppendNumber(Platform.Application.FrameTime).Append(" ms")
				.Append(", RT: ").AppendNumber(Platform.Application.RenderTime).Append(" (").AppendNumber(Platform.Application.RendersPerSecond).Append(")")
				.Append(", UT: ").AppendNumber(Platform.Application.UpdateTime).Append(" (").AppendNumber(Platform.Application.UpdatesPerSecond).Append(")")
				.Append(", RD: ").AppendNumber(delta);

			_spriteBatch.Begin();
			_spriteBatch.Render(Platform.GraphicsDevice.SansSerifFont, 10, 10, Color.White, _sb);
			_spriteBatch.End();
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

