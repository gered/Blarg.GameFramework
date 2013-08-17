using System;

namespace Blarg.GameFramework
{
	public interface IPlatformWindow
	{
		Rect ClientRectangle { get; }
		int ClientWidth { get; }
		int ClientHeight { get; }
		int X { get; }
		int Y { get; }
		int Width { get; }
		int Height { get; }
	}
}

