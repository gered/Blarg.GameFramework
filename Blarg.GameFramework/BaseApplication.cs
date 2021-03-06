using System;
using System.IO;
using PortableGL;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Input;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework
{
	public abstract class BaseApplication : IApplication
	{
		const string LOG_TAG = "BASE_APP";

		bool _isReleased = false;
		bool _isFirstContextCreatedYet = false;

		protected IGameApp GameApp { get; set; }

		public abstract PlatformOS OperatingSystem { get; }
		public abstract PlatformType PlatformType { get; }

		public abstract ILogger Logger { get; }
		public ServiceContainer Services { get; private set; }
		public abstract IFileSystem FileSystem { get; }
		public abstract IKeyboard Keyboard { get; }
		public abstract IMouse Mouse { get; }
		public abstract ITouchScreen TouchScreen { get; }
		public abstract IPlatformWindow Window { get; }
		public abstract GL20 GL { get; }
		public GraphicsDevice GraphicsDevice { get; private set; }

		public int FPS { get; protected set; }
		public float FrameTime { get; protected set; }
		public int RendersPerSecond { get; protected set; }
		public int UpdatesPerSecond { get; protected set; }
		public int RenderTime { get; protected set; }
		public int UpdateTime { get; protected set; }
		public bool IsRunningSlowly { get; protected set; }

		public abstract void Run(IGameApp gameApp, IPlatformConfiguration config);
		public abstract void Quit();

		public abstract IPlatformBitmap LoadBitmap(Stream file);

		protected void OnInit()
		{
			Logger.Info(LOG_TAG, "Initializing application objects.");
			Services = new ServiceContainer();
			GraphicsDevice = new GraphicsDevice(GL);
			GraphicsDevice.OnInit();

			Logger.Info(LOG_TAG, "Registering framework objects with services container.");
			Services.Register(Logger);
			Services.Register(FileSystem);
			if (Keyboard != null)
				Services.Register(Keyboard);
			if (Mouse != null)
				Services.Register(Mouse);
			if (TouchScreen != null)
				Services.Register(TouchScreen);
			Services.Register(GraphicsDevice);

			Logger.Info(LOG_TAG, "Game app init.");
			GameApp.OnInit();
		}

		protected void OnShutdown()
		{
			Logger.Info(LOG_TAG, "Game app shutdown.");
			GameApp.OnShutdown();

			Logger.Info(LOG_TAG, "Shutting down application objects.");
			Services.Dispose();
			Services = null;
		}

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
			if (GraphicsDevice != null)
				GraphicsDevice.OnUnload();
		}

		protected void OnLostContext()
		{
			Logger.Info(LOG_TAG, "OnLostContext");
			GameApp.OnLostContext();
			if (GraphicsDevice != null)
				GraphicsDevice.OnLostContext();
		}

		protected void OnNewContext()
		{
			Logger.Info(LOG_TAG, "OnNewContext");
			if (_isFirstContextCreatedYet)
			{
				if (GraphicsDevice != null)
					GraphicsDevice.OnNewContext();
			}
			_isFirstContextCreatedYet = true;
			GameApp.OnNewContext();
		}

		protected void OnRender(float delta)
		{
			GraphicsDevice.OnRender(delta);
			GameApp.OnRender(delta);
		}

		protected void OnResize(ScreenOrientation orientation, Rect size)
		{
			Logger.Info(LOG_TAG, "OnResize");
			GraphicsDevice.OnResize(ref size, orientation);
			GameApp.OnResize(orientation, size);
		}

		protected void OnUpdate(float delta)
		{
			GameApp.OnUpdate(delta);

			if (Framework.Keyboard != null)
				Framework.Keyboard.OnPostUpdate(delta);
			if (Framework.Mouse != null)
				Framework.Mouse.OnPostUpdate(delta);
			if (Framework.TouchScreen != null)
				Framework.TouchScreen.OnPostUpdate(delta);
		}

		#region Disposable

		protected virtual void Release()
		{
			if (_isReleased)
				return;

			Logger.Info(LOG_TAG, "Releasing resources.");
			GraphicsDevice.Dispose();
		}

		public virtual void Dispose()
		{
			Logger.Info(LOG_TAG, "Disposing.");
		}

		#endregion
	}
}
