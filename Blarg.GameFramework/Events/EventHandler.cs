using System;

namespace Blarg.GameFramework.Events
{
	public abstract class EventHandler : IEventListener
	{
		public readonly EventManager EventManager;

		public EventHandler(EventManager eventManager)
		{
			if (eventManager == null)
				throw new ArgumentNullException("eventManager");

			EventManager = eventManager;
		}

		public bool ListenFor<T>() where T : Event
		{
			return EventManager.AddListener<T>(this);
		}

		public bool StopListeningFor<T>() where T : Event
		{
			return EventManager.RemoveListener<T>(this);
		}

		public abstract bool Handle(Event e);
	}
}

