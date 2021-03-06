using System;

namespace Blarg.GameFramework
{
	public class SDLConfiguration : IPlatformConfiguration
	{
		public string Title;
		public int Width;
		public int Height;
		public bool Fullscreen;
		public bool Resizeable;

		public int glMajorVersion;
		public int glMinorVersion;
		public bool glDoubleBuffer;
		public int glDepthBufferSize;
		public int glRedSize;
		public int glGreenSize;
		public int glBlueSize;
		public int glAlphaSize;

		public int TargetUpdateFrequency;

		public SDLConfiguration()
		{
			Title = "SDL Application";
			Width = 854;
			Height = 480;
			Fullscreen = false;
			Resizeable = false;
			glMajorVersion = 2;
			glMinorVersion = 1;
			glDoubleBuffer = true;
			glDepthBufferSize = 24;
			glRedSize = 8;
			glGreenSize = 8;
			glBlueSize = 8;
			glAlphaSize = 8;
			TargetUpdateFrequency = 25;
		}
	}
}

