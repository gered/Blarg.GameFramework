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
		public static IApplication Application { get; private set; }

		public static PlatformOS OperatingSystem
		{
			get { return Application.OperatingSystem; }
		}

		public static PlatformType Type
		{
			get { return Application.Type; }
		}

		public static ILogger Logger
		{
			get { return Application.Logger; }
		}

		public static IFileSystem FileSystem
		{
			get { return Application.FileSystem; }
		}

		public static IKeyboard Keyboard
		{
			get { return Application.Keyboard; }
		}

		public static IMouse Mouse
		{
			get { return Application.Mouse; }
		}

		public static ITouchScreen TouchScreen
		{
			get { return Application.TouchScreen; }
		}

		public static GraphicsDevice GraphicsDevice
		{
			get { return Application.GraphicsDevice; }
		}

		public static GL20 GL
		{
			get { return Application.GL; }
		}

		public static void Set(IApplication application)
		{
			if (Application != null)
				throw new InvalidOperationException();

			Application = application;
		}
	}
}

