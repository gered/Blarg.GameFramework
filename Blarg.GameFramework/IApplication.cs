using System;
using PortableGL;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Input;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework
{
	public interface IApplication : IDisposable
	{
		PlatformOS OperatingSystem { get; }
		PlatformType Type { get; }

		ILogger Logger { get; }
		IFileSystem FileSystem { get; }
		IKeyboard Keyboard { get; }
		IMouse Mouse { get; }
		ITouchScreen TouchScreen { get; }
		IPlatformWindow Window { get; }
		GraphicsDevice GraphicsDevice { get; }
		GL20 GL { get; }

		int FPS { get; }
		float FrameTime { get; }
		int RendersPerSecond { get; }
		int UpdatesPerSecond { get; }
		int RenderTime { get; }
		int UpdateTime { get; }
		bool IsRunningSlowly { get; }

		void Run(IGameApp gameApp, IPlatformConfiguration config);
		void Quit();
	}
}

