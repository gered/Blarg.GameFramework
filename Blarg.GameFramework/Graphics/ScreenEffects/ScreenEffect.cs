using System;

namespace Blarg.GameFramework.Graphics.ScreenEffects
{
	public abstract class ScreenEffect : IDisposable
	{
		public bool IsActive = true;

		public ScreenEffect()
		{
		}

		#region Events

		public virtual void OnAdd()
		{
		}

		public virtual void OnRemove()
		{
		}

		public virtual void OnAppGainFocus()
		{
		}

		public virtual void OnAppLostFocus()
		{
		}

		public virtual void OnAppPause()
		{
		}

		public virtual void OnAppResume()
		{
		}

		public virtual void OnLostContext()
		{
		}

		public virtual void OnNewContext()
		{
		}

		public virtual void OnRender(float delta, SpriteBatch spriteBatch)
		{
		}

		public virtual void OnResize()
		{
		}

		public virtual void OnUpdate(float delta)
		{
		}

		#endregion

		public virtual void Dispose()
		{
		}
	}
}
