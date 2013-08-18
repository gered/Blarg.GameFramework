using System;
using System.Text;

namespace Blarg.GameFramework.Graphics
{
	public class SpriteFont : GraphicsContextResource
	{
		private static StringBuilder _buffer = new StringBuilder(8192);

		public const int LowGlyphAscii = 32;
		public const int HighGlyphAscii = 127;

		private TextureAtlas _glyphs;

		public int Size { get; private set; }
		public int LetterHeight { get; private set; }

		public Texture Texture { get; private set; }

		public bool IsInvalidated { get; private set; }

		#region Instantiation, (Re-)loading, new/lost context

		// this class is designed to be instantiated and have it's lifecycle maintained
		// by a sprite font content loader *only*
		// therefore, the following methods are all internal

		internal SpriteFont(GraphicsDevice graphicsDevice, int fontSize, Texture texture, TextureAtlas glyphs)
			: base(graphicsDevice)
		{
			if (fontSize < 1)
				throw new ArgumentOutOfRangeException("fontSize");
			if (texture == null)
				throw new ArgumentNullException("texture");
			if (texture.IsInvalidated)
				throw new ArgumentException("Texture is invalidated.");
			if (glyphs == null)
				throw new ArgumentNullException("glyphs");
			if (glyphs.NumTiles == 0)
				throw new ArgumentException("TextureAtlas is empty.");

			Size = fontSize;
			Texture = texture;
			_glyphs = glyphs;

			Rect charSize = new Rect();
			_glyphs.GetTileDimensions(GetIndexOfChar(' '), out charSize);
			LetterHeight = charSize.Height;

			IsInvalidated = false;
		}

		internal void Reload(Image fontBitmap)
		{
			if (!IsInvalidated)
				throw new InvalidOperationException();
			if (fontBitmap == null)
				throw new ArgumentNullException("fontBitmap");
			if (fontBitmap.Width != Texture.Width || fontBitmap.Height != Texture.Height)
				throw new ArgumentException("Font bitmap dimensions do not match texture dimensions.");

			Texture.Update(fontBitmap);

			IsInvalidated = false;
		}

		public override void OnNewContext()
		{
		}

		public override void OnLostContext()
		{
			IsInvalidated = true;
		}

		#endregion

		private int GetIndexOfChar(char c)
		{
			if ((int)c < LowGlyphAscii || (int)c > HighGlyphAscii)
				return HighGlyphAscii - LowGlyphAscii;
			else
				return (int)c - LowGlyphAscii;
		}

		public void GetCharDimensions(char c, out Rect dimensions)
		{
			int index = GetIndexOfChar(c);
			_glyphs.GetTileDimensions(index, out dimensions);
		}

		public void GetCharTexCoords(char c, out RectF texCoords)
		{
			int index = GetIndexOfChar(c);
			_glyphs.GetTileTexCoords(index, out texCoords);
		}

		public void MeasureString(out int width, out int height, string format, params object[] args)
		{
			_buffer.Clear();
			_buffer.AppendFormat(format, args);

			int textLength = _buffer.Length;

			int currentMaxWidth = 0;
			int left = 0;
			int numLines = 1;

			for (int i = 0; i < textLength; ++i)
			{
				char c = _buffer[i];
				if (c == '\n')
				{
					// new line
					left = 0;
					++numLines;
				}
				else
				{
					Rect charSize = new Rect();
					GetCharDimensions(c, out charSize);
					left += charSize.Width;
				}

				currentMaxWidth = Math.Max(left, currentMaxWidth);
			}

			width = currentMaxWidth;
			height = numLines * LetterHeight;
		}

		public void MeasureString(out int width, out int height, float scale, string format, params object[] args)
		{
			_buffer.Clear();
			_buffer.AppendFormat(format, args);

			int textLength = _buffer.Length;

			float scaledLetterHeight = (float)LetterHeight * scale;

			float currentMaxWidth = 0.0f;
			float left = 0.0f;
			int numLines = 1;

			for (int i = 0; i < textLength; ++i)
			{
				char c = _buffer[i];
				if (c == '\n')
				{
					// new line
					left = 0.0f;
					++numLines;
				}
				else
				{
					Rect charSize = new Rect();
					GetCharDimensions(c, out charSize);
					left += (float)charSize.Width * scale;
				}

				currentMaxWidth = Math.Max(left, currentMaxWidth);
			}

			width = (int)Math.Ceiling(currentMaxWidth);
			height = (int)(numLines * scaledLetterHeight);
		}
	}
}
