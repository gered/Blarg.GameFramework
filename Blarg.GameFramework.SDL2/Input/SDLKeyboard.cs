using System;
using System.Collections.Generic;
using SDL2;

namespace Blarg.GameFramework.Input
{
	public class SDLKeyboard : IKeyboard
	{
		bool[] _keys;
		bool[] _lockedKeys;
		List<IKeyboardListener> _listeners;

		public SDLKeyboard()
		{
			_keys = new bool[(int)Key.LastKey];
			_lockedKeys = new bool[(int)Key.LastKey];
			_listeners = new List<IKeyboardListener>();
		}

		public bool OnKeyEvent(SDL.SDL_KeyboardEvent e)
		{
			int keyCode = (int)SDLKeyMapper.ToKey(e.keysym.scancode);

			if (e.state == SDL.SDL_PRESSED)
			{
				_keys[keyCode] = !(_lockedKeys[keyCode]);   // pressed only if not locked

				// always report keydown events
				// NOTE: we're ignoring the "locked key" state because listeners
				//       don't have support for it (yet)
				for (int i = 0; i < _listeners.Count; ++i)
				{
					if (_listeners[i].OnKeyDown((Key)keyCode))
						break;
				}
			}
			else
			{
				// if the key is just being released this tick, then trigger an event in all listeners
				if (_keys[keyCode])
				{
					for (int i = 0; i < _listeners.Count; ++i)
					{
						if (_listeners[i].OnKeyUp((Key)keyCode))
							break;
					}
				}

				_keys[keyCode] = false;
				_lockedKeys[keyCode] = false;
			}

			return true;
		}

		public bool IsDown(Key key)
		{
			return _keys[(int)key] && !_lockedKeys[(int)key];
		}

		public bool IsPressed(Key key)
		{
			if (_keys[(int)key] && !_lockedKeys[(int)key])
			{
				_lockedKeys[(int)key] = true;
				return true;
			}
			else
				return false;
		}

		public void Lock(Key key)
		{
			_lockedKeys[(int)key] = true;
		}

		public void OnPostUpdate(float delta)
		{
		}

		public void Reset()
		{
			for (int i = 0; i < _keys.Length; ++i)
				_keys[i] = false;
			for (int i = 0; i < _lockedKeys.Length; ++i)
				_lockedKeys[i] = false;
		}

		public void RegisterListener(IKeyboardListener listener)
		{
			if (_listeners.Contains(listener))
				throw new InvalidOperationException("Duplicate listener registration.");

			_listeners.Add(listener);
		}

		public void UnregisterListener(IKeyboardListener listener)
		{
			int index = _listeners.IndexOf(listener);
			if (index == -1)
				throw new InvalidOperationException("Listener not registered.");

			_listeners.RemoveAt(index);
		}

		public bool HasPhysicalKeysForGameControls
		{
			get { return true; }
		}
	}
}

