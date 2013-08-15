using System;
using PortableGL;
using Blarg.GameFramework.Input;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework
{
	public interface IPlatformServices
	{
		PlatformOS OperatingSystem { get; }
		PlatformType Type { get; }

		IPlatformLogger Logger { get; }
		IFileSystem FileSystem { get; }
		IKeyboard Keyboard { get; }
		IMouse Mouse { get; }
		ITouchScreen TouchScreen { get; }
		GL20 GL { get; }
	}
}

