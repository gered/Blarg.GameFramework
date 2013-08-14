using System;

namespace Blarg.GameFramework.Input
{
	public interface IMouseListener
	{
		bool OnMouseButtonDown(MouseButton button, int x, int y);
		bool OnMouseButtonUp(MouseButton button, int x, int y);
		bool OnMouseMove(int x, int y, int deltaX, int deltaY);
	}
}
