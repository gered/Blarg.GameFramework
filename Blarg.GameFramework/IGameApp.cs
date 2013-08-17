using System;
using Blarg.GameFramework.Graphics;

namespace Blarg.GameFramework
{
	public interface IGameApp : IDisposable
	{
		void OnAppGainFocus();
		void OnAppLostFocus();
		void OnAppPause();
		void OnAppResume();
		void OnLoad();
		void OnUnload();
		void OnLostContext();
		void OnNewContext();
		void OnRender(float delta);
		void OnResize(ScreenOrientation orientation, Rect size);
		void OnUpdate(float delta);
	}
}
