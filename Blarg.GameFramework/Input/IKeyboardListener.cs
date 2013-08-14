using System;

namespace Blarg.GameFramework.Input
{
	public interface IKeyboardListener
	{
		bool OnKeyDown(Key key);
		bool OnKeyUp(Key key);
	}
}
