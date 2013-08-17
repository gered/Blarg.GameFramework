using System;
using PortableGL;
using Blarg.GameFramework.Input;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework
{
	public class SDLPlatformServices : IPlatformServices
	{
		public SDLPlatformServices()
		{
			if (CurrentOS.IsWindows)
				OperatingSystem = PlatformOS.Windows;
			else if (CurrentOS.IsLinux)
				OperatingSystem = PlatformOS.Linux;
			else if (CurrentOS.IsMac)
				OperatingSystem = PlatformOS.MacOS;
			else
				throw new Exception("Unable to determine OS.");

			Logger = new SDLLogger();
		}

		public PlatformOS OperatingSystem { get; private set; }
		public PlatformType Type
		{
			get { return PlatformType.Desktop; }
		}

		public IPlatformLogger Logger { get; internal set; }
		public IFileSystem FileSystem { get; internal set; }
		public IKeyboard Keyboard { get; internal set; }
		public IMouse Mouse { get; internal set; }
		public ITouchScreen TouchScreen { get; internal set; }
		public GL20 GL { get; internal set; }
	}
}

