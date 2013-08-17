using System;

namespace Blarg.GameFramework
{
	public interface ILooper : IDisposable
	{
		void Run(IGameApp gameApp);
	}
}

