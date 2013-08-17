using System;
using System.Diagnostics;
using SDL2;
using PortableGL;
using PortableGL.SDL;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Input;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework
{
	public class SDLLooper : BaseLooper
	{
		const string LOOPER_TAG = "SDLLOOPER";

		bool _isSDLinited;
		IntPtr _window;
		IntPtr _glContext;
		SDLLogger _logger;
		SDLKeyboard _keyboard;
		SDLMouse _mouse;
		SDLFileSystem _filesystem;
		SDLGL20 _gl;
		SDLWindow _windowInfo;
		PlatformOS _os;

		bool _isWindowActive;
		bool _isPaused;
		bool _isQuitting;

		int _targetUpdatesPerSecond;
		int _ticksPerUpdate;
		float _fixedUpdateInterval;
		float _fixedRenderInterval;
		int _maxFrameSkip = 10;

		public override PlatformOS OperatingSystem
		{
			get { return _os; }
		}

		public override PlatformType Type
		{
			get { return PlatformType.Desktop; }
		}

		public override IPlatformLogger Logger
		{
			get { return _logger; }
		}

		public override IFileSystem FileSystem
		{
			get { return _filesystem; }
		}

		public override IKeyboard Keyboard
		{
			get { return _keyboard; }
		}

		public override IMouse Mouse
		{
			get { return _mouse; }
		}

		public override ITouchScreen TouchScreen
		{
			get { return null; }
		}

		public override GL20 GL
		{
			get { return _gl; }
		}

		public SDLLooper()
		{
			_logger = new SDLLogger();
			_windowInfo = new SDLWindow();

			if (CurrentOS.IsWindows)
				_os = PlatformOS.Windows;
			else if (CurrentOS.IsLinux)
				_os = PlatformOS.Linux;
			else if (CurrentOS.IsMac)
				_os = PlatformOS.MacOS;
			else
				throw new Exception("Unable to determine OS.");

			SetUpdateFrequency(60);
		}

		public override void Run(IGameApp gameApp, IPlatformConfiguration config)
		{
			if (gameApp == null)
				throw new ArgumentNullException("gameApp");
			GameApp = gameApp;

			if (config == null)
				throw new ArgumentNullException("config");
			if (!(config is SDLConfiguration))
				throw new ArgumentException("Must pass a SDLConfiguration object.", "config");

			Logger.Info(LOOPER_TAG, "Running...");

			SDLConfiguration sdlConfig = (SDLConfiguration)config;

			Logger.Info(LOOPER_TAG, "Received SDL configuration:");
			Logger.Info(LOOPER_TAG, "\tTitle: {0}", sdlConfig.Title);
			Logger.Info(LOOPER_TAG, "\tWidth: {0}", sdlConfig.Width);
			Logger.Info(LOOPER_TAG, "\tHeight: {0}", sdlConfig.Height);
			Logger.Info(LOOPER_TAG, "\tFullscreen: {0}", sdlConfig.Fullscreen);
			Logger.Info(LOOPER_TAG, "\tResizeable: {0}", sdlConfig.Resizeable);
			Logger.Info(LOOPER_TAG, "GL Doublebuffer: {0}", sdlConfig.glDoubleBuffer);
			Logger.Info(LOOPER_TAG, "GL Depth Buffer Size: {0}", sdlConfig.glDepthBufferSize);
			Logger.Info(LOOPER_TAG, "GL Red Size: {0}", sdlConfig.glRedSize);
			Logger.Info(LOOPER_TAG, "GL Green Size: {0}", sdlConfig.glGreenSize);
			Logger.Info(LOOPER_TAG, "GL Blue Size: {0}", sdlConfig.glBlueSize);
			Logger.Info(LOOPER_TAG, "GL Alpha Size: {0}", sdlConfig.glAlphaSize);

			if (!InitSDL())
			{
				Logger.Error(LOOPER_TAG, "SDL initialization failed. Aborting.");
				return;
			}

			if (!InitSDLWindow(sdlConfig))
			{
				Logger.Error(LOOPER_TAG, "SDL window creation failed. Aborting.");
				return;
			}

			Platform.Set(this);

			OnNewContext();
			OnResize(ScreenOrientation.Rotation0, _windowInfo.ClientRectangle);
			OnLoad();

			Logger.Info(LOOPER_TAG, "Main loop starting.");
			MainLoop();
			Logger.Info(LOOPER_TAG, "Main loop finished.");

			OnUnload();
			OnLostContext();

			GameApp.Dispose();
			GameApp = null;

			ReleaseSDL();
		}

		private bool InitSDL()
		{
			Logger.Info(LOOPER_TAG, "SDL initialization starting.");

			SDL.SDL_version sdlVersion;
			SDL.SDL_VERSION(out sdlVersion);
			Logger.Info(LOOPER_TAG, "SDL Runtime Version: {0}.{1}.{2}", sdlVersion.major, sdlVersion.minor, sdlVersion.patch);
			Logger.Info(LOOPER_TAG, "SDL Linked Version: {0}.{1}.{2}", SDL.SDL_MAJOR_VERSION, SDL.SDL_MINOR_VERSION, SDL.SDL_PATCHLEVEL);

			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_GAMECONTROLLER | SDL.SDL_INIT_JOYSTICK | SDL.SDL_INIT_TIMER) == -1)
			{
				Logger.Error(LOOPER_TAG, "SDL_Init() failed: {0}", SDL.SDL_GetError());
				return false;
			}
			_isSDLinited = true;

			_keyboard = new SDLKeyboard();
			Logger.Info(LOOPER_TAG, "Keyboard input device ready.");

			_mouse = new SDLMouse();
			Logger.Info(LOOPER_TAG, "Mouse input device ready.");

			int numJoysticks = SDL.SDL_NumJoysticks();

			Logger.Info(LOOPER_TAG, "{0} joystick input devices found.", numJoysticks);
			for (int i = 0; i < numJoysticks; ++i)
			{
				Logger.Info(LOOPER_TAG, "Joystick #{0}. {1}:", (i + 1), SDL.SDL_JoystickNameForIndex(i));
				IntPtr joystick = SDL.SDL_JoystickOpen(i);
				if (joystick != IntPtr.Zero)
				{
					Logger.Info(LOOPER_TAG, "\tAxes: {0}", SDL.SDL_JoystickNumAxes(joystick));
					Logger.Info(LOOPER_TAG, "\tBalls: {0}", SDL.SDL_JoystickNumBalls(joystick));
					Logger.Info(LOOPER_TAG, "\tHats: {0}", SDL.SDL_JoystickNumHats(joystick));
					Logger.Info(LOOPER_TAG, "\tButtons: {0}", SDL.SDL_JoystickNumButtons(joystick));
					SDL.SDL_JoystickClose(joystick);
				}
				else
					Logger.Warn(LOOPER_TAG, "\tMore information could not be obtained.");
			}

			_filesystem = new SDLFileSystem();
			Logger.Info(LOOPER_TAG, "Filesystem access initialized.");

			int numVideoDrivers = SDL.SDL_GetNumVideoDrivers();
			Logger.Info(LOOPER_TAG, "Video drivers present: {0}.", numVideoDrivers);
			for (int i = 0; i < numVideoDrivers; ++i)
				Logger.Info(LOOPER_TAG, "\t{0}: {1}", (i + 1), SDL.SDL_GetVideoDriver(i));
			Logger.Info(LOOPER_TAG, "Currently using video driver: {0}", SDL.SDL_GetCurrentVideoDriver());

			int numAudioDrivers = SDL.SDL_GetNumAudioDrivers();
			Logger.Info(LOOPER_TAG, "Audio drivers present: {0}", numAudioDrivers);
			for (int i = 0; i < numAudioDrivers; ++i)
				Logger.Info(LOOPER_TAG, "\t{0}: {1}", (i + 1), SDL.SDL_GetAudioDriver(i));
			Logger.Info(LOOPER_TAG, "Currently using audio driver: {0}", SDL.SDL_GetCurrentAudioDriver());

			Logger.Info(LOOPER_TAG, "SDL initialization finished.");
			return true;
		}

		private void SetUpdateFrequency(int targetFrequency)
		{
			_targetUpdatesPerSecond = targetFrequency;
			_ticksPerUpdate = 1000 / _targetUpdatesPerSecond;
			_fixedUpdateInterval = _ticksPerUpdate / 1000.0f;
		}

		private void MainLoop()
		{
			_isWindowActive = true;
			_isPaused = false;
			_isQuitting = false;

			int numUpdatesThisFrame = 0;
			int numLoops = 0;
			int timeElapsed = 0;
			bool isDirty = false;

			int updateTime = 0;
			int renderTime = 0;
			int numUpdates = 0;
			int numRenders = 0;

			int nextUpdateAt = Environment.TickCount;
			int currentTime = Environment.TickCount;

			while (!_isQuitting)
			{
				if (_isPaused)
				{
					// we always want to be processing events, even when paused
					DoEvents();
				}
				else
				{
					int newTime = Environment.TickCount;
					int frameTime = newTime - currentTime;
					currentTime = newTime;
					timeElapsed += frameTime;

					// every second recalculate the FPS
					if (timeElapsed >= 1000)
					{
						FPS = numLoops;
						FrameTime = 1000.0f / FPS;

						RendersPerSecond = numRenders;
						UpdatesPerSecond = numUpdates;
						RenderTime = renderTime;
						UpdateTime = updateTime;

						numUpdates = 0;
						numRenders = 0;

						numLoops = 0;
						timeElapsed = 0;
					}

					// we're "running slowly" if we're more then one update behind
					if (currentTime > nextUpdateAt + _ticksPerUpdate)
						IsRunningSlowly = true;
					else
						IsRunningSlowly = false;

					numUpdatesThisFrame = 0;
					while (Environment.TickCount >= nextUpdateAt && numUpdatesThisFrame < _maxFrameSkip)
					{
						if (numUpdatesThisFrame > 0)
							IsRunningSlowly = true;

						int before = Environment.TickCount;
						DoEvents();
						OnUpdate(_fixedUpdateInterval);
						updateTime += Environment.TickCount - before;

						++numUpdatesThisFrame;
						nextUpdateAt += _ticksPerUpdate;

						++numUpdates;

						// just updated, so we need to render the new game state
						isDirty = true;
					}

					if (isDirty && _isWindowActive)
					{
						int before = Environment.TickCount;
						OnRender(_fixedRenderInterval);  // TODO
						SDL.SDL_GL_SwapWindow(_window);
						renderTime += Environment.TickCount - before;

						++numRenders;

						// don't render again until we hve something new to show
						isDirty = false;
					}

					++numLoops;
				}
			}
		}

		#region Window Management

		private bool InitSDLWindow(SDLConfiguration config)
		{
			Logger.Info(LOOPER_TAG, "SDL Window initialization starting.");

			int flags = (int)SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | (int)SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN;
			if (config.Fullscreen)
				flags |= (int)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
			if (config.Resizeable)
				flags |= (int)SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

			if (!CreateWindow(config.Title, config.Width, config.Height, flags))
				return false;

			if (!CreateOpenGLContext(config))
				return false;

			Logger.Info(LOOPER_TAG, "SDL Window initialization finished.");
			return true;
		}

		private bool CreateWindow(string title, int width, int height, int flags)
		{
			Logger.Info(LOOPER_TAG, "Attempting to set up new window with dimensions {0}x{1}.", width, height);
			if (_window != IntPtr.Zero)
				throw new InvalidOperationException("Cannot create new window before destorying existing one.");

			_window = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, (SDL.SDL_WindowFlags)flags);
			if (_window == IntPtr.Zero)
			{
				Logger.Error(LOOPER_TAG, "Window creation failed: {0}", SDL.SDL_GetError());
				return false;
			}

			SetWindowInfo();

			Logger.Info(LOOPER_TAG, "Window creation succeeded.");
			return true;
		}

		private bool DestroyWindow()
		{
			Logger.Info(LOOPER_TAG, "Destroying window.");
			if (_window == IntPtr.Zero)
			{
				Logger.Warn(LOOPER_TAG, "No window currently exists, not doing anything.");
				return true;
			}

			SDL.SDL_DestroyWindow(_window);
			_window = IntPtr.Zero;

			Logger.Info(LOOPER_TAG, "Window destroyed.");
			return true;
		}

		private void SetWindowInfo()
		{
			if (_window == IntPtr.Zero)
				throw new InvalidOperationException("Cannot set window info for a non-existant window.");

			int windowX;
			int windowY;
			SDL.SDL_GetWindowPosition(_window, out windowX, out windowY);

			int clientWidth;
			int clientHeight;
			SDL.SDL_GetWindowSize(_window, out clientWidth, out clientHeight);

			_windowInfo.ClientWidth = clientWidth;
			_windowInfo.ClientHeight = clientHeight;
			_windowInfo.ClientRectangle = new Rect(0, 0, clientWidth, clientHeight);

			Logger.Info(LOOPER_TAG, "Window content area set to {0}", _windowInfo.ClientRectangle);
		}

		#endregion

		#region OpenGL Context Management

		private bool CreateOpenGLContext(SDLConfiguration config)
		{
			Logger.Info(LOOPER_TAG, "Attempting to create OpenGL context.");
			if (_glContext != IntPtr.Zero)
				throw new InvalidOperationException("Cannoy create new OpenGL context before destroying existing one.");
			if (_window == IntPtr.Zero)
				throw new InvalidOperationException("Cannot create an OpenGL context without an existing window.");

			// minimum requirements
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, config.glDoubleBuffer ? 1 : 0);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DEPTH_SIZE, config.glDepthBufferSize);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_RED_SIZE, config.glRedSize);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_GREEN_SIZE, config.glGreenSize);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_BLUE_SIZE, config.glBlueSize);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ALPHA_SIZE, config.glAlphaSize);

			_glContext = SDL.SDL_GL_CreateContext(_window);
			if (_glContext == IntPtr.Zero)
			{
				Logger.Error(LOOPER_TAG, "OpenGL context creation failed: {0}", SDL.SDL_GetError());
				return false;
			}

			Logger.Info(LOOPER_TAG, "OpenGL context creation succeeded.");

			Logger.Info(LOOPER_TAG, "Setting OpenTK's OpenGL context and loading OpenGL extensions.");
			OpenTK.Graphics.GraphicsContext.CurrentContext = _glContext;
			OpenTK.Graphics.OpenGL.GL.LoadAll();

			_gl = new SDLGL20();

			int redSize;
			int greenSize;
			int blueSize;
			int alphaSize;
			int bufferSize;
			int doubleBuffer;
			int depthSize;
			int stencilSize;
			int accumRedSize;
			int accumGreenSize;
			int accumBlueSize;
			int accumAlphaSize;
			int stereo;
			int multisampleBuffers;
			int multisampleSamples;
			int acceleratedVisual;
			int contextMajorVersion;
			int contextMinorVersion;

			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_RED_SIZE, out redSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_GREEN_SIZE, out greenSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_BLUE_SIZE, out blueSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_ALPHA_SIZE, out alphaSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_BUFFER_SIZE, out bufferSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, out doubleBuffer);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_DEPTH_SIZE, out depthSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_STENCIL_SIZE, out stencilSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_ACCUM_RED_SIZE, out accumRedSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_ACCUM_GREEN_SIZE, out accumGreenSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_ACCUM_BLUE_SIZE, out accumBlueSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_ACCUM_ALPHA_SIZE, out accumAlphaSize);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_STEREO, out stereo);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_MULTISAMPLEBUFFERS, out multisampleBuffers);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_MULTISAMPLESAMPLES, out multisampleSamples);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_ACCELERATED_VISUAL, out acceleratedVisual);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, out contextMajorVersion);
			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, out contextMinorVersion);

			Logger.Info(LOOPER_TAG, "OpenGL context attributes:");
			Logger.Info(LOOPER_TAG, "\tGL_RED_SIZE: {0}", redSize);
			Logger.Info(LOOPER_TAG, "\tGL_GREEN_SIZE: {0}", greenSize);
			Logger.Info(LOOPER_TAG, "\tGL_BLUE_SIZE: {0}", blueSize);
			Logger.Info(LOOPER_TAG, "\tGL_ALPHA_SIZE: {0}", alphaSize);
			Logger.Info(LOOPER_TAG, "\tGL_BUFFER_SIZE: {0}", bufferSize);
			Logger.Info(LOOPER_TAG, "\tGL_DOUBLEBUFFER: {0}", doubleBuffer);
			Logger.Info(LOOPER_TAG, "\tGL_DEPTH_SIZE: {0}", depthSize);
			Logger.Info(LOOPER_TAG, "\tGL_STENCIL_SIZE: {0}", stencilSize);
			Logger.Info(LOOPER_TAG, "\tGL_ACCUM_RED_SIZE: {0}", accumRedSize);
			Logger.Info(LOOPER_TAG, "\tGL_ACCUM_GREEN_SIZE: {0}", accumGreenSize);
			Logger.Info(LOOPER_TAG, "\tGL_ACCUM_BLUE_SIZE: {0}", accumBlueSize);
			Logger.Info(LOOPER_TAG, "\tGL_ACCUM_ALPHA_SIZE: {0}", accumAlphaSize);
			Logger.Info(LOOPER_TAG, "\tGL_STEREO: {0}", stereo);
			Logger.Info(LOOPER_TAG, "\tGL_MULTISAMPLEBUFFERS: {0}", multisampleBuffers);
			Logger.Info(LOOPER_TAG, "\tGL_MULTISAMPLESAMPLES: {0}", multisampleSamples);
			Logger.Info(LOOPER_TAG, "\tGL_ACCELERATED_VISUAL: {0}", acceleratedVisual);
			Logger.Info(LOOPER_TAG, "\tGL_CONTEXT_MAJOR_VERSION: {0}", contextMajorVersion);
			Logger.Info(LOOPER_TAG, "\tGL_CONTEXT_MINOR_VERSION: {0}", contextMinorVersion);

			Logger.Info(LOOPER_TAG, "Attempting to enable V-sync.");
			if (SDL.SDL_GL_SetSwapInterval(1) != 0)
				Logger.Warn(LOOPER_TAG, "Could not set swap interval: {0}", SDL.SDL_GetError());
			else
				Logger.Info(LOOPER_TAG, "Swap interval set successful.");

			return true;
		}

		private bool DestroyOpenGLContext()
		{
			Logger.Info(LOOPER_TAG, "Destroying OpenGL context.");
			if (_glContext == IntPtr.Zero)
			{
				Logger.Warn(LOOPER_TAG, "No OpenGL context currently exists, not doing anything.");
				return true;
			}

			SDL.SDL_GL_DeleteContext(_glContext);
			OpenTK.Graphics.GraphicsContext.CurrentContext = IntPtr.Zero;
			_glContext = IntPtr.Zero;

			Logger.Info(LOOPER_TAG, "OpenGL context destroyed.");
			return true;
		}

		#endregion

		#region Events

		private void DoEvents()
		{
			SDL.SDL_Event e;
			while (SDL.SDL_PollEvent(out e) != 0)
			{
				if (e.type == SDL.SDL_EventType.SDL_WINDOWEVENT)
				{
					switch (e.window.windowEvent)
					{
						case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED:
							Logger.Info(LOOPER_TAG, "Window focus lost.");
							Logger.Info(LOOPER_TAG, "Window marked inactive.");
							break;

							case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED:
							Logger.Info(LOOPER_TAG, "Window focus gained.");
							Logger.Info(LOOPER_TAG, "Window marked active.");
							break;

							case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:
							Logger.Info(LOOPER_TAG, "Gained mouse focus.");
							break;

							case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:
							Logger.Info(LOOPER_TAG, "Lost mouse focus.");
							break;

							case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
							Logger.Info(LOOPER_TAG, "Gained input device focus.");
							OnAppGainFocus();
							break;

							case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
							Logger.Info(LOOPER_TAG, "Lost input device focus.");
							OnAppLostFocus();
							break;

							case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
							Logger.Info(LOOPER_TAG, "Window resized to {0}x{1}.", e.window.data1, e.window.data2);
							Rect size = new Rect();
							size.Right = e.window.data1;
							size.Bottom = e.window.data2;
							SetWindowInfo();
							OnResize(ScreenOrientation.Rotation0, size);
							break;
					}
				}
				else
				{
					switch (e.type)
					{
						case SDL.SDL_EventType.SDL_QUIT:
							Logger.Info(LOOPER_TAG, "Event: SQL_QUIT");
							_isQuitting = true;
							break;

							case SDL.SDL_EventType.SDL_KEYDOWN:
							case SDL.SDL_EventType.SDL_KEYUP:
							_keyboard.OnKeyEvent(e.key);
							break;

							case SDL.SDL_EventType.SDL_MOUSEMOTION:
							_mouse.OnMotionEvent(e.motion);
							break;

							case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
							case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
							_mouse.OnButtonEvent(e.button);
							break;
					}
				}
			}
		}

		#endregion

		#region IDisposable

		private void ReleaseSDL()
		{
			if (!_isSDLinited)
				return;

			Logger.Info(LOOPER_TAG, "Releasing SDL.");

			DestroyOpenGLContext();
			DestroyWindow();
			SDL.SDL_Quit();
			_isSDLinited = false;

			Logger.Info(LOOPER_TAG, "SDL shutdown.");
		}

		~SDLLooper()
		{
			ReleaseSDL();
		}

		public override void Dispose()
		{
			ReleaseSDL();
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}

