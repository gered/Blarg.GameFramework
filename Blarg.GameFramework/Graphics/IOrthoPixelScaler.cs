using System;

namespace Blarg.GameFramework.Graphics
{
	public interface IOrthoPixelScaler
	{
		int Scale { get; }

		int ScaledWidth { get; }
		int ScaledHeight { get; }

		void Calculate(Rect viewport);
	}
}

