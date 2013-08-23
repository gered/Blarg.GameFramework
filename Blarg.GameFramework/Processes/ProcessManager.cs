using System;
using System.Collections.Generic;
using Blarg.GameFramework.States;

namespace Blarg.GameFramework.Processes
{
	public class ProcessManager : IDisposable
	{
		const string LOG_TAG = "PROCESSES";

		LinkedList<ProcessInfo> _processes;
		Queue<ProcessInfo> _queue;

		public readonly GameState GameState;

		public ProcessManager(GameState state)
		{
			if (state == null)
				throw new ArgumentNullException("state");

			GameState = state;
			_processes = new LinkedList<ProcessInfo>();
			_queue = new Queue<ProcessInfo>();
		}

		#region Complex Properties

		public bool IsTransitioning
		{
			get
			{
				for (var node = _processes.First; node != null; node = node.Next)
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
				return (_processes.Count == 0 && _queue.Count == 0);
			}
		}

		#endregion

		#region Events

		public void OnPause(bool dueToOverlay)
		{
			if (_processes.Count == 0)
				return;

			if (dueToOverlay)
			{
				Framework.Logger.Info(LOG_TAG, "Pausing all active processes due to state being overlayed on to the parent state.");
				for (var node = _processes.First; node != null; node = node.Next)
				{
					var processInfo = node.Value;
					if (!processInfo.IsInactive)
					{
						Framework.Logger.Info(LOG_TAG, "Pausing process {0} due to parent state overlay.", processInfo.Descriptor);
						processInfo.Process.OnPause(true);
					}
				}
			}
			else
			{
				Framework.Logger.Info(LOG_TAG, "Transitioning out all active processes pending pause.");
				for (var node = _processes.First; node != null; node = node.Next)
				{
					var processInfo = node.Value;
					if (!processInfo.IsInactive)
						StartTransitionOut(processInfo, false);
				}
			}
		}

		public void OnResume(bool fromOverlay)
		{
			if (_processes.Count == 0)
				return;

			if (fromOverlay)
			{
				Framework.Logger.Info(LOG_TAG, "Resuming all active processes due to overlay state being removed from overtop of parent state.");
				for (var node = _processes.First; node != null; node = node.Next)
				{
					var processInfo = node.Value;
					if (!processInfo.IsInactive)
					{
						Framework.Logger.Info(LOG_TAG, "Resuming process {0} due to overlay state removal.", processInfo.Descriptor);
						processInfo.Process.OnResume(true);
					}
				}
			}
			else
			{
				Framework.Logger.Info(LOG_TAG, "Resuming processes.");
				for (var node = _processes.First; node != null; node = node.Next)
				{
					var processInfo = node.Value;
					if (processInfo.IsInactive && !processInfo.IsBeingRemoved)
					{
						Framework.Logger.Info(LOG_TAG, "Resuming process {0}", processInfo.Descriptor);
						processInfo.Process.OnResume(false);

						StartTransitionIn(processInfo);
					}
				}
			}
		}

