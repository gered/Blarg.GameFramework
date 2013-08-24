using System;
using Blarg.GameFramework.Events;
using Blarg.GameFramework.Graphics.ScreenEffects;
using Blarg.GameFramework.Processes;

namespace Blarg.GameFramework.States
{
	public abstract class GameState : EventListener, IDisposable
	{
		public readonly ProcessManager ProcessManager;
		public readonly ScreenEffectManager EffectManager;
		public readonly StateManager StateManager;

		public bool IsFinished { get; private set; }
		public int? ReturnValue { get; private set; }

		public bool IsTransitioning
		{
			get { return StateManager.IsStateTransitioning(this); }
		}

		public bool IsTopState
		{
			get { return StateManager.IsTopState(this); }
		}

		public GameState(StateManager stateManager, EventManager eventManager)
			: base(eventManager)
		{
			if (stateManager == null)
				throw new ArgumentNullException("stateManager");

			StateManager = stateManager;

			EffectManager = new ScreenEffectManager();
			ProcessManager = new ProcessManager(this);
		}

		protected void SetFinished()
		{
			IsFinished = true;
			ReturnValue = null;
		}

		protected void SetFinished(int returnValue)
		{
			IsFinished = true;
			ReturnValue = returnValue;
		}

		#region Events

		public virtual void OnPush()
		{
		}

		public virtual void OnPop()
		{
		}

		public virtual void OnPause(bool dueToOverlay)
		{
			ProcessManager.OnPause(dueToOverlay);
		}

		public virtual void OnResume(bool fromOverlay)
		{
			ProcessManager.OnResume(fromOverlay);
		}

		public virtual void OnAppGainFocus()
		{
			ProcessManager.OnAppGainFocus();
			EffectManager.OnAppGainFocus();
		}

		public virtual void OnAppLostFocus()
		{
			ProcessManager.OnAppLostFocus();
			EffectManager.OnAppLostFocus();
		}

		public virtual void OnAppPause()
		{
			ProcessManager.OnAppPause();
			EffectManager.OnAppPause();
		}

		public virtual void OnAppResume()
		{
			ProcessManager.OnAppResume();
			EffectManager.OnAppResume();
		}

		public virtual void OnLostContext()
		{
			ProcessManager.OnLostContext();
			EffectManager.OnLostContext();
		}

		public virtual void OnNewContext()
		{
			ProcessManager.OnNewContext();
			EffectManager.OnNewContext();
		}

		public virtual void OnRender(float delta)
		{
			// switch it up and do effects before processes here so that processes
			// (which would commonly be used for UI overlay elements) don't get
			// overwritten by local effects (e.g. flashes, etc.)
			EffectManager.OnRenderLocal(delta);
			ProcessManager.OnRender(delta);
		}

		public virtual void OnResize()
		{
			ProcessManager.OnResize();
			EffectManager.OnResize();
		}

		public virtual void OnUpdate(float delta)
		{
			ProcessManager.OnUpdate(delta);
			EffectManager.OnUpdate(delta);
		}

		public virtual bool OnTransition(float delta, bool isTransitioningOut, bool started)
		{
			return true;
		}

		public override bool Handle(Event e)
		{
			return false;
		}

		#endregion

		public virtual void Dispose()
		{
			EffectManager.Dispose();
			ProcessManager.Dispose();
		}
	}
}
