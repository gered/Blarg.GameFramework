using System;
using System.Collections.Generic;
using SDL2;

namespace Blarg.GameFramework.Input
{
	public static class SDLKeyMapper
	{
		static IDictionary<SDL.SDL_Scancode, Key> _mapping = new Dictionary<SDL.SDL_Scancode, Key>();

		static SDLKeyMapper()
		{
			// borrowing and changing from the monogame-sdl2 WIP code
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_A,               Key.A);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_B,               Key.B);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_C,               Key.C);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_D,               Key.D);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_E,               Key.E);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F,               Key.F);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_G,               Key.G);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_H,               Key.H);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_I,               Key.I);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_J,               Key.J);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_K,               Key.K);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_L,               Key.L);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_M,               Key.M);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_N,               Key.N);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_O,               Key.O);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_P,               Key.P);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_Q,               Key.Q);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_R,               Key.R);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_S,               Key.S);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_T,               Key.T);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_U,               Key.U);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_V,               Key.V);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_W,               Key.W);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_X,               Key.X);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_Y,               Key.Y);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_Z,               Key.Z);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_0,               Key.Num0);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_1,               Key.Num1);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_2,               Key.Num2);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_3,               Key.Num3);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_4,               Key.Num4);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_5,               Key.Num5);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_6,               Key.Num6);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_7,               Key.Num7);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_8,               Key.Num8);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_9,               Key.Num9);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_0,            Key.Numpad0);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_1,            Key.Numpad1);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_2,            Key.Numpad2);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_3,            Key.Numpad3);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_4,            Key.Numpad4);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_5,            Key.Numpad5);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_6,            Key.Numpad6);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_7,            Key.Numpad7);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_8,            Key.Numpad8);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_9,            Key.Numpad9);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_CLEAR,        Key.Clear);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_DECIMAL,      Key.NumpadDecimal);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_DIVIDE,       Key.NumpadDivide);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_ENTER,        Key.Enter);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_MINUS,        Key.NumpadSubtract);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY,     Key.NumpadMultiply);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_PERIOD,       Key.NumpadDecimal);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_KP_PLUS,         Key.NumpadAdd);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F1,              Key.F1);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F2,              Key.F2);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F3,              Key.F3);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F4,              Key.F4);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F5,              Key.F5);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F6,              Key.F6);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F7,              Key.F7);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F8,              Key.F8);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F9,              Key.F9);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F10,             Key.F10);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F11,             Key.F11);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F12,             Key.F12);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F13,             Key.F13);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F14,             Key.F14);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F15,             Key.F15);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F16,             Key.F16);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F17,             Key.F17);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F18,             Key.F18);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F19,             Key.F19);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F20,             Key.F20);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F21,             Key.F21);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F22,             Key.F22);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F23,             Key.F23);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_F24,             Key.F24);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_SPACE,           Key.Space);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_UP,              Key.Up);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_DOWN,            Key.Down);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_LEFT,            Key.Left);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_RIGHT,           Key.Right);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_LALT,            Key.LeftAlt);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_RALT,            Key.RightAlt);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_LCTRL,           Key.LeftCtrl);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_RCTRL,           Key.RightCtrl);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_LGUI,            Key.LeftWindows);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_RGUI,            Key.RightWindows);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_LSHIFT,          Key.LeftShift);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_RSHIFT,          Key.RightShift);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_SLASH,           Key.Slash);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_BACKSLASH,       Key.BackSlash);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_LEFTBRACKET,     Key.LeftBracket);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_RIGHTBRACKET,    Key.RightBracket);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_CAPSLOCK,        Key.CapsLock);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_COMMA,           Key.Comma);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_DELETE,          Key.Delete);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_END,             Key.End);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_BACKSPACE,       Key.Back);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_RETURN,          Key.Enter);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE,          Key.Escape);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_HOME,            Key.Home);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_INSERT,          Key.Insert);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_MINUS,           Key.Minus);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_NUMLOCKCLEAR,    Key.NumLock);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_PAGEUP,          Key.PageUp);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_PAGEDOWN,        Key.PageDown);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_PAUSE,           Key.Pause);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_PERIOD,          Key.Period);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_EQUALS,          Key.Plus);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_PRINTSCREEN,     Key.PrtScr);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_APOSTROPHE,      Key.Quote);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_SCROLLLOCK,      Key.ScrollLock);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_SEMICOLON,       Key.Semicolon);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_SLEEP,           Key.Sleep);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_TAB,             Key.Tab);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_GRAVE,           Key.Tilde);
			_mapping.Add(SDL.SDL_Scancode.SDL_SCANCODE_UNKNOWN,         Key.Unknown);
		}

		public static Key ToKey(SDL.SDL_Scancode sdlKey)
		{
			Key key;
			if (_mapping.TryGetValue(sdlKey, out key))
				return key;
			else
				return Key.Unknown;
		}
	}
}

