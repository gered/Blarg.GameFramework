using System;

namespace Blarg.GameFramework.Graphics.ScreenEffects
{
	internal class EffectInfo
	{
		public readonly ScreenEffect Effect;
		public bool IsLocal;

		public EffectInfo(ScreenEffect effect, bool isLocal)
		{
			if (effect == null)
				throw new ArgumentNullException("effect");

			Effect = effect;
			IsLocal = isLocal;
		}
	}
}
