using System;
using System.IO;
using TrueTypeSharp;

namespace Blarg.GameFramework.Graphics
{
	internal static class SpriteFontTrueTypeLoader
	{
		private struct SpriteFontGlyphMetrics
		{
			public uint Index;
			public float Scale;
			public Rect Dimensions;
			public int Ascent;
			public int Descent;
			public int LineGap;
			public int Advance;
			public int LetterWidth;
			public int LetterHeight;
		}

		public static SpriteFont Load(GraphicsDevice graphicsDevice, Stream file, int size, SpriteFont existingFont = null)
		{
			const int FontBitmapWidth = 512;
			const int FontBitmapHeight = 512;
			const int NumGlyphs = SpriteFont.HighGlyphAscii - SpriteFont.LowGlyphAscii;

			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");
			if (file == null)
				throw new ArgumentNullException("file");

			var binaryReader = new BinaryReader(file);
			var fontBytes = binaryReader.ReadBytes((int)file.Length);

			var ttf = new TrueTypeFont(fontBytes, 0);

			// get glyph metrics for the "maximum size" glyph as an indicator of the
			// general size of each glyph in this font. Uppercase 'W' seems to be a 
			// pretty good glyph to represent this "maximum size".
			var maxMetrics = new SpriteFontGlyphMetrics();
			if (!GetGlyphMetrics(ttf, 'W', size, ref maxMetrics))
				throw new Exception("Failed getting initial 'large glyph' metrics.");

			var fontBitmap = new Image(FontBitmapWidth, FontBitmapHeight, ImageFormat.A);
			fontBitmap.Clear();

			CustomTextureAtlas glyphs = null;

			// if an existing font was provided, then we're just rebuilding the bitmap
			// and the font will continue using the texture atlas is was provided on
			// initial creation again
			if (existingFont == null)
				glyphs = new CustomTextureAtlas(fontBitmap.Width, fontBitmap.Height);

			// NOTE TO SELF: a lot of this feels "hackish" and that some weird font isn't going to play nice with this. clean this up at some point!

			// current position to draw the glyph to on the bitmap
			int x = 0;
			int y = 0;

			// total line height for each row of glyphs on the bitmap. this is not really the font height.
			// it is likely slightly larger then that. this adds the font's descent a second time to make
			// vertical room for a few of the glyphs (e.g. '{}' or '()', or even '$') which go off the
			// top/bottom by a slight bit
			int lineHeight = (maxMetrics.Ascent - maxMetrics.Descent) + maxMetrics.LineGap - maxMetrics.Descent;

			var metrics = new SpriteFontGlyphMetrics();
			var position = new Rect();

			// temporary bitmap to hold each rendered glyph (since TrueTypeSharp can't render into
			// our the pixel buffer in the fontBitmap object directly)
			// setting the initial size like this is slightly overkill for what will actually be used
			// but it seems to be big enough for even the largest glyphs (minimal-to-no resizing needed
			// in the render loop below)
			var glyphBitmap = new FontBitmap(lineHeight, lineHeight);

			for (int i = 0; i < NumGlyphs; ++i)
			{
				char c = (char)(i + SpriteFont.LowGlyphAscii);

				if (!GetGlyphMetrics(ttf, c, size, ref metrics))
					throw new Exception(String.Format("Failed getting metrics for glyph \"{0}\".", c));

				// adjust each glyph's rect so that it has it's own space that doesn't
				// collide with any of it's neighbour glyphs (neighbour as seen on the 
				// font bitmap)
				if (metrics.Dimensions.Left < 0)
				{
					// bump the glyph over to the right by the same amount it was over to the left
					metrics.Advance += -metrics.Dimensions.Left;
					metrics.Dimensions.Left = 0;
				}

				// do we need to move to the next row?
				if ((x + metrics.Advance + metrics.Dimensions.Left) >= fontBitmap.Width)
				{
					// yes
					x = 0;
					y += lineHeight;
					System.Diagnostics.Debug.Assert((y + lineHeight) < fontBitmap.Height);
				}

				// the destination bitmap pixel coordinates of this glyph. these are the
				// pixel coordinates that will be stored in the font's texture atlas
				// which will be used by the texture atlas to build texture coords
				position.Left = x;
				position.Top = y;
				position.Right = x + metrics.Advance;
				position.Bottom = y + lineHeight;

				// top-left coords and dimensions to have stb_truetype draw the glyph at in the font bitmap
				int drawX = position.Left + metrics.Dimensions.Left;
				int drawY = (position.Bottom + metrics.Descent) + metrics.Descent - metrics.Dimensions.Bottom + maxMetrics.LineGap;
				int drawWidth = position.Width;
				int drawHeight = position.Height;

				// resize the glyph bitmap only when necessary
				if (drawWidth > glyphBitmap.Width || drawHeight > glyphBitmap.Height)
					glyphBitmap = new FontBitmap(drawWidth, drawHeight);

				// render glyph
				ttf.MakeGlyphBitmap(metrics.Index, metrics.Scale, metrics.Scale, glyphBitmap);

				// copy from the temp bitmap to our full font bitmap (unfortunately TrueTypeSharp doesn't give us
				// a way to do this directly out of the box)
				fontBitmap.Copy(glyphBitmap.Buffer, glyphBitmap.Width, glyphBitmap.Height, 8, 0, 0, drawWidth, drawHeight, drawX, drawY);

				if (glyphs != null)
				{
					// add the glyph position to the texture atlas (which will calc the texture coords for us)
					int newIndex = glyphs.Add(position);
					System.Diagnostics.Debug.Assert(newIndex == ((int)c - SpriteFont.LowGlyphAscii));
				}

				// move to the next glyph's position in the bitmap
				x += maxMetrics.Advance;
			}

			SpriteFont font;
			if (existingFont == null)
			{
				var texture = new Texture(graphicsDevice, fontBitmap, TextureParameters.Pixelated);
				font = new SpriteFont(graphicsDevice, size, texture, glyphs);
			}
			else
			{
				font = existingFont;

				// texture will have been reallocated already by SpriteFont, just need to provide the bitmap
				// and the Reload method will handle updating the texture itself
				font.Reload(fontBitmap);
			}

			return font;
		}

