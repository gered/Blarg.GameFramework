using System;
using System.Collections.Generic;
using Blarg.GameFramework.Events;

namespace Blarg.GameFramework.States
{
	public class StateManager : IDisposable
	{
		const string LOG_TAG = "STATES";

		LinkedList<StateInfo> _states;
		Queue<StateInfo> _pushQueue;
		Queue<StateInfo> _swapQueue;

		bool _pushQueueHasOverlay;
		bool _swapQueueHasOverlay;
		bool _lastCleanedStatesWereAllOverlays;

		public readonly IGameApp GameApp;
		public readonly EventManager EventManager;

		public int? LastReturnValue { get; private set; }

		public StateManager(IGameApp gameApp, EventManager eventManager)
		{
			if (gameApp == null)
				throw new ArgumentNullException("gameApp");
			if (eventManager == null)
				throw new ArgumentNullException("eventManager");

			GameApp = gameApp;
			EventManager = eventManager;

			_states = new LinkedList<StateInfo>();
			_pushQueue = new Queue<StateInfo>();
			_swapQueue = new Queue<StateInfo>();
		}

		#region Complex Properties

		public GameState TopState
		{
			get
			{
				var topInfo = Top;
				if (topInfo == null)
					return null;
				else
					return topInfo.State;
			}
		}

		public GameState TopNonOverlayState
		{
			get
			{
				var topInfo = TopNonOverlay;
				if (topInfo == null)
					return null;
				else
					return topInfo.State;
			}
		}

		public bool IsTransitioning
		{
			get
			{
				for (var node = _states.First; node != null; node = node.Next)
				{
					if (node.Value.IsTransitioning)
						return true;
				}
				return false;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return (_states.Count == 0 && _pushQueue.Count == 0 && _swapQueue.Count == 0);
			}
		}

		private StateInfo Top
		{
			get
			{
				if (_states.Count == 0)
					return null;
				else
					return _states.Last.Value;
			}
		}

		private StateInfo TopNonOverlay
		{
			get
			{
				var node = TopNonOverlayNode;
				if (node != null)
					return node.Value;
				else
					return null;
			}
		}

		private LinkedListNode<StateInfo> TopNonOverlayNode
		{
			get
			{
				var node = _states.Last;
				while (node != null && node.Value.IsOverlay)
					node = node.Previous;
				return node;
			}
		}

		#endregion

		#region Events

		public void OnAppGainFocus()
		{
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				if (!node.Value.IsInactive)
					node.Value.State.OnAppGainFocus();
			}
		}

