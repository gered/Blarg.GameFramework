using System;

namespace Blarg.GameFramework.Input
{
	public interface IKeyboard
	{
		bool HasPhysicalKeysForGameControls { get; }

		bool IsDown(Key key);
		bool IsPressed(Key key);
		void Lock(Key key);

		void OnPostUpdate(float delta);

		void Reset();

		void RegisterListener(IKeyboardListener listener);
		void UnregisterListener(IKeyboardListener listener);
	}
}
