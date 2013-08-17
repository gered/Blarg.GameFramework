using System;
using PortableGL;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Input;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework
{
	public enum PlatformOS
	{
		Windows,
		Linux,
		MacOS,
		Android,
		iOS
	}

	public enum PlatformType
	{
		Mobile,
		Desktop
	}

	public static class Platform
	{
		public static PlatformOS OperatingSystem { get; private set; }
		public static PlatformType Type { get; private set; }

		public static ILooper Looper { get; private set; }
		public static ILogger Logger { get; private set; }
		public static IFileSystem FileSystem { get; private set; }
		public static IKeyboard Keyboard { get; private set; }
		public static IMouse Mouse { get; private set; }
		public static ITouchScreen TouchScreen { get; private set; }
		public static GL20 GL { get; private set; }

		public static void Set(ILooper looper)
		{
			if (Looper != null)
				throw new InvalidOperationException();

			Looper = looper;
			OperatingSystem = Looper.OperatingSystem;
			Type = Looper.Type;
			Logger = Looper.Logger;
			FileSystem = Looper.FileSystem;
			Keyboard = Looper.Keyboard;
			Mouse = Looper.Mouse;
			TouchScreen = Looper.TouchScreen;
			GL = Looper.GL;
		}
	}
}