		public void OnAppLostFocus()
		{
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				if (!node.Value.IsInactive)
					node.Value.State.OnAppLostFocus();
			}
		}

		public void OnAppPause()
		{
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				if (!node.Value.IsInactive)
					node.Value.State.OnAppPause();
			}
		}

		public void OnAppResume()
		{
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				if (!node.Value.IsInactive)
					node.Value.State.OnAppResume();
			}
		}

		public void OnLostContext()
		{
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				if (!node.Value.IsInactive)
					node.Value.State.OnLostContext();
			}
		}

		public void OnNewContext()
		{
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				if (!node.Value.IsInactive)
					node.Value.State.OnNewContext();
			}
		}

		public void OnRender(float delta)
		{
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				if (!node.Value.IsInactive)
				{
					node.Value.State.OnRender(delta);
					node.Value.State.EffectManager.OnRenderGlobal(delta);
				}
			}
		}

		public void OnResize()
		{
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				if (!node.Value.IsInactive)
					node.Value.State.OnResize();
			}
		}

		public void OnUpdate(float delta)
		{
			// clear return values (ensuring they're only accessible for 1 tick)
			LastReturnValue = null;
			_lastCleanedStatesWereAllOverlays = false;

			CleanupInactiveStates();
			CheckForFinishedStates();
			ProcessQueues();
			ResumeStatesIfNeeded();
			UpdateTransitions(delta);

			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				if (!node.Value.IsInactive)
					node.Value.State.OnUpdate(delta);
			}
		}

		#endregion

		#region Push / Pop / Overlay / Swap / Queue

		public T Push<T>(string name = null) where T : GameState
		{
			var newState = (T)Activator.CreateInstance(typeof(T), this);
			var stateInfo = new StateInfo(newState, name);
			QueueForPush(stateInfo);
			return newState;
		}

		public T Overlay<T>(string name = null) where T : GameState
		{
			var newState = (T)Activator.CreateInstance(typeof(T), this);
			var stateInfo = new StateInfo(newState, name);
			stateInfo.IsOverlay = true;
			QueueForPush(stateInfo);
			return newState;
		}

		public T SwapTopWith<T>(string name = null) where T : GameState
		{
			// figure out if the current top state is an overlay or not. use that
			// same setting for the new state that is to be swapped in
			var currentTopStateInfo = Top;
			if (currentTopStateInfo == null)
				throw new InvalidOperationException("Cannot swap, no existing states.");
			bool isOverlay = currentTopStateInfo.IsOverlay;

			var newState = (T)Activator.CreateInstance(typeof(T), this);
			var stateInfo = new StateInfo(newState, name);
			stateInfo.IsOverlay = isOverlay;
			QueueForSwap(stateInfo, false);
			return newState;
		}

		public T SwapTopNonOverlayWith<T>(string name = null) where T : GameState
		{
			var newState = (T)Activator.CreateInstance(typeof(T), this);
			var stateInfo = new StateInfo(newState, name);
			QueueForSwap(stateInfo, true);
			return newState;
		}

		public void Pop()
		{
			if (IsTransitioning)
				throw new InvalidOperationException();

			Platform.Logger.Info(LOG_TAG, "Pop initiated for top-most state only.");
			StartOnlyTopStateTransitioningOut(false);
		}

		public void PopTopNonOverlay()
		{
			if (IsTransitioning)
				throw new InvalidOperationException();

			Platform.Logger.Info(LOG_TAG, "Pop initiated for all top active states");
			StartTopStatesTransitioningOut(false);
		}

		private void QueueForPush(StateInfo newStateInfo)
		{
			if (newStateInfo == null)
				throw new ArgumentNullException("newStateInfo");
			if (newStateInfo.State == null)
				throw new ArgumentException("No GameState provided.");
			if (_pushQueueHasOverlay && !newStateInfo.IsOverlay)
				throw new InvalidOperationException("Cannot queue new non-overlay state while queue is active with overlay states.");

			Platform.Logger.Info(LOG_TAG, "Queueing state {0} for pushing.", newStateInfo.Descriptor);

			if (!newStateInfo.IsOverlay)
				StartTopStatesTransitioningOut(true);

			_pushQueue.Enqueue(newStateInfo);

			if (newStateInfo.IsOverlay)
				_pushQueueHasOverlay = true;
		}

		private void QueueForSwap(StateInfo newStateInfo, bool swapTopNonOverlay)
		{
			if (newStateInfo == null)
				throw new ArgumentNullException("newStateInfo");
			if (newStateInfo.State == null)
				throw new ArgumentException("No GameState provided.");
			if (_swapQueueHasOverlay && !newStateInfo.IsOverlay)
				throw new InvalidOperationException("Cannot queue new non-overlay state while queue is active with overlay states.");

			Platform.Logger.Info(LOG_TAG, "Queueing state {0} for swapping with {1}.", newStateInfo.Descriptor, (swapTopNonOverlay ? "all top active states" : "only top-most active state."));

			if (swapTopNonOverlay)
				StartTopStatesTransitioningOut(false);
			else
				StartOnlyTopStateTransitioningOut(false);

			_swapQueue.Enqueue(newStateInfo);

			if (newStateInfo.IsOverlay)
				_swapQueueHasOverlay = true;
		}

		#endregion

		#region Internal State Management

		private void StartTopStatesTransitioningOut(bool pausing)
		{
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				// only look at active states, since inactive ones have already
				// been transitioned out and will be removed on the next OnUpdate()
				if (!node.Value.IsInactive)
					TransitionOut(node.Value, !pausing);
			}
		}

		private void StartOnlyTopStateTransitioningOut(bool pausing)
		{
			var stateInfo = Top;
			// if it's not active, then it's just been transitioned out and will be
			// removed on the next OnUpdate()
			if (!stateInfo.IsInactive)
				TransitionOut(stateInfo, !pausing);
		}

		private void CleanupInactiveStates()
		{
			// we don't want to remove any states until everything is done transitioning.
			// this is to avoid the scenario where the top non-overlay state finishes 
			// transitioning before one of the overlays. if we removed it, the overlays
			// would then be overlayed over an inactive non-overlay (which wouldn't get
			// resumed until the current active overlays were done being transitioned)
			if (IsTransitioning)
				return;

			bool cleanedUpSomething = false;
			bool cleanedUpNonOverlay = false;

			var node = _states.First;
			while (node != null)
			{
				var stateInfo = node.Value;
				if (stateInfo.IsInactive && stateInfo.IsBeingPopped)
				{
					cleanedUpSomething = true;
					if (!stateInfo.IsOverlay)
						cleanedUpNonOverlay = true;

					// remove this state and move to the next node
					var next = node.Next;
					_states.Remove(node);
					node = next;

					Platform.Logger.Info(LOG_TAG, "Deleting inactive popped state {0}.", stateInfo.Descriptor);
					stateInfo.State.Dispose();
					stateInfo = null;
				}
				else
					node = node.Next;
			}

			if (cleanedUpSomething && !cleanedUpNonOverlay)
				_lastCleanedStatesWereAllOverlays = true;
		}

		private void CheckForFinishedStates()
		{
			if (_states.Count == 0)
				return;

			// don't do anything if something is currently transitioning
			if (IsTransitioning)
				return;

			bool needToAlsoTransitionOutOverlays = false;

			// check the top non-overlay state first to see if it's finished
			// and should be transitioned out
			var topNonOverlayStateInfo = TopNonOverlay;
			if (!topNonOverlayStateInfo.IsInactive && topNonOverlayStateInfo.State.IsFinished)
			{
				Platform.Logger.Info(LOG_TAG, "State {0} marked as finished.", topNonOverlayStateInfo.Descriptor);
				TransitionOut(topNonOverlayStateInfo, true);

				needToAlsoTransitionOutOverlays = true;
			}

			// now also check the overlay states (if there were any). we force them to
			// transition out if the non-overlay state started to transition out so that
			// we don't end up with overlay states without a parent non-overlay state

			// start the loop off 1 beyond the top non-overlay (which is where the
			// overlays are, if any)
			var node = TopNonOverlayNode;
			if (node != null)
			{
				for (node = node.Next; node != null; node = node.Next)
				{
					var stateInfo = node.Value;
					if (!stateInfo.IsInactive && (stateInfo.State.IsFinished || needToAlsoTransitionOutOverlays))
					{
						Platform.Logger.Info(LOG_TAG, "State {0} marked as finished.", stateInfo.Descriptor);
						TransitionOut(stateInfo, true);
					}
				}
			}
		}

		private void ProcessQueues()
		{
			// don't do anything if stuff is currently transitioning
			if (IsTransitioning)
				return;

			if (_pushQueue.Count > 0 && _swapQueue.Count > 0)
				throw new InvalidOperationException("Cannot process queues when both the swap and push queues have items in them.");

			// for each state in the queue, add it to the main list and start
			// transitioning it in
			// (note, only one of these queues will be processed each tick due to the above check!)

			while (_pushQueue.Count > 0)
			{
				var stateInfo = _pushQueue.Dequeue();

				if (_states.Count > 0)
				{
					// if this new state is an overlay, and the current top state is both
					// currently active and is not currently marked as being overlay-ed
					// then we should pause it due to overlay
					var currentTopStateInfo = Top;
					if (stateInfo.IsOverlay && !currentTopStateInfo.IsInactive && !currentTopStateInfo.IsOverlayed)
					{
						Platform.Logger.Info(LOG_TAG, "Pausing {0}state {1} due to overlay.", (currentTopStateInfo.IsOverlay ? "overlay " : ""), currentTopStateInfo.Descriptor);
						currentTopStateInfo.State.OnPause(true);

						// also mark the current top state as being overlay-ed
						currentTopStateInfo.IsOverlayed = true;
					}
				}

				Platform.Logger.Info(LOG_TAG, "Pushing {0}state {1} from push-queue.", (stateInfo.IsOverlay ? "overlay " : ""), stateInfo.Descriptor);
				stateInfo.State.OnPush();

				TransitionIn(stateInfo, false);

				_states.AddLast(stateInfo);
			}

			while (_swapQueue.Count > 0)
			{
				var stateInfo = _swapQueue.Dequeue();

				// if this new state is an overlay, and the current top state is both
				// currently active and is not currently marked as being overlay-ed
				// then we should pause it due to overlay
				var currentTopStateInfo = Top;
				if (stateInfo.IsOverlay && !currentTopStateInfo.IsInactive && !currentTopStateInfo.IsOverlayed)
				{
					Platform.Logger.Info(LOG_TAG, "Pausing {0}state {1} due to overlay.", (currentTopStateInfo.IsOverlay ? "overlay " : ""), currentTopStateInfo.Descriptor);
					currentTopStateInfo.State.OnPause(true);

					// also mark the current top state as being overlay-ed
					currentTopStateInfo.IsOverlayed = true;
				}

				Platform.Logger.Info(LOG_TAG, "Pushing {0}state {1} from swap-queue.", (stateInfo.IsOverlay ? "overlay " : ""), stateInfo.Descriptor);
				stateInfo.State.OnPush();

				TransitionIn(stateInfo, false);

				_states.AddLast(stateInfo);
			}

			_pushQueueHasOverlay = false;
			_swapQueueHasOverlay = false;
		}

		private void ResumeStatesIfNeeded()
		{
			if (_states.Count == 0)
				return;

			// don't do anything if stuff is currently transitioning
			if (IsTransitioning)
				return;

			// did we just clean up one or more overlay states?
			if (_lastCleanedStatesWereAllOverlays)
			{
				// then we need to resume the current top state
				// (those paused with the flag "from an overlay")
				var stateInfo = Top;
				if (stateInfo.IsInactive || !stateInfo.IsOverlayed)
					throw new InvalidOperationException();

				Platform.Logger.Info(LOG_TAG, "Resuming {0}state {1} due to overlay removal.", (stateInfo.IsOverlay ? "overlay " : ""), stateInfo.Descriptor);
				stateInfo.State.OnResume(true);

				stateInfo.IsOverlayed = false;

				return;
			}

			// if the top state is not inactive, then we don't need to resume anything
			if (!Top.IsInactive)
				return;

			Platform.Logger.Info(LOG_TAG, "Top-most state is inactive. Resuming all top states up to and including the next non-overlay.");

			// top state is inactive, time to resume one or more states...
			// find the topmost non-overlay state and take it and all overlay
			// states that are above it and transition them in
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				var stateInfo = node.Value;
				Platform.Logger.Info(LOG_TAG, "Resuming {0}state {1}.", (stateInfo.IsOverlay ? "overlay " : ""), stateInfo.Descriptor);
				stateInfo.State.OnResume(false);

				TransitionIn(stateInfo, true);
			}

		}

		private void UpdateTransitions(float delta)
		{
			for (var node = TopNonOverlayNode; node != null; node = node.Next)
			{
				var stateInfo = node.Value;
				if (stateInfo.IsTransitioning)
				{
					bool isDone = stateInfo.State.OnTransition(delta, stateInfo.IsTransitioningOut, stateInfo.IsTransitionStarting);
					if (isDone)
					{
						Platform.Logger.Info(LOG_TAG, "Transition {0} {1}state {2} finished.",
						                          (stateInfo.IsTransitioningOut ? "out of" : "into"),
						                          (stateInfo.IsOverlay ? "overlay " : ""),
						                          stateInfo.Descriptor);

						// if the state was being transitioned out, then we should mark
						// it as inactive, and trigger it's OnPop or OnPause event now
						if (stateInfo.IsTransitioningOut)
						{
							if (stateInfo.IsBeingPopped)
							{
								Platform.Logger.Info(LOG_TAG, "Popping {0}state {1}", (stateInfo.IsOverlay ? "overlay " : ""), stateInfo.Descriptor);
								stateInfo.State.OnPop();

								if (stateInfo.State.ReturnValue != null)
								{
									LastReturnValue = stateInfo.State.ReturnValue;
									Platform.Logger.Info(LOG_TAG, "Return value of {0} retrieved from {1}.", LastReturnValue.Value, stateInfo.Descriptor);
								}
							}
							else
							{
								Platform.Logger.Info(LOG_TAG, "Pausing {0}state {1}.", (stateInfo.IsOverlay ? "overlay " : ""), stateInfo.Descriptor);
								stateInfo.State.OnPause(false);
							}
							stateInfo.IsInactive = true;
						}

						// done transitioning
						stateInfo.IsTransitioning = false;
						stateInfo.IsTransitioningOut = false;
					}

					stateInfo.IsTransitionStarting = false;
				}
			}
		}

		private void TransitionIn(StateInfo stateInfo, bool forResuming)
		{
			stateInfo.IsInactive = false;
			stateInfo.IsTransitioning = true;
			stateInfo.IsTransitioningOut = false;
			stateInfo.IsTransitionStarting = true;
			Platform.Logger.Info(LOG_TAG, "Transition into {0}state {1} started.", (stateInfo.IsOverlay ? "overlay " : ""), stateInfo.Descriptor);

			if (forResuming)
				stateInfo.State.ProcessManager.OnResume(false);
		}

		private void TransitionOut(StateInfo stateInfo, bool forPopping)
		{
			stateInfo.IsTransitioning = true;
			stateInfo.IsTransitioningOut = true;
			stateInfo.IsTransitionStarting = true;
			stateInfo.IsBeingPopped = forPopping;
			Platform.Logger.Info(LOG_TAG, "Transition out of {0}state {1} started.", (stateInfo.IsOverlay ? "overlay " : ""), stateInfo.Descriptor);

			if (forPopping)
				stateInfo.State.ProcessManager.RemoveAll();
			else
				stateInfo.State.ProcessManager.OnPause(false);
		}

		#endregion

		#region Misc

		private StateInfo GetStateInfoFor(GameState state)
		{
			if (state == null)
				throw new ArgumentNullException("state");

			for (var node = _states.First; node != null; node = node.Next)
			{
				if (node.Value.State == state)
					return node.Value;
			}
			return null;
		}

		public bool IsStateTransitioning(GameState state)
		{
			if (state == null)
				throw new ArgumentNullException("state");

			var info = GetStateInfoFor(state);
			if (info == null)
				return false;
			else
				return info.IsTransitioning;
		}

		public bool IsTopState(GameState state)
		{
			if (state == null)
				throw new ArgumentNullException("state");

			var top = Top;
			if (top == null)
				return false;
			else
				return (top.State == state);
		}

		public bool HasState(string name)
		{
			for (var node = _states.First; node != null; node = node.Next)
			{
				if (!String.IsNullOrEmpty(node.Value.Name) && node.Value.Name == name)
					return true;
			}
			return false;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (_states == null)
				return;

			Platform.Logger.Info(LOG_TAG, "StateManager disposing.");

			while (_states.Count > 0)
			{
				var stateInfo = _states.Last.Value;
				Platform.Logger.Info(LOG_TAG, "Popping state {0} as part of StateManager shutdown.", stateInfo.Descriptor);
				stateInfo.State.OnPop();
				stateInfo.State.Dispose();
				_states.RemoveLast();
			}

			// these queues will likely not have anything in them, but just in case ...
			while (_pushQueue.Count > 0)
			{
				var stateInfo = _pushQueue.Dequeue();
				Platform.Logger.Info(LOG_TAG, "Deleting push-queued state {0} as part of StateManager shutdown.", stateInfo.Descriptor);
				stateInfo.State.Dispose();
			}
			while (_swapQueue.Count > 0)
			{
				var stateInfo = _swapQueue.Dequeue();
				Platform.Logger.Info(LOG_TAG, "Deleting swap-queued state {0} as part of StateManager shutdown.", stateInfo.Descriptor);
				stateInfo.State.Dispose();
			}

			_states = null;
			_pushQueue = null;
			_swapQueue = null;
		}

		#endregion
	}
}
