using System;
using System.IO;

namespace Blarg.GameFramework.Graphics
{
	public partial class Image
	{
		private byte[] _pixels;

		public int Pitch { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int BitsPerPixel { get; private set; }
		public ImageFormat PixelFormat { get; private set; }

		public byte[] Pixels
		{
			get { return _pixels; }
		}

		public int SizeInBytes
		{
			get { return (Width * Height) * (BitsPerPixel / 8); }
		}

		public Image(int width, int height, ImageFormat pixelFormat)
		{
			CreateBaseImage(width, height, pixelFormat);
		}

		public Image(Image source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			Create(source, 0, 0, source.Width, source.Height);
		}

		public Image(Image source, int copyX, int copyY, int copyWidth, int copyHeight)
		{
			Create(source, copyX, copyY, copyWidth, copyHeight);
		}

		public Image(Stream file)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			var bitmap = Platform.Application.LoadBitmap(file);

			CreateBaseImage(bitmap.Width, bitmap.Height, bitmap.Format);
			Buffer.BlockCopy(bitmap.Pixels, 0, _pixels, 0, bitmap.Pixels.Length);
		}

		private void CreateBaseImage(int width, int height, ImageFormat pixelFormat)
		{
			if (width < 1)
				throw new ArgumentOutOfRangeException("width");
			if (height < 1)
				throw new ArgumentOutOfRangeException("height");

			int bpp = 0;
			if (pixelFormat == ImageFormat.RGB)
				bpp = 24;
			else if (pixelFormat == ImageFormat.RGBA)
				bpp = 32;
			else if (pixelFormat == ImageFormat.A)
				bpp = 8;

			if (bpp == 0)
				throw new ArgumentException("pixelFormat");

			int pixelsLength = (width * height) * (bpp / 8);
			_pixels = new byte[pixelsLength];

			Width = width;
			Height = height;
			BitsPerPixel = bpp;
			PixelFormat = pixelFormat;
			Pitch = Width * (BitsPerPixel / 8);
		}

		private void Create(Image source, int copyX, int copyY, int copyWidth, int copyHeight)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (copyX >= source.Width)
				throw new InvalidOperationException();
			if (copyWidth > source.Width)
				throw new InvalidOperationException();
			if (copyY >= source.Height)
				throw new InvalidOperationException();
			if (copyHeight > source.Height)
				throw new InvalidOperationException();

			CreateBaseImage(copyWidth, copyHeight, source.PixelFormat);
			Copy(source, copyX, copyY, copyWidth, copyHeight, 0, 0);
		}

		public int GetOffsetFor(int x, int y)
		{
			return (x + (y * Width)) * (BitsPerPixel / 8);
		}

		public Color GetColorAt(int x, int y)
		{
			Color result;
			GetColorAt(x, y, out result);
			return result;
		}

		public void GetColorAt(int x, int y, out Color result)
		{
			if (PixelFormat == ImageFormat.RGB)
			{
				int offset = GetOffsetFor(x, y);
				result = new Color(_pixels[offset], _pixels[offset + 1], _pixels[offset + 2]);
			}
			else if (PixelFormat == ImageFormat.RGBA)
			{
				int offset = GetOffsetFor(x, y);
				result = new Color(_pixels[offset], _pixels[offset + 1], _pixels[offset + 2], _pixels[offset + 3]);
			}
			else
				throw new InvalidOperationException();
		}

		public void SetColorAt(int x, int y, ref Color color)
		{
			if (PixelFormat == ImageFormat.RGB)
			{
				int offset = GetOffsetFor(x, y);
				_pixels[offset] = (byte)color.IntR;
				_pixels[offset + 1] = (byte)color.IntG;
				_pixels[offset + 2] = (byte)color.IntB;
			}
			else if (PixelFormat == ImageFormat.RGBA)
			{
				int offset = GetOffsetFor(x, y);
				_pixels[offset] = (byte)color.IntR;
				_pixels[offset + 1] = (byte)color.IntG;
				_pixels[offset + 2] = (byte)color.IntB;
				_pixels[offset + 3] = (byte)color.IntA;
			}
			else
				throw new InvalidOperationException();
		}

		public void SetColorAt(int x, int y, int color)
		{
			Color c = Color.FromInt(color);
			SetColorAt(x, y, ref c);
		}

