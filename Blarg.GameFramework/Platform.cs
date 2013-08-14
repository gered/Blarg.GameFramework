using System;

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

	public static partial class Platform
	{
		public static IPlatformServices Services { get; private set; }
	}
}

