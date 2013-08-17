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

	public static class Platform
	{
		static IPlatformServices _services;

		public static IPlatformServices Services
		{
			get { return _services; }
			set
			{
				if (_services != null)
					throw new InvalidOperationException();
				else
					_services = value;
			}
		}
	}
}

