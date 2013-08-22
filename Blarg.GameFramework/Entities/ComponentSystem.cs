using System;
using Blarg.GameFramework.Events;

namespace Blarg.GameFramework.Entities
{
	public class ComponentSystem : EventListener, IDisposable
	{
		public readonly EntityManager EntityManager;

		public ComponentSystem(EntityManager entityManager, EventManager eventManager)
			: base(eventManager)
		{
			if (entityManager == null)
				throw new ArgumentNullException("entityManager");

			EntityManager = entityManager;
		}

		public virtual void OnAppPause()
		{
		}

		public virtual void OnAppResume()
		{
		}

		public virtual void OnResize()
		{
		}

		public virtual void OnRender(float delta)
		{
		}

		public virtual void OnUpdate(float delta)
		{
		}

		public override bool Handle(Event e)
		{
			return false;
		}

		public virtual void Dispose()
		{
		}
	}
}

