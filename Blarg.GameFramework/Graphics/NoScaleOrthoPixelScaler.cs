using System;

namespace Blarg.GameFramework.Graphics
{
	public class NoScaleOrthoPixelScaler : IOrthoPixelScaler
	{
		Rect _viewport;

		public void Calculate(Rect viewport)
		{
			// no scaling whatsoever!
			_viewport = viewport;
		}

		public int Scale
		{
			get { return 1; }
		}

		public Rect ScaledViewport
		{
			get { return _viewport; }
		}

		public int ScaledWidth
		{
			get { return _viewport.Width; }
		}

		public int ScaledHeight
		{
			get { return _viewport.Height; }
		}
	}
}

