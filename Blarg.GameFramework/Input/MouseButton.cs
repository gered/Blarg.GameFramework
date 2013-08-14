using System;

namespace Blarg.GameFramework.Input
{
	// this is based off OpenTK.Input.MouseButton

	// this enum needs to include values that can encompass buttons used
	// by all framework supported platforms (even if some of them go
	// unused on certain platforms)

	public enum MouseButton
	{
		Left,
		Middle,
		Right,
		Button1,
		Button2,
		Button3,
		Button4,
		Button5,
		Button6,
		Button7,
		Button8,
		Button9,
		LastButton
	}
}
