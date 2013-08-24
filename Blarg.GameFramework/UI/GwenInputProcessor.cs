using System;
using Blarg.GameFramework.Input;

namespace Blarg.GameFramework.UI
{
	public class GwenInputProcessor : IKeyboardListener, IMouseListener, ITouchListener
	{
		Gwen.Control.Canvas _canvas;
		bool _isEnabled;

		public GwenInputProcessor(Gwen.Control.Canvas canvas)
		{
			if (canvas == null)
				throw new ArgumentNullException("canvas");

			_canvas = canvas;
		}

		#region Enable / Disable

		public bool Enabled
		{
			get { return _isEnabled; }
			set { _isEnabled = Enable(value); }
		}

		private bool Enable(bool enable)
		{
			if (_isEnabled != enable)
			{
				var keyboard = Framework.Keyboard;
				var mouse = Framework.Mouse;
				var touchscreen = Framework.TouchScreen;

				if (enable)
				{
					if (keyboard != null)
						keyboard.RegisterListener(this);
					if (mouse != null)
						mouse.RegisterListener(this);
					if (touchscreen != null)
						touchscreen.RegisterListener(this);
				}
				else
				{
					if (keyboard != null)
						keyboard.UnregisterListener(this);
					if (mouse != null)
						mouse.UnregisterListener(this);
					if (touchscreen != null)
						touchscreen.UnregisterListener(this);
				}
			}

			return enable;
		}

		#endregion

		#region Keyboard Events

		public bool OnKeyDown(Key key)
		{
			char ch = ConvertKeyToChar(key);
			if (Gwen.Input.InputHandler.DoSpecialKeys(_canvas, ch))
				return false;

			var gwenKey = ConvertToGwenKey(key);
			return _canvas.Input_Key(gwenKey, true);
		}

		public bool OnKeyUp(Key key)
		{
			var gwenKey = ConvertToGwenKey(key);
			return _canvas.Input_Key(gwenKey, false);
		}

		#endregion

		#region Mouse Events

		public bool OnMouseButtonDown(MouseButton button, int x, int y)
		{
			int gwenButton = (int)button;

			int scaledX = (int)((float)x / _canvas.Scale);
			int scaledY = (int)((float)y / _canvas.Scale);

			// trigger mouse move event for button events to ensure GWEN
			// knows where the button event occured at
			bool movedResult = _canvas.Input_MouseMoved(scaledX, scaledY, 0, 0);
			bool clickResult = _canvas.Input_MouseButton(gwenButton, true);

			// TODO: is this really the right way to do this .. ?
			return (movedResult || clickResult);
		}

		public bool OnMouseButtonUp(MouseButton button, int x, int y)
		{
			int gwenButton = (int)button;

			int scaledX = (int)((float)x / _canvas.Scale);
			int scaledY = (int)((float)y / _canvas.Scale);

			// trigger mouse move event for button events to ensure GWEN
			// knows where the button event occured at
			bool movedResult = _canvas.Input_MouseMoved(scaledX, scaledY, 0, 0);
			bool clickResult = _canvas.Input_MouseButton(gwenButton, false);

			// TODO: is this really the right way to do this .. ?
			return (movedResult || clickResult);
		}

		public bool OnMouseMove(int x, int y, int deltaX, int deltaY)
		{
			// Gwen's input handling only processes coordinates in terms of scale = 1.0f
			int scaledX = (int)((float)x / _canvas.Scale);
			int scaledY = (int)((float)y / _canvas.Scale);
			int scaledDeltaX = (int)((float)deltaX / _canvas.Scale);
			int scaledDeltaY = (int)((float)deltaY / _canvas.Scale);

			return _canvas.Input_MouseMoved(scaledX, scaledY, scaledDeltaX, scaledDeltaY);
		}

		#endregion

		#region Touchscreen Events

		public bool OnTouchDown(int id, int x, int y, bool isPrimary)
		{
			if (!isPrimary)
				return false;

			// Gwen's input handling only processes coordinates in terms of scale = 1.0f
			int scaledX = (int)((float)x / _canvas.Scale);
			int scaledY = (int)((float)y / _canvas.Scale);

			bool movedResult = _canvas.Input_MouseMoved(scaledX, scaledY, 0, 0);
			bool clickResult = _canvas.Input_MouseButton(0, true);

			// TODO: is this really the right way to do this .. ?
			return (movedResult || clickResult);
		}

		public bool OnTouchUp(int id, bool isPrimary)
		{
			if (!isPrimary)
				return false;

			bool clickResult = _canvas.Input_MouseButton(0, false);

			// we do this so that GWEN isn't left thinking that the "mouse" is
			// hovering over whatever we were just clicking/touching. This is
			// done because obviously with a touchscreen, you don't hover over
			// anything unless you are clicking/touching...
			bool movedResult = _canvas.Input_MouseMoved(-1, -1, 0, 0);

			// TODO: is this really the right way to do this .. ?
			return (movedResult || clickResult);
		}

		public bool OnTouchMove(int id, int x, int y, int deltaX, int deltaY, bool isPrimary)
		{
			if (!isPrimary)
				return false;

			// Gwen's input handling only processes coordinates in terms of scale = 1.0f
			int scaledX = (int)((float)x / _canvas.Scale);
			int scaledY = (int)((float)y / _canvas.Scale);
			int scaledDeltaX = (int)((float)deltaX / _canvas.Scale);
			int scaledDeltaY = (int)((float)deltaY / _canvas.Scale);

			bool movedResult = _canvas.Input_MouseMoved(scaledX, scaledY, scaledDeltaX, scaledDeltaY);
			bool clickResult = _canvas.Input_MouseButton(0, true);

			// TODO: is this really the right way to do this .. ?
			return (movedResult || clickResult);
		}

		#endregion

		#region Misc

		private char ConvertKeyToChar(Key key)
		{
			if (key >= Key.A && key <= Key.Z)
				return (char)('a' + ((int)key - (int)Key.A));
			else
				return ' ';
		}

		private Gwen.Key ConvertToGwenKey(Key key)
		{
			switch (key)
			{
				case Key.Backspace: return Gwen.Key.Backspace;
					case Key.Enter: return Gwen.Key.Return;
					case Key.Escape: return Gwen.Key.Escape;
					case Key.Tab: return Gwen.Key.Tab;
					case Key.Space: return Gwen.Key.Space;
					case Key.Up: return Gwen.Key.Up;
					case Key.Down: return Gwen.Key.Down;
					case Key.Left: return Gwen.Key.Left;
					case Key.Right: return Gwen.Key.Right;
					case Key.Home: return Gwen.Key.Home;
					case Key.End: return Gwen.Key.End;
					case Key.Delete: return Gwen.Key.Delete;
					case Key.LeftCtrl: return Gwen.Key.Control;
					case Key.LeftAlt: return Gwen.Key.Alt;
					case Key.LeftShift: return Gwen.Key.Shift;
					case Key.RightCtrl: return Gwen.Key.Control;
					case Key.RightAlt: return Gwen.Key.Alt;
					case Key.RightShift: return Gwen.Key.Shift;
			}

			return Gwen.Key.Invalid;
		}

		#endregion
	}
}
