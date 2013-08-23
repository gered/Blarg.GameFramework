using System;

namespace Blarg.GameFramework.Processes
{
	internal class ProcessInfo
	{
		public readonly GameProcess Process;
		public readonly string Name;
		public readonly string Descriptor;

		public bool IsTransitioning;
		public bool IsTransitioningOut;
		public bool IsTransitionStarting;
		public bool IsInactive;
		public bool IsBeingRemoved;

		public ProcessInfo(GameProcess process, string name = null)
		{
			if (process == null)
				throw new ArgumentNullException("process");

			Process = process;
			Name = name;
			IsInactive = true;

			if (String.IsNullOrEmpty(Name))
				Descriptor = process.GetType().Name;
			else
				Descriptor = String.Format("{0}[{1}]", process.GetType().Name, Name);
		}
	}
}
