using System;
using Blarg.GameFramework.Graphics;

namespace Blarg.GameFramework
{
	public abstract class BaseLooper : ILooper
	{
		IGameApp _gameApp;

		public IGameApp GameApp
		{
			get
			{
				return _gameApp;
			}
			protected set
			{
				_gameApp = value;
			}
		}

		public abstract int FPS { get; }
		public abstract float FrameTime { get; }
		public abstract int RendersPerSecond { get; }
		public abstract int UpdatesPerSecond { get; }
		public abstract int RenderTime { get; }
		public abstract int UpdateTime { get; }

		public abstract void Run(IGameApp gameApp, IPlatformConfiguration config);

		protected void OnAppGainFocus()
		{
			GameApp.OnAppGainFocus();
		}

		protected void OnAppLostFocus()
		{
			GameApp.OnAppLostFocus();
		}

		protected void OnAppPause()
		{
			GameApp.OnAppPause();
		}

		protected void OnAppResume()
		{
			GameApp.OnAppResume();
		}

		protected void OnLoad()
		{
			GameApp.OnLoad();
		}

		protected void OnUnload()
		{
			GameApp.OnUnload();
		}

		protected void OnLostContext()
		{
			GameApp.OnLostContext();
		}

		protected void OnNewContext()
		{
			GameApp.OnNewContext();
		}

		protected void OnRender(float delta)
		{
			GameApp.OnRender(delta);
		}

		protected void OnResize(ScreenOrientation orientation, Rect size)
		{
			GameApp.OnResize(orientation, size);
		}

		protected void OnUpdate(float delta)
		{
			GameApp.OnUpdate(delta);
		}

		public virtual void Dispose()
		{
		}
	}
}

