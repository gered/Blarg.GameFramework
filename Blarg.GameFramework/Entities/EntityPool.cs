using System;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.Entities
{
	internal class EntityPool : ObjectPool<Entity>
	{
		EntityManager _entityManager;

		public EntityPool(EntityManager entityManager)
		{
			if (entityManager == null)
				throw new ArgumentNullException("entityManager");

			_entityManager = entityManager;
		}

		protected override Entity Allocate()
		{
			return new Entity(_entityManager);
		}
	}
}

