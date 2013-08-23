using System;

namespace Blarg.GameFramework.Graphics.ScreenEffects
{
	public class FadeEffect : ScreenEffect
	{
		public const float DefaultFadeSpeed = 3.0f;

		float _fadeSpeed;
		bool _isFadingOut;
		float _alpha;
		Color _color;
		float _fadeToAlpha;

		public bool IsDoneFading { get; private set; }

		public FadeEffect()
		{
		}

		public void FadeOut(float toAlpha, Color color, float speed = DefaultFadeSpeed)
		{
			if (toAlpha < 0.0f || toAlpha > 1.0f)
				throw new ArgumentOutOfRangeException("toAlpha");

			_color = color;
			_fadeSpeed = speed;
			_isFadingOut = true;
			_alpha = Color.AlphaTransparent;
			_fadeToAlpha = toAlpha;
		}

		public void FadeIn(float toAlpha, Color color, float speed = DefaultFadeSpeed)
		{
			if (toAlpha < 0.0f || toAlpha > 1.0f)
				throw new ArgumentOutOfRangeException("toAlpha");

			_color = color;
			_fadeSpeed = speed;
			_isFadingOut = false;
			_alpha = Color.AlphaOpaque;
			_fadeToAlpha = toAlpha;
		}

		public override void OnRender(float delta)
		{
			int width = Framework.GraphicsDevice.ViewContext.ViewportWidth;
			int height = Framework.GraphicsDevice.ViewContext.ViewportHeight;

			var texture = Framework.GraphicsDevice.GetSolidColorTexture(Color.White);
			_color.A = _alpha;

			//Platform.SpriteBatch.Render(texture, 0, 0, width, height, ref _color);
		}

		public override void OnUpdate(float delta)
		{
			if (IsDoneFading)
				return;

			if (_isFadingOut)
			{
				_alpha += (delta + _fadeSpeed);
				if (_alpha >= _fadeToAlpha)
				{
					_alpha = _fadeToAlpha;
					IsDoneFading = true;
				}
			}
			else
			{
				_alpha -= (delta + _fadeSpeed);
				if (_alpha < _fadeToAlpha)
				{
					_alpha = _fadeToAlpha;
					IsDoneFading = true;
				}
			}
		}
	}
}
