using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework.Graphics
{
	public class SDLBitmap : IPlatformBitmap
	{
		byte[] _pixels;
		int _width;
		int _height;
		Graphics.ImageFormat _format;

		public byte[] Pixels
		{
			get { return _pixels; }
		}

		public int Width
		{
			get { return _width; }
		}

		public int Height
		{
			get { return _height; }
		}

		public Graphics.ImageFormat Format
		{
			get { return _format; }
		}

		public SDLBitmap(Stream file)
		{
			_pixels = GetBitmap(file, out _width, out _height, out _format);
		}

		private byte[] GetBitmap(Stream file, out int width, out int height, out Graphics.ImageFormat format)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			using (Bitmap bitmap = (Bitmap)Bitmap.FromStream(file))
			{
				int bpp = 0;
				if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
				{
					bpp = 32;
					format = Graphics.ImageFormat.RGBA;
				}
				else if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
				{
					bpp = 24;
					format = Graphics.ImageFormat.RGB;
				}
				else if (bitmap.PixelFormat == PixelFormat.Alpha)
				{
					bpp = 8;
					format = Graphics.ImageFormat.A;
				}
				else
					throw new InvalidOperationException(String.Format("Unsupported bitmap pixel format: {0}", bitmap.PixelFormat.ToString()));

				width = bitmap.Width;
				height = bitmap.Height;

				byte[] pixels;

				var lockRegion = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				var bitmapData = bitmap.LockBits(lockRegion, ImageLockMode.ReadOnly, bitmap.PixelFormat);
				try
				{
					int bitmapSizeInBytes = bitmap.Width * bitmap.Height * (bpp / 8);
					int expectedPitch = bitmap.Width * (bpp / 8);
					int actualPitch = bitmapData.Stride;

					if (expectedPitch != actualPitch)
						throw new InvalidOperationException("Expected bitmap pitch mismatch. Uncaught unsupported bitmap format?");

					pixels = new byte[bitmapData.Stride * bitmap.Height];
					Marshal.Copy(bitmapData.Scan0, pixels, 0, bitmapSizeInBytes);
				}
				finally
				{
					bitmap.UnlockBits(bitmapData);
				}

				// TODO: are there any cases where this should *not* be run?
				SwapRgbOrdering(pixels, format);

				return pixels;
			}
		}

		private void SwapRgbOrdering(byte[] pixels, Graphics.ImageFormat pixelFormat)
		{
			if (pixelFormat == Graphics.ImageFormat.A)
				return;

			// assumption is that this will only ever be 3 or 4 (RGB, or RGBA)
			int pixelSizeBytes;
			if (pixelFormat == Graphics.ImageFormat.RGB)
				pixelSizeBytes = 3;
			else
				pixelSizeBytes = 4;

			if (pixels.Length % pixelSizeBytes != 0)
				throw new InvalidDataException("Pixel data format mismatch.");

			// swap R and B values for each pixel
			for (int i = 0; i < pixels.Length; i += pixelSizeBytes)
			{
				byte r = pixels[i];
				byte b = pixels[i + 2];
				pixels[i] = b;
				pixels[i + 2] = r;
			}
		}

	}
}

