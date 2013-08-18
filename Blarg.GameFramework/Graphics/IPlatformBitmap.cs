using System;

namespace Blarg.GameFramework.Graphics
{
	public interface IPlatformBitmap
	{
		byte[] Pixels { get; }
		int Width { get; }
		int Height { get; }
		ImageFormat Format { get; }
	}
}