		public void Copy(Image source, int destX, int destY)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			Copy(source, 0, 0, source.Width, source.Height, destX, destY);
		}

		public void Copy(Image source, int copyX, int copyY, int copyWidth, int copyHeight, int destX, int destY)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			Copy(source.Pixels, source.Width, source.Height, source.BitsPerPixel, copyX, copyY, copyWidth, copyHeight, destX, destY);
		}

		public void Copy(byte[] source, int sourceWidth, int sourceHeight, int sourceBpp, int destX, int destY)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			Copy(source, sourceWidth, sourceHeight, sourceBpp, 0, 0, sourceWidth, sourceHeight, destX, destY);
		}

		public void Copy(byte[] source, int sourceWidth, int sourceHeight, int sourceBpp, int copyX, int copyY, int copyWidth, int copyHeight, int destX, int destY)
		{
			if (sourceBpp != BitsPerPixel)
				throw new InvalidOperationException();
			if (copyX >= sourceWidth)
				throw new InvalidOperationException();
			if (copyWidth > sourceWidth)
				throw new InvalidOperationException();
			if (copyY >= sourceHeight)
				throw new InvalidOperationException();
			if (copyHeight > sourceHeight)
				throw new InvalidOperationException();
			if (destX >= Width)
				throw new InvalidOperationException();
			if (destY >= Height)
				throw new InvalidOperationException();
			if (copyWidth > Width)
				throw new InvalidOperationException();
			if (copyHeight > Height)
				throw new InvalidOperationException();
			if ((destX + copyWidth) > Width)
				throw new InvalidOperationException();
			if ((destY + copyHeight) > Height)
				throw new InvalidOperationException();

			int expectedSourceSize = sourceWidth * sourceHeight * (sourceBpp / 8);
			if (expectedSourceSize != source.Length)
				throw new InvalidOperationException();

			int lineCopySizeInBytes = copyWidth * (sourceBpp / 8);
			int numLinesToCopy = copyHeight;

			int sourceOffset = (copyX + (copyY * sourceWidth)) * (sourceBpp / 8);
			int destOffset = GetOffsetFor(destX, destY);

			int sourcePitch = sourceWidth * (sourceBpp / 8);

			for (int i = 0; i < numLinesToCopy; ++i)
			{
				Buffer.BlockCopy(source, sourceOffset, _pixels, destOffset, lineCopySizeInBytes);
				sourceOffset += sourcePitch;
				destOffset += Pitch;
			}
		}

		public void Clear()
		{
			Array.Clear(_pixels, 0, _pixels.Length);
		}

		public void Clear(ref Color color)
		{
			// guessing that filling the pixels array this way using pointers is
			// maybe a bit faster due to not indexing an array each iteration
			// probably a negligible difference if any ... ?

			byte r = (byte)color.IntR;
			byte g = (byte)color.IntG;
			byte b = (byte)color.IntB;
			byte a = (byte)color.IntA;

			if (PixelFormat == ImageFormat.RGB)
			{
				unsafe
				{
					fixed (byte *pixel = _pixels)
					{
						int length = _pixels.Length;
						for (int i = 0; i < length; i += 3)
						{
							byte* dest = pixel + i;
							*(dest) = r;
							*(dest + 1) = g;
							*(dest + 2) = b;
						}
					}
				}
			}
			else if (PixelFormat == ImageFormat.RGBA)
			{
				unsafe
				{
					fixed (byte *pixel = _pixels)
					{
						int length = _pixels.Length;
						for (int i = 0; i < length; i += 4)
						{
							byte* dest = pixel + i;
							*(dest) = r;
							*(dest + 1) = g;
							*(dest + 2) = b;
							*(dest + 3) = a;
						}
					}
				}
			}
			else
				throw new InvalidOperationException();
		}

		public void Clear(int color)
		{
			Color c = Color.FromInt(color);
			Clear(ref c);
		}

		public void Clear(byte alpha)
		{
			if (PixelFormat != ImageFormat.A)
				throw new InvalidOperationException();

			// guessing that filling the pixels array this way using pointers is
			// maybe a bit faster due to not indexing an array each iteration
			// probably a negligible difference if any ... ?

			unsafe
			{
				fixed (byte *pixel = _pixels)
				{
					int length = _pixels.Length;
					for (int i = 0; i < length; ++i)
					{
						byte* dest = pixel + i;
						*(dest) = alpha;
					}
				}
			}
		}

		public void FlipVertically()
		{
			// TODO: probably could be a bit nicer on memory usage if we only use a temp
			// buffer equal to one line of pixels and swap top/bottom lines one at a time
			// just in the existing pixel array

			byte[] flippedPixels = new byte[SizeInBytes];

			int sourceOffset = SizeInBytes - Pitch;   // first pixel of the last line
			int destOffset = 0;                       // first pixel of the first line

			for (int i = 0; i < Height; ++i)
			{
				Buffer.BlockCopy(_pixels, sourceOffset, flippedPixels, destOffset, Pitch);
				sourceOffset -= Pitch;         // go up one line
				destOffset += Pitch;           // go down one line
			}

			_pixels = flippedPixels;
		}
	}
}
