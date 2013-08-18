using System;
using PortableGL;
using Blarg.GameFramework;
using Blarg.GameFramework.Graphics;

namespace Game
{
	public class GameApp : IGameApp
	{
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
		}

		public void OnUnload()
		{
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
		}

		public void OnResize(ScreenOrientation orientation, Rect size)
		{
		}

		public void OnUpdate(float delta)
		{
			if (Platform.Keyboard.IsPressed(Blarg.GameFramework.Input.Key.Escape))
				Platform.Application.Quit();
		}

		public void Dispose()
		{
		}
	}
}

