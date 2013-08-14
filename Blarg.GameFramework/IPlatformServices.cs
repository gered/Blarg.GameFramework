using System;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework
{
	public interface IPlatformServices
	{
		PlatformOS OperatingSystem { get; }
		PlatformType Type { get; }

		IPlatformLogger Logger { get; }
		IFileSystem FileSystem { get; }
	}
}