		public void OnAppGainFocus()
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsInactive)
					processInfo.Process.OnAppGainFocus();
			}
		}

		public void OnAppLostFocus()
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsInactive)
					processInfo.Process.OnAppLostFocus();
			}
		}

		public void OnAppPause()
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsInactive)
					processInfo.Process.OnAppPause();
			}
		}

		public void OnAppResume()
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsInactive)
					processInfo.Process.OnAppResume();
			}
		}

		public void OnLostContext()
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsInactive)
					processInfo.Process.OnLostContext();
			}
		}

		public void OnNewContext()
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsInactive)
					processInfo.Process.OnNewContext();
			}
		}

		public void OnRender(float delta)
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsInactive)
					processInfo.Process.OnRender(delta);
			}
		}

		public void OnResize()
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsInactive)
					processInfo.Process.OnResize();
			}
		}

		public void OnUpdate(float delta)
		{
			CleanupInactiveProcesses();
			CheckForFinishedProcesses();
			ProcessQueue();
			UpdateTransitions(delta);

			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsInactive)
					processInfo.Process.OnUpdate(delta);
			}
		}

		#endregion

		#region Add / Remove

		public T Add<T>(string name = null) where T : GameProcess
		{
			var newProcess = (T)Activator.CreateInstance(typeof(T), this);
			var processInfo = new ProcessInfo(newProcess, name);
			Queue(processInfo);
			return newProcess;
		}

		public void Remove(string name)
		{
			var node = GetNodeFor(name);
			if (node == null)
				throw new Exception("No process with given name.");
			StartTransitionOut(node.Value, true);
		}

		public void RemoveFirstOf<T>() where T : GameProcess
		{
			var node = GetNodeForFirst<T>();
			if (node == null)
				throw new Exception("No process of given type.");
			StartTransitionOut(node.Value, true);
		}

		public void RemoveAll()
		{
			Framework.Logger.Info(LOG_TAG, "Transitioning out all processes pending removal.");
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsTransitioning && !processInfo.IsInactive)
					StartTransitionOut(processInfo, true);
			}
		}

		private void Queue(ProcessInfo newProcessInfo)
		{
			if (newProcessInfo == null)
				throw new ArgumentNullException("newProcessInfo");
			if (newProcessInfo.Process == null)
				throw new ArgumentException("No GameProcess provided.");

			Framework.Logger.Info(LOG_TAG, "Queueing process {0}.", newProcessInfo.Descriptor);
			_queue.Enqueue(newProcessInfo);
		}

		#endregion

		#region Internal Process Management

		private void StartTransitionIn(ProcessInfo processInfo)
		{
			if (processInfo == null)
				throw new ArgumentNullException("processInfo");
			if (!processInfo.IsInactive || processInfo.IsTransitioning)
				throw new InvalidOperationException();

			processInfo.IsInactive = false;
			processInfo.IsTransitioning = true;
			processInfo.IsTransitioningOut = false;
			processInfo.IsTransitionStarting = true;
			Framework.Logger.Info(LOG_TAG, "Transition into process {0} started.", processInfo.Descriptor);
		}

		private void StartTransitionOut(ProcessInfo processInfo, bool forRemoval)
		{
			if (processInfo == null)
				throw new ArgumentNullException("processInfo");
			if (processInfo.IsInactive || processInfo.IsTransitioning)
				throw new InvalidOperationException();

			processInfo.IsTransitioning = true;
			processInfo.IsTransitioningOut = true;
			processInfo.IsTransitionStarting = true;
			processInfo.IsBeingRemoved = forRemoval;
			Framework.Logger.Info(LOG_TAG, "Transition out of process {0} started pending {1}.", processInfo.Descriptor, (forRemoval ? "removal" : "pause"));
		}

		private void CleanupInactiveProcesses()
		{
			var node = _processes.First;
			while (node != null)
			{
				var processInfo = node.Value;
				if (processInfo.IsInactive && processInfo.IsBeingRemoved)
				{
					var next = node.Next;
					_processes.Remove(node);
					node = next;

					Framework.Logger.Info(LOG_TAG, "Deleting inactive process {0}.", processInfo.Descriptor);
					processInfo.Process.Dispose();
					processInfo = null;
				}
				else
					node = node.Next;
			}
		}

		private void CheckForFinishedProcesses()
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (!processInfo.IsInactive && processInfo.Process.IsFinished && !processInfo.IsTransitioning)
				{
					Framework.Logger.Info(LOG_TAG, "Process {0} marked as finished.", processInfo.Descriptor);
					StartTransitionOut(processInfo, true);
				}
			}
		}

		private void ProcessQueue()
		{
			while (_queue.Count > 0)
			{
				var processInfo = _queue.Dequeue();

				Framework.Logger.Info(LOG_TAG, "Adding process {0} from queue.", processInfo.Descriptor);
				_processes.AddLast(processInfo);
				processInfo.Process.OnAdd();

				StartTransitionIn(processInfo);
			}
		}

		private void UpdateTransitions(float delta)
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				var processInfo = node.Value;
				if (processInfo.IsTransitioning)
				{
					bool isDone = processInfo.Process.OnTransition(delta, processInfo.IsTransitioningOut, processInfo.IsTransitionStarting);
					if (isDone)
					{
						Framework.Logger.Info(LOG_TAG, "Transition {0} into process {1} finished.",
						                          (processInfo.IsTransitioningOut ? "out of" : "into"),
						                          processInfo.Descriptor);

						// if the process was being transitioned out, then we should mark
						// it as inactive, and trigger it's OnRemove event now
						if (processInfo.IsTransitioningOut)
						{
							if (processInfo.IsBeingRemoved)
							{
								Framework.Logger.Info(LOG_TAG, "Removing process {0}.", processInfo.Descriptor);
								processInfo.Process.OnRemove();
							}
							else
							{
								Framework.Logger.Info(LOG_TAG, "Pausing process {0}.", processInfo.Descriptor);
								processInfo.Process.OnPause(false);
							}
							processInfo.IsInactive = true;
						}

						// done transitioning
						processInfo.IsTransitioning = false;
						processInfo.IsTransitioningOut = false;
					}
					processInfo.IsTransitionStarting = false;
				}
			}
		}

		#endregion

		#region Misc

		public bool IsProcessTransitioning(GameProcess process)
		{
			var processInfo = GetProcessInfoFor(process);
			if (processInfo == null)
				return false;
			else
				return processInfo.IsTransitioning;
		}

		public bool HasProcess(string name)
		{
			for (var node = _processes.First; node != null; node = node.Next)
			{
				if (!String.IsNullOrEmpty(node.Value.Name) && node.Value.Name == name)
					return true;
			}
			return false;
		}

		private LinkedListNode<ProcessInfo> GetNodeFor(string processName)
		{
			if (String.IsNullOrEmpty(processName))
				throw new ArgumentNullException("processName");

			for (var node = _processes.First; node != null; node = node.Next)
			{
				if (node.Value.Name == processName)
					return node;
			}
			return null;
		}

		private LinkedListNode<ProcessInfo> GetNodeForFirst<T>() where T : GameProcess
		{
			var type = typeof(T);
			for (var node = _processes.First; node != null; node = node.Next)
			{
				if (node.Value.Process.GetType() == type)
					return node;
			}
			return null;
		}

		private ProcessInfo GetProcessInfoFor(GameProcess process)
		{
			if (process == null)
				throw new ArgumentNullException("process");

			for (var node = _processes.First; node != null; node = node.Next)
			{
				if (node.Value.Process == process)
					return node.Value;
			}
			return null;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (_processes == null)
				return;

			Framework.Logger.Info(LOG_TAG, "ProcessManager disposing.");

			while (_processes.Count > 0)
			{
				var processInfo = _processes.Last.Value;
				Framework.Logger.Info(LOG_TAG, "Removing process {0} as part of ProcessManager shutdown.", processInfo.Descriptor);
				processInfo.Process.OnRemove();
				processInfo.Process.Dispose();
				_processes.RemoveLast();
			}

			// the queues will likely not have anything in it, but just in case ...
			while (_queue.Count > 0)
			{
				var processInfo = _queue.Dequeue();
				Framework.Logger.Info(LOG_TAG, "Removing queued process {0} as part of ProcessManager shutdown.", processInfo.Descriptor);
				processInfo.Process.Dispose();
			}

			_processes = null;
			_queue = null;
		}

		#endregion
	}
}
