using System;
using Blarg.GameFramework.Events;
using Blarg.GameFramework.States;

namespace Blarg.GameFramework.Processes
{
	public class GameProcess : EventListener, IDisposable
	{
		public readonly GameState GameState;
		public readonly ProcessManager ProcessManager;

		public bool IsFinished { get; private set; }

		public bool IsTransitioning
		{
			get { return ProcessManager.IsProcessTransitioning(this); }
		}

		public GameProcess(ProcessManager processManager, EventManager eventManager)
			: base(eventManager)
		{
			if (processManager == null)
				throw new ArgumentNullException("processManager");
			if (eventManager == null)
				throw new ArgumentNullException("eventManager");

			GameState = processManager.GameState;
			ProcessManager = processManager;
		}

		protected void SetFinished()
		{
			IsFinished = true;
		}

		#region Events

		public virtual void OnAdd()
		{
		}

		public virtual void OnRemove()
		{
		}

		public virtual void OnPause(bool dueToOverlay)
		{
		}

		public virtual void OnResume(bool fromOverlay)
		{
		}

		public virtual void OnAppGainFocus()
		{
		}

		public virtual void OnAppLostFocus()
		{
		}

		public virtual void OnAppPause()
		{
		}

		public virtual void OnAppResume()
		{
		}

		public virtual void OnLostContext()
		{
		}

		public virtual void OnNewContext()
		{
		}

		public virtual void OnRender(float delta)
		{
		}

		public virtual void OnResize()
		{
		}

		public virtual void OnUpdate(float delta)
		{
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
		}
	}
}
