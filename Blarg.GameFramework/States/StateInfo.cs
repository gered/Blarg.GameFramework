using System;

namespace Blarg.GameFramework.States
{
	internal class StateInfo
	{
		public readonly GameState State;
		public readonly string Name;
		public readonly string Descriptor;

		public bool IsOverlay;
		public bool IsOverlayed;
		public bool IsTransitioning;
		public bool IsTransitioningOut;
		public bool IsTransitionStarting;
		public bool IsInactive;
		public bool IsBeingPopped;

		public StateInfo(GameState state, string name = null)
		{
			if (state == null)
				throw new ArgumentNullException("state");

			State = state;
			Name = name;
			IsInactive = true;

			if (String.IsNullOrEmpty(Name))
				Descriptor = state.GetType().Name;
			else
				Descriptor = String.Format("{0}[{1}]", state.GetType().Name, Name);
		}
	}
}