		private static bool GetGlyphMetrics(TrueTypeFont font, char glyph, int size, ref SpriteFontGlyphMetrics metrics)
		{
			uint index = font.FindGlyphIndex(glyph);
			if (index == 0)
				return false;

			metrics.Scale = font.GetScaleForPixelHeight((float)size);
			metrics.LetterHeight = size;
			metrics.Index = index;

			font.GetGlyphBox(index, out metrics.Dimensions.Left, out metrics.Dimensions.Top, out metrics.Dimensions.Right, out metrics.Dimensions.Bottom);
			font.GetFontVMetrics(out metrics.Ascent, out metrics.Descent, out metrics.LineGap);
			int leftSideBearing;
			font.GetGlyphHMetrics(index, out metrics.Advance, out leftSideBearing);

			// adjust all the metrics we got by the font size scale value
			// (I guess this puts them from whatever units they were in to pixel units)
			metrics.Dimensions.Left = (int)Math.Ceiling((double)metrics.Dimensions.Left * metrics.Scale);
			metrics.Dimensions.Top = (int)Math.Ceiling((double)metrics.Dimensions.Top * metrics.Scale);
			metrics.Dimensions.Right = (int)Math.Ceiling((double)metrics.Dimensions.Right * metrics.Scale);
			metrics.Dimensions.Bottom = (int)Math.Ceiling((double)metrics.Dimensions.Bottom * metrics.Scale);
			metrics.Ascent = (int)Math.Ceiling((double)metrics.Ascent * metrics.Scale);
			metrics.Descent = (int)Math.Ceiling((double)metrics.Descent * metrics.Scale);
			metrics.LineGap = (int)Math.Ceiling((double)metrics.LineGap * metrics.Scale);
			metrics.Advance = (int)Math.Ceiling((double)metrics.Advance * metrics.Scale);
			metrics.LetterWidth = metrics.Dimensions.Right - metrics.Dimensions.Left;

			// seen some pixel/bitmap fonts that have the total ascent/descent calculated height
			// greater then the pixel height. this just figures out this difference, if present,
			// and sets an appropriate line gap equal to it (in these cases, linegap was 0)
			int calculatedHeight = metrics.Ascent - metrics.Descent;
			int heightDifference = Math.Abs(calculatedHeight - metrics.LetterHeight);
			if (heightDifference != metrics.LineGap && metrics.LineGap == 0)
				metrics.LineGap = heightDifference;

			return true;
		}
	}
}
