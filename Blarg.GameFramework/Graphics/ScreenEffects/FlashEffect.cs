using System;

namespace Blarg.GameFramework.Graphics.ScreenEffects
{
	public class FlashEffect : ScreenEffect
	{
		public const float DefaultFlashSpeed = 16.0f;
		public const float DefaultMaxIntensity = 1.0f;

		public float FlashInSpeed;
		public float FlashOutSpeed;
		public float MaximumIntensity;
		public Color Color;

		bool _isFlashingIn;
		float _alpha;

		public FlashEffect()
		{
			_isFlashingIn = true;
			FlashInSpeed = DefaultFlashSpeed;
			FlashOutSpeed = DefaultFlashSpeed;
			MaximumIntensity = DefaultMaxIntensity;
			Color = Color.White;
		}

		public override void OnRender(float delta, SpriteBatch spriteBatch)
		{
			int width = Framework.GraphicsDevice.ViewContext.ViewportWidth;
			int height = Framework.GraphicsDevice.ViewContext.ViewportHeight;

			var texture = Framework.GraphicsDevice.GetSolidColorTexture(Color.White);
			var color = Color;
			color.A = _alpha;

			spriteBatch.Render(texture, 0, 0, width, height, ref color);
		}

		public override void OnUpdate(float delta)
		{
			if (_isFlashingIn)
			{
				_alpha += (delta * FlashInSpeed);
				if (_alpha >= MaximumIntensity)
				{
					_alpha = MaximumIntensity;
					_isFlashingIn = false;
				}
			}
			else
			{
				_alpha -= (delta * FlashOutSpeed);
				if (_alpha < 0.0f)
					_alpha = 0.0f;
			}

			if (_alpha == 0.0f && _isFlashingIn == false)
				IsActive = false;
		}
	}
}
