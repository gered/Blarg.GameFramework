using System;

namespace Blarg.GameFramework.Input
{
	public interface IMouse
	{
		bool IsDown(MouseButton button);
		bool IsPressed(MouseButton button);
		void Lock(MouseButton button);

		int X { get; }
		int Y { get; }
		int DeltaX { get; }
		int DeltaY { get; }

		void OnPostUpdate(float delta);

		void Reset();

		void RegisterListener(IMouseListener listener);
		void UnregisterListener(IMouseListener listener);
	}
}
