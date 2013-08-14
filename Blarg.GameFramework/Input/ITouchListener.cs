using System;

namespace Blarg.GameFramework.Input
{
	public interface ITouchListener
	{
		bool OnTouchDown(int id, int x, int y, bool isPrimary);
		bool OnTouchUp(int id, bool isPrimary);
		bool OnTouchMove(int id, int x, int y, int deltaX, int deltaY, bool isPrimary);
	}
}
