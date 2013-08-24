using System;
using Blarg.GameFramework.Content;
using Blarg.GameFramework.Graphics;

namespace Blarg.GameFramework.UI
{
	public class GwenSpriteBatchRenderer : Gwen.Renderer.Base
	{
		const string LOG_TAG = "GWEN";

		ContentManager _contentManager;
		GraphicsDevice _graphicsDevice;
		SpriteBatch _spriteBatch;

		public GwenSpriteBatchRenderer(ContentManager contentManager, GraphicsDevice graphicsDevice)
		{
			if (contentManager == null)
				throw new ArgumentNullException("contentManager");
			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");

			_contentManager = contentManager;
			_graphicsDevice = graphicsDevice;
			Alpha = Color.AlphaOpaque;
		}

		#region Begin / End

		public void PreRender(SpriteBatch spriteBatch)
		{
			if (spriteBatch == null)
				throw new ArgumentNullException("spriteBatch");

			_spriteBatch = spriteBatch;
		}

		public void PostRender()
		{
			_spriteBatch = null;
		}

		public override void Begin()
		{
			if (_spriteBatch == null)
				throw new InvalidOperationException();
			base.Begin();
		}

		public override void End()
		{
			base.End();
		}

		#endregion

		#region Rendering States / Properties

		public float Alpha { get; set; }

		public override void StartClip()
		{
			var rect = ClipRegion;

			int left = (int)((float)rect.X * Scale);
			int top = (int)((float)rect.Y * Scale);
			int right = (int)((float)(rect.X + rect.Width) * Scale);
			int bottom = (int)((float)(rect.Y + rect.Height) * Scale);

			_spriteBatch.BeginClipping(left, top, right, bottom);
		}

		public override void EndClip()
		{
			_spriteBatch.EndClipping();
		}

		private void AdjustColorForAlpha(ref Color color)
		{
			color.A *= Alpha;
		}

		#endregion

		#region General Rendering Operations

		public override void DrawFilledRect(Gwen.Rectangle rect)
		{
			Translate(rect);

			var renderColor = new Color((int)DrawColor.R, (int)DrawColor.G, (int)DrawColor.B, (int)DrawColor.A);
			AdjustColorForAlpha(ref renderColor);

			// TODO: this solid color texture should probably be grabbed using a color
			//       that has A = 1.0 always otherwise any kind of fading, etc. will
			//       result in many different solid color's ending up in the solid color 
			//       texture cache (due to all the different A values!)
			var colorTexture = _graphicsDevice.GetSolidColorTexture(ref renderColor);

			_spriteBatch.Render(colorTexture, rect.X, rect.Y, rect.Width, rect.Height);
		}

		#endregion

		#region Textured Rendering

		public override void LoadTexture(Gwen.Texture t)
		{
			Framework.Logger.Info(LOG_TAG, "SpriteBatchRenderer loading texture \"{0}\".", t.Name);
			var texture = _contentManager.Get<Texture>(t.Name);

			t.RendererData = texture;
			t.Width = texture.Width;
			t.Height = texture.Height;
		}

		public override void FreeTexture(Gwen.Texture t)
		{
			// right now we don't care, ContentManager.RemoveAlLContent() can and will take care of this
			// (Gwen doesn't really load too many resources that I think we really need to care about this)
		}

		public override void DrawTexturedRect(Gwen.Texture t, Gwen.Rectangle targetRect, float u1, float v1, float u2, float v2)
		{
			if (t.RendererData == null)
				throw new InvalidOperationException();

			Translate(targetRect);
			var texture = (Texture)t.RendererData;

			var renderColor = Color.White;
			AdjustColorForAlpha(ref renderColor);

			_spriteBatch.Render(texture, targetRect.X, targetRect.Y, targetRect.Width, targetRect.Height, u1, v1, u2, v2, ref renderColor);
		}

		public override Gwen.Color PixelColor(Gwen.Texture texture, uint x, uint y, Gwen.Color defaultColor)
		{
			if (texture.RendererData == null)
				return defaultColor;

			// This method is really only used by Gwen to figure out various "system" colors
			// at initialization time. Pixel colors are read from the renderer skin texture
			// pretty sure no other textures are ever used with this method. Unless some
			// future custom control eventually is needed to read pixel colors...
			//
			// We load the image using ContentManager, but it won't be released... at least
			// not until the next ContentManager.RemoveAllContent() call (not done by this class)
			// I feel that this is fine given the above about only one texture (and therefore
			// only one image) being ever read by this method.

			var image = _contentManager.Get<Image>(texture.Name);
			var pixelColor = image.GetColorAt((int)x, (int)y);

			return Gwen.Color.FromArgb(pixelColor.IntA, pixelColor.IntR, pixelColor.IntG, pixelColor.IntB);
		}

		#endregion

		#region Font Rendering

		public override bool LoadFont(Gwen.Font font)
		{
			Framework.Logger.Info(LOG_TAG, "SpriteBatchRenderer loading font \"{0}\".", font.FaceName);
			int size = font.Size;
			var spriteFont = _contentManager.Get<SpriteFont>(font.FaceName, size);

			font.RendererData = spriteFont;
			return true;
		}

		public override void FreeFont(Gwen.Font font)
		{
			// right now we don't care, ContentManager.RemoveAlLContent() can and will take care of this
			// (Gwen doesn't really load too many resources that I think we really need to care about this)
		}

		public override void RenderText(Gwen.Font font, Gwen.Point position, string text)
		{
			Translate(position);
			var spriteFont = (SpriteFont)font.RendererData;

			var renderColor = new Color((int)DrawColor.R, (int)DrawColor.G, (int)DrawColor.B, (int)DrawColor.A);
			AdjustColorForAlpha(ref renderColor);

			_spriteBatch.Render(spriteFont, position.X, position.Y, ref renderColor, Scale, text);
		}

		public override Gwen.Point MeasureText(Gwen.Font font, string text)
		{
			// HACK: is this supposed to work this way? seems that MeasureText 
			//       can (and will) get called from Gwen's classes before a call
			//       to LoadFont is made...
			if (font.RendererData == null)
				LoadFont(font);

			if (font.RendererData == null)
				throw new Exception("Failed to load font.");

			var spriteFont = (SpriteFont)font.RendererData;

			int width;
			int height;
			spriteFont.MeasureString(out width, out height, text);

			return new Gwen.Point(width, height);
		}

		#endregion
	}
}
