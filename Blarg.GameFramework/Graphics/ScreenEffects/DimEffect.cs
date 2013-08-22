using System;

namespace Blarg.GameFramework.Graphics.ScreenEffects
{
	public class DimEffect : ScreenEffect
	{
		public static readonly Color DefaultDimColor = Color.Black;
		public const float DefaultDimAlpha = 0.5f;

		public Color Color;
		public float Alpha;

		public DimEffect()
		{
			Color = DefaultDimColor;
			Alpha = DefaultDimAlpha;
		}

		public override void OnRender(float delta)
		{
			int width = Platform.GraphicsDevice.ViewContext.ViewportWidth;
			int height = Platform.GraphicsDevice.ViewContext.ViewportHeight;

			var texture = Platform.GraphicsDevice.GetSolidColorTexture(Color.White);
			var color = Color;
			color.A = Alpha;

			//Platform.SpriteBatch.Render(texture, 0, 0, width, height, ref color);
		}
	}
}
