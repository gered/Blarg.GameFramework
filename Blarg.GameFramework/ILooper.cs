using System;

namespace Blarg.GameFramework
{
	public interface ILooper : IDisposable
	{
		int FPS { get; }
		float FrameTime { get; }
		int RendersPerSecond { get; }
		int UpdatesPerSecond { get; }
		int RenderTime { get; }
		int UpdateTime { get; }

		void Run(IGameApp gameApp, IPlatformConfiguration config);
	}
}

