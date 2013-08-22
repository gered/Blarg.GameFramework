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
			return _entityManager.GetComponent<T>(this);
		}

		public T Add<T>() where T : Component
		{
			return _entityManager.AddComponent<T>(this);
		}

		public void Remove<T>() where T : Component
		{
			_entityManager.RemoveComponent<T>(this);
		}

		public bool Has<T>() where T : Component
		{
			return _entityManager.HasComponent<T>(this);
		}
	}
}

