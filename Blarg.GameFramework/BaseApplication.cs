using System;
using PortableGL;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Input;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework
{
	public abstract class BaseApplication : IApplication
	{
		public const string LOG_TAG = "BASE_APP";

		protected IGameApp GameApp { get; set; }

		public abstract PlatformOS OperatingSystem { get; }
		public abstract PlatformType Type { get; }

		public abstract ILogger Logger { get; }
		public abstract IFileSystem FileSystem { get; }
		public abstract IKeyboard Keyboard { get; }
		public abstract IMouse Mouse { get; }
		public abstract ITouchScreen TouchScreen { get; }
		public abstract IPlatformWindow Window { get; }
		public abstract GL20 GL { get; }

		public int FPS { get; protected set; }
		public float FrameTime { get; protected set; }
		public int RendersPerSecond { get; protected set; }
		public int UpdatesPerSecond { get; protected set; }
		public int RenderTime { get; protected set; }
		public int UpdateTime { get; protected set; }
		public bool IsRunningSlowly { get; protected set; }

		public abstract void Run(IGameApp gameApp, IPlatformConfiguration config);
		public abstract void Quit();

		protected void OnAppGainFocus()
		{
			Logger.Info(LOG_TAG, "OnAppGainFocus");
			GameApp.OnAppGainFocus();
		}

		protected void OnAppLostFocus()
		{
			Logger.Info(LOG_TAG, "OnAppLostFocus");
			GameApp.OnAppLostFocus();
		}

		protected void OnAppPause()
		{
			Logger.Info(LOG_TAG, "OnAppPause");
			GameApp.OnAppPause();
		}

		protected void OnAppResume()
		{
			Logger.Info(LOG_TAG, "OnAppResume");
			GameApp.OnAppResume();
		}

		protected void OnLoad()
		{
			Logger.Info(LOG_TAG, "OnLoad");
			GameApp.OnLoad();
		}

		protected void OnUnload()
		{
			Logger.Info(LOG_TAG, "OnUnload");
			GameApp.OnUnload();
		}

		protected void OnLostContext()
		{
			Logger.Info(LOG_TAG, "OnLostContext");
			GameApp.OnLostContext();
		}

		protected void OnNewContext()
		{
			Logger.Info(LOG_TAG, "OnNewContext");
			GameApp.OnNewContext();
		}

		protected void OnRender(float delta)
		{
			GameApp.OnRender(delta);
		}

		protected void OnResize(ScreenOrientation orientation, Rect size)
		{
			Logger.Info(LOG_TAG, "OnResize");
			GameApp.OnResize(orientation, size);
		}

		protected void OnUpdate(float delta)
		{
			GameApp.OnUpdate(delta);
		}

		public virtual void Dispose()
		{
			Logger.Info(LOG_TAG, "Disposing.");
		}
	}
}

