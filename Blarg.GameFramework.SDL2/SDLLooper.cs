using System;
using SDL2;
using Blarg.GameFramework.Input;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework
{
	public class SDLLooper : ILooper
	{
		bool _isSDLinited;
		IntPtr _window;
		IntPtr _glContext;
		SDLKeyboard _keyboard;
		SDLMouse _mouse;
		SDLFileSystem _filesystem;

		public SDLLooper()
		{
		}

		public void Run(IGameApp gameApp)
		{
			if (gameApp == null)
				throw new ArgumentNullException("gameApp");

			Platform.Services.Logger.Info("Looper", "Running...");

			if (!InitSDL())
			{
				Platform.Services.Logger.Error("Looper", "SDL initialization failed. Aborting.");
				return;
			}
		}

		private bool InitSDL()
		{
			Platform.Services.Logger.Info("Looper", "SDL initialization starting.");

			SDL.SDL_version sdlVersion;
			SDL.SDL_VERSION(out sdlVersion);
			Platform.Services.Logger.Info("Looper", "SDL Runtime Version: {0}.{1}.{2}", sdlVersion.major, sdlVersion.minor, sdlVersion.patch);
			Platform.Services.Logger.Info("Looper", "SDL Linked Version: {0}.{1}.{2}", SDL.SDL_MAJOR_VERSION, SDL.SDL_MINOR_VERSION, SDL.SDL_PATCHLEVEL);

			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_GAMECONTROLLER | SDL.SDL_INIT_JOYSTICK | SDL.SDL_INIT_TIMER) == -1)
			{
				Platform.Services.Logger.Error("Looper", "SDL_Init() failed: {0}", SDL.SDL_GetError());
				return false;
			}

			_keyboard = new SDLKeyboard();
			Platform.Services.Logger.Info("Looper", "Keyboard input device ready.");

			_mouse = new SDLMouse();
			Platform.Services.Logger.Info("Looper", "Mouse input device ready.");

			int numJoysticks = SDL.SDL_NumJoysticks();

			Platform.Services.Logger.Info("Looper", "{0} joystick input devices found.", numJoysticks);
			for (int i = 0; i < numJoysticks; ++i)
			{
				Platform.Services.Logger.Info("Looper", "Joystick #{0}. {1}:", (i + 1), SDL.SDL_JoystickNameForIndex(i));
				IntPtr joystick = SDL.SDL_JoystickOpen(i);
				if (joystick != IntPtr.Zero)
				{
					Platform.Services.Logger.Info("Looper", "\tAxes: {0}", SDL.SDL_JoystickNumAxes(joystick));
					Platform.Services.Logger.Info("Looper", "\tBalls: {0}", SDL.SDL_JoystickNumBalls(joystick));
					Platform.Services.Logger.Info("Looper", "\tHats: {0}", SDL.SDL_JoystickNumHats(joystick));
					Platform.Services.Logger.Info("Looper", "\tButtons: {0}", SDL.SDL_JoystickNumButtons(joystick));
					SDL.SDL_JoystickClose(joystick);
				}
				else
					Platform.Services.Logger.Warn("Looper", "\tMore information could not be obtained.");
			}

			_filesystem = new SDLFileSystem();
			Platform.Services.Logger.Info("Looper", "Filesystem access initialized.");

			Platform.Services.Logger.Info("Looper", "SDL initialization finished.");
			return true;
		}

		#region IDisposable

		private void ReleaseSDL()
		{
			if (!_isSDLinited)
				return;

			if (_glContext != IntPtr.Zero)
			{
				SDL.SDL_GL_DeleteContext(_glContext);
				_glContext = IntPtr.Zero;
			}
			if (_window != IntPtr.Zero)
			{
				SDL.SDL_DestroyWindow(_window);
				_window = IntPtr.Zero;
			}

			SDL.SDL_Quit();

			Platform.Services.Logger.Info("Looper", "SDL shutdown.");
		}

		~SDLLooper()
		{
			ReleaseSDL();
		}

		public void Dispose()
		{
			ReleaseSDL();
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}

