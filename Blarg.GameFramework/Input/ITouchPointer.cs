using System;

namespace Blarg.GameFramework.Input
{
	public interface ITouchPointer
	{
		int Id { get; }
		int X { get; }
		int Y { get; }
		int DeltaX { get; }
		int DeltaY { get; }
		bool IsTouching { get; }

		bool IsTouchingWithinArea(int left, int top, int right, int bottom);
		bool IsTouchingWithinArea(ref Rect area);
		bool IsTouchingWithinArea(int centerX, int centerY, int radius);
		bool IsTouchingWithinArea(ref Circle area);
	}
}
