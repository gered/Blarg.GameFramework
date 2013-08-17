using System;

namespace Blarg.GameFramework
{
	public interface IPlatformWindow
	{
		Rect ClientRectangle { get; }
		int ClientWidth { get; }
		int ClientHeight { get; }
	}
}

