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

		public static IApplication Application { get; private set; }
		public static ILogger Logger { get; private set; }
		public static IFileSystem FileSystem { get; private set; }
		public static IKeyboard Keyboard { get; private set; }
		public static IMouse Mouse { get; private set; }
		public static ITouchScreen TouchScreen { get; private set; }
		public static GL20 GL { get; private set; }

		public static void Set(IApplication application)
		{
			if (Application != null)
				throw new InvalidOperationException();

			Application = application;
			OperatingSystem = Application.OperatingSystem;
			Type = Application.Type;
			Logger = Application.Logger;
			FileSystem = Application.FileSystem;
			Keyboard = Application.Keyboard;
			Mouse = Application.Mouse;
			TouchScreen = Application.TouchScreen;
			GL = Application.GL;
		}
	}
}

