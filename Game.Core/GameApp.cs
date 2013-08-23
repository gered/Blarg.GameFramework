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
			_camera = new FreeMovementCamera(Framework.GraphicsDevice.ViewContext);
			_camera.Position = new Vector3(0.0f, 5.0f, 0.0f);
			Framework.GraphicsDevice.ViewContext.Camera = _camera;

			_grid = new FlatWireframeGrid(Framework.GraphicsDevice, 32, 32);

			_spriteBatch = new SpriteBatch(Framework.GraphicsDevice);
		}

		public void OnUnload()
		{
			Framework.GraphicsDevice.ViewContext.Camera = null;
		}

		public void OnLostContext()
		{
		}

		public void OnNewContext()
		{
		}

		public void OnRender(float delta)
		{
			Framework.GraphicsDevice.Clear(0.25f, 0.5f, 1.0f, 1.0f);

			var shader = Framework.GraphicsDevice.SimpleColorShader;
			Framework.GraphicsDevice.BindShader(shader);
			shader.SetModelViewMatrix(Framework.GraphicsDevice.ViewContext.ModelViewMatrix);
			shader.SetProjectionMatrix(Framework.GraphicsDevice.ViewContext.ProjectionMatrix);
			_grid.Render();
			Framework.GraphicsDevice.UnbindShader();

			long gcmem = GC.GetTotalMemory(false);
			_sb.Clear();
			_sb.Append("GC Mem Usage: ").AppendNumber((int)gcmem).Append('\n')
				.Append("FPS: ").AppendNumber(Framework.Application.FPS)
				.Append(", ").AppendNumber(Framework.Application.FrameTime).Append(" ms")
				.Append(", RT: ").AppendNumber(Framework.Application.RenderTime).Append(" (").AppendNumber(Framework.Application.RendersPerSecond).Append(")")
				.Append(", UT: ").AppendNumber(Framework.Application.UpdateTime).Append(" (").AppendNumber(Framework.Application.UpdatesPerSecond).Append(")")
				.Append(", RD: ").AppendNumber(delta);

			_spriteBatch.Begin();
			_spriteBatch.Render(Framework.GraphicsDevice.SansSerifFont, 10, 10, Color.White, _sb);
			_spriteBatch.End();
		}

		public void OnResize(ScreenOrientation orientation, Rect size)
		{
		}

		public void OnUpdate(float delta)
		{
			if (Framework.Keyboard.IsPressed(Blarg.GameFramework.Input.Key.Escape))
				Framework.Application.Quit();
			_camera.OnUpdate(delta);
		}

		public void Dispose()
		{
		}
	}
}

