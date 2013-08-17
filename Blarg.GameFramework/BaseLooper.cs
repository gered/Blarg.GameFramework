using System;
using PortableGL;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Input;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework
{
	public abstract class BaseLooper : ILooper
	{
		protected IGameApp GameApp { get; set; }

		public abstract PlatformOS OperatingSystem { get; }
		public abstract PlatformType Type { get; }

		public abstract IPlatformLogger Logger { get; }
		public abstract IFileSystem FileSystem { get; }
		public abstract IKeyboard Keyboard { get; }
		public abstract IMouse Mouse { get; }
		public abstract ITouchScreen TouchScreen { get; }
		public abstract GL20 GL { get; }

		public int FPS { get; protected set; }
		public float FrameTime { get; protected set; }
		public int RendersPerSecond { get; protected set; }
		public int UpdatesPerSecond { get; protected set; }
		public int RenderTime { get; protected set; }
		public int UpdateTime { get; protected set; }
		public bool IsRunningSlowly { get; protected set; }

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

