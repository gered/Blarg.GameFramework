using System;

namespace Blarg.GameFramework
{
	public class SDLWindow : IPlatformWindow
	{
		public Rect ClientRectangle { get; internal set; }
		public int ClientWidth { get; internal set; }
		public int ClientHeight { get; internal set; }
	}
}

