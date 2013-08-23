using System;
using System.Collections.Generic;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.Events
{
	using EventListenerList = IList<IEventListener>;
	using EventTypeSet = ISet<Type>;
	using EventListenerTable = IList<IEventListener>;
	using EventListenerMap = IDictionary<Type, IList<IEventListener>>;
	using EventQueue = LinkedList<Event>;

	public class EventManager
	{
		const int NumEventQueues = 2;

		EventTypeSet _typeList;
		EventListenerMap _registry;
		EventQueue[] _queues;
		int _activeQueue;

		public EventManager()
		{
			_typeList = new HashSet<Type>();
			_registry = new Dictionary<Type, EventListenerList>();
			_queues = new EventQueue[NumEventQueues];
			for (int i = 0; i < _queues.Length; ++i)
				_queues[i] = new LinkedList<Event>();

			_activeQueue = 0;
		}

		public bool AddListener<T>(IEventListener listener) where T : Event
		{
			if (listener == null)
				throw new ArgumentNullException("listener");

			var type = typeof(T);
			EventListenerTable listenerTable = _registry.Get(type);
			if (listenerTable == null)
			{
				// need to register this listener for the given type
				listenerTable = new List<IEventListener>();
				_registry.Add(type, listenerTable);
			}

			// prevent duplicate listeners from being registered
			if (listenerTable.Contains(listener))
				throw new InvalidOperationException("Duplicate event listener registration.");

			listenerTable.Add(listener);
			Framework.Logger.Debug("EventManager", "Added {0} as a listener for event type {1}", listener.GetType().Name, type.Name);

			// also update the list of currently registered event types
			_typeList.Add(type);

			return true;
		}

		public bool RemoveListener<T>(IEventListener listener) where T : Event
		{
			if (listener == null)
				throw new ArgumentNullException("listener");

			var type = typeof(T);

			// get the list of listeners for the given event type
			EventListenerTable listenersForType = _registry.Get(type);
			if (listenersForType == null)
				return false;  // either no listeners for this type, or the listener wasn't registered with us

			if (listenersForType.Contains(listener))
			{
				listenersForType.Remove(listener);
				Framework.Logger.Debug("EventManager", "Removed {0} as a listener for event type {1}", listener.GetType().Name, type.Name);

				// if there are no more listeners for this type, remove the type
				// from the list of registered event types
				if (listenersForType.Count == 0)
					_typeList.Remove(type);

				return true;
			}
			else
				return false;
		}

		public bool Trigger(Event e)
		{
			if (e == null)
				throw new ArgumentNullException("e");

			var type = e.GetType();

			// find the listeners for the event type provided
			EventListenerTable listenersForType = _registry.Get(type);
			if (listenersForType == null)
				return false;  // no listeners for this event type have been registered -- we can't handle the event

			bool result = false;

			// trigger the event in each listener
			foreach (var listener in listenersForType)
			{
				if (listener.Handle(e))
				{
					// don't let other listeners handle the event if this one signals it handled it
					result = true;
					break;
				}
			}

			// TODO: maybe, for Trigger() only, it's better to force the calling code
			//       to "putback" the event object being triggered? since we handle the
			//       event immediately, unlike with Queue() where it makes a lot more
			//       sense for us to place it back in the pool ourselves ...
			Free(e);

			// a result of "false" merely indicates that no listener indicates
			// it "handled" the event
			return result;
		}

		public bool Queue(Event e)
		{
			if (e == null)
				throw new ArgumentNullException("e");

			// validate that there is infact a listener for this event type
			// (otherwise, we don't queue this event)
			var type = e.GetType();
			if (!_typeList.Contains(type))
				return false;

			_queues[_activeQueue].AddLast(e);

			return true;
		}

		public bool Abort<T>(bool stopAfterFirstRemoval = true) where T : Event
		{
			// validate that there is infact a listener for this event type
			// (otherwise, we don't queue this event)
			var type = typeof(T);
			if (!_typeList.Contains(type))
				return false;

			bool result = false;

			// walk through the queue and remove matching events
			// NOTE: foreach not used because we need to remove items while inside the loop
			var queue = _queues[_activeQueue];
			var node = queue.First;
			while (node != null)
			{
				// grab the next node first (so we have it before potentially removing
				// this node and then losing the link to the next one)
				var nextNode = node.Next;

				if (node.Value.GetType() == type)
				{
					// found a match, remove it
					var e = node.Value;
					queue.Remove(node);
					Free(e);
					result = true;

					if (stopAfterFirstRemoval)
						break;
				}

				node = nextNode;
			}

			return result;
		}

		public bool ProcessQueue()
		{
			// swap active queues and empty the new queue
			int queueToProcess = _activeQueue;
			_activeQueue = (_activeQueue + 1) % NumEventQueues;
			_queues[_activeQueue].Clear();

			// process the queue
			var queue = _queues[queueToProcess];
			while (queue.Count > 0)
			{
				// pop the next event off the queue
				var e = queue.First.Value;
				queue.RemoveFirst();

				var type = e.GetType();

				EventListenerTable listenersForType = _registry.Get(type);
				if (listenersForType != null)
				{
					foreach (var listener in listenersForType)
					{
						if (listener.Handle(e))
							break;    // don't let other listeners handle the event if this one signals it handled it
					}
				}

				Free(e);
			}

			return true;
		}

		public T Create<T>() where T : Event
		{
			return ObjectPools.Take<T>();
		}

		public void Free<T>(T e) where T : Event
		{
			ObjectPools.Free<T>(e);
		}
	}
}

