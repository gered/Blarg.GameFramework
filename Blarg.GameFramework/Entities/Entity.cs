using System;

namespace Blarg.GameFramework.Entities
{
	public class Entity
	{
		EntityManager _entityManager;

		internal Entity(EntityManager entityManager)
		{
			if (entityManager == null)
				throw new ArgumentNullException("entityManager");

			_entityManager = entityManager;
		}

		public T Get<T>() where T : Component
		{
			return null;
		}

		public T Add<T>() where T : Component
		{
			return null;
		}

		public void Remove<T>() where T : Component
		{
			return;
		}

		public bool Has<T>() where T : Component
		{
			return false;
		}
	}
}

