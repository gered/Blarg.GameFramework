using System;

namespace Blarg.GameFramework.Graphics
{
	public class RetroOrthoPixelScaler : IOrthoPixelScaler
	{
		int _scale = 0;
		Rect _scaledViewport;

		public void Calculate(Rect viewport)
		{
			int viewportWidth = viewport.Width;
			int viewportHeight = viewport.Height;

			// TODO: these might need tweaking, this is fairly arbitrary
			if (viewportWidth < 640 || viewportHeight < 480)
				_scale = 1;
			else if (viewportWidth < 960 || viewportHeight < 720)
				_scale = 2;
			else if (viewportWidth < 1280 || viewportHeight < 960)
				_scale = 3;
			else if (viewportWidth < 1920 || viewportHeight < 1080)
				_scale = 4;
			else
				_scale = 5;

			// TODO: desktop "retina" / 4K display sizes? 1440p?

			_scaledViewport.Left = viewport.Left / _scale;
			_scaledViewport.Top = viewport.Top / _scale;
			_scaledViewport.Right = viewport.Right / _scale;
			_scaledViewport.Bottom = viewport.Bottom / _scale;
		}

		public int Scale
		{
			get { return _scale; }
		}

		public Rect ScaledViewport
		{
			get { return _scaledViewport; }
		}

		public int ScaledWidth
		{
			get { return _scaledViewport.Width; }
		}

		public int ScaledHeight
		{
			get { return _scaledViewport.Height; }
		}
	}
}

