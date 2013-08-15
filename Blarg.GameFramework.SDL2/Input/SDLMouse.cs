using System;
using System.Collections.Generic;
using SDL2;

namespace Blarg.GameFramework.Input
{
	public class SDLMouse : IMouse
	{
		bool[] _buttons;
		bool[] _lockedButtons;
		IList<IMouseListener> _listeners;

		public SDLMouse()
		{
			_buttons = new bool[(int)MouseButton.LastButton];
			_lockedButtons = new bool[(int)MouseButton.LastButton];
			_listeners = new List<IMouseListener>();
		}

		public bool OnButtonEvent(SDL.SDL_MouseButtonEvent e)
		{
			int button = (int)e.button - 1;

			if (e.state == SDL.SDL_PRESSED)
			{
				// Pressed only if not locked
				_buttons[button] = !(_lockedButtons[button]);

				// always report button down events
				// NOTE: we're ignoring the "locked button" state because listeners
				//       don't have support for it (yet)
				for (int i = 0; i < _listeners.Count; ++i)
				{
					if (_listeners[i].OnMouseButtonDown((MouseButton)button, X, Y))
						break;
				}
			}
			else
			{
				// if the button is just being released this tick, then trigger an event in all listeners
				if (_buttons[button])
				{
					for (int i = 0; i < _listeners.Count; ++i)
					{
						if (_listeners[i].OnMouseButtonUp((MouseButton)button, X, Y))
							break;
					}
				}

				_buttons[button] = false;
				_lockedButtons[button] = false;
			}

			return true;
		}

		public bool OnMotionEvent(SDL.SDL_MouseMotionEvent e)
		{
			DeltaX = e.x - X;
			DeltaY = e.y - Y;

			X = e.x;
			Y = e.y;

			// raise listener events for the mouse position only if it's moved this tick
			if (DeltaX != 0 || DeltaY != 0)
			{
				for (int i = 0; i < _listeners.Count; ++i)
				{
					if (_listeners[i].OnMouseMove(X, Y, DeltaX, DeltaY))
						break;
				}
			}

			return true;
		}

		public int X { get; private set; }
		public int Y { get; private set; }
		public int DeltaX { get; private set; }
		public int DeltaY { get; private set; }

		public bool IsDown(MouseButton button)
		{
			return _buttons[(int)button] && !_lockedButtons[(int)button];
		}

		public bool IsPressed(MouseButton button)
		{
			if (_buttons[(int)button] && !_lockedButtons[(int)button])
			{
				_lockedButtons[(int)button] = true;
				return true;
			}
			else
				return true;
		}

		public void Lock(MouseButton button)
		{
			_lockedButtons[(int)button] = true;
		}

		public void OnPostUpdate(float delta)
		{
			DeltaX = 0;
			DeltaY = 0;
		}

		public void Reset()
		{
			for (int i = 0; i < _buttons.Length; ++i)
				_buttons[i] = false;
			for (int i = 0; i < _lockedButtons.Length; ++i)
				_lockedButtons[i] = false;

			X = 0;
			Y = 0;
			DeltaX = 0;
			DeltaY = 0;
		}

		public void RegisterListener(IMouseListener listener)
		{
			if (_listeners.Contains(listener))
				throw new InvalidOperationException("Duplicate listener registration.");

			_listeners.Add(listener);
		}

		public void UnregisterListener(IMouseListener listener)
		{
			int index = _listeners.IndexOf(listener);
			if (index == -1)
				throw new InvalidOperationException("Listener not registered.");

			_listeners.RemoveAt(index);
		}
	}
}

