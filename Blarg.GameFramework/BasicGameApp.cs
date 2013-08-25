using System;
using Blarg.GameFramework.Content;
using Blarg.GameFramework.Content.Types;
using Blarg.GameFramework.Events;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.States;

namespace Blarg.GameFramework
{
	public class BasicGameApp : IGameApp
	{
		const string LOG_TAG = "BASIC_GAME_APP";

		public ContentManager ContentManager { get; private set; }
		public SpriteBatch SpriteBatch { get; private set; }
		public BillboardSpriteBatch BillboardSpriteBatch { get; private set; }
		public EventManager EventManager { get; private set; }
		public StateManager StateManager { get; private set; }

		public virtual void OnInit()
		{
			Framework.Logger.Info(LOG_TAG, "Initializing.");
			ContentManager = new ContentManager();
			SpriteBatch = new SpriteBatch(Framework.GraphicsDevice);
			BillboardSpriteBatch = new BillboardSpriteBatch(Framework.GraphicsDevice);
			EventManager = new EventManager();
			StateManager = new StateManager(EventManager);

			ContentManager.RegisterLoader(new ImageLoader(ContentManager));
			ContentManager.RegisterLoader(new TextureLoader(ContentManager));
			ContentManager.RegisterLoader(new SpriteFontLoader(ContentManager));

			Framework.Services.Register(ContentManager);
			Framework.Services.Register(SpriteBatch);
			Framework.Services.Register(BillboardSpriteBatch);
			Framework.Services.Register(EventManager);
			Framework.Services.Register(StateManager);
		}

		public virtual void OnShutdown()
		{
			Framework.Logger.Info(LOG_TAG, "Shutting down.");
			Framework.Services.Unregister<StateManager>();
			Framework.Services.Unregister<EventManager>();
			Framework.Services.Unregister<BillboardSpriteBatch>();
			Framework.Services.Unregister<SpriteBatch>();
			Framework.Services.Unregister<ContentManager>();
		}

		public virtual void OnAppGainFocus()
		{
			Framework.Logger.Info(LOG_TAG, "OnAppGainFocus");
			StateManager.OnAppGainFocus();
		}

		public virtual void OnAppLostFocus()
		{
			Framework.Logger.Info(LOG_TAG, "OnAppLostFocus");
			StateManager.OnAppLostFocus();
		}

		public virtual void OnAppPause()
		{
			Framework.Logger.Info(LOG_TAG, "OnAppPause");
			StateManager.OnAppPause();
		}

		public virtual void OnAppResume()
		{
			Framework.Logger.Info(LOG_TAG, "OnAppResume");
			StateManager.OnAppResume();
		}

		public virtual void OnLoad()
		{
			Framework.Logger.Info(LOG_TAG, "OnLoad");
			ContentManager.OnLoad();
		}

		public virtual void OnUnload()
		{
			Framework.Logger.Info(LOG_TAG, "OnUnload");
			StateManager.OnUnload();
			ContentManager.OnUnload();
		}

		public virtual void OnLostContext()
		{
			Framework.Logger.Info(LOG_TAG, "OnLostContext");
			StateManager.OnLostContext();
			ContentManager.OnLostContext();
		}

		public virtual void OnNewContext()
		{
			Framework.Logger.Info(LOG_TAG, "OnNewContext");
			ContentManager.OnNewContext();
			StateManager.OnNewContext();
		}

		public virtual void OnRender(float delta)
		{
			StateManager.OnRender(delta);
		}

		public virtual void OnResize(ScreenOrientation orientation, Rect size)
		{
			Framework.Logger.Info(LOG_TAG, "OnResize");
			StateManager.OnResize();
		}

		public virtual void OnUpdate(float delta)
		{
			EventManager.ProcessQueue();
			StateManager.OnUpdate(delta);
			if (StateManager.IsEmpty)
			{
				Framework.Logger.Info(LOG_TAG, "No states running. Quitting.");
				Framework.Application.Quit();
				return;
			}
		}

		public virtual void Dispose()
		{
			Framework.Logger.Info(LOG_TAG, "Disposing.");
			StateManager.Dispose();
			ContentManager.Dispose();
		}
	}
}

