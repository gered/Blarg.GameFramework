using System;

namespace Blarg.GameFramework.Entities
{
	public abstract class EntityPreset
	{
		public struct EmptyEntityPresetArgs : EntityPresetArgs
		{
		}

		public readonly EntityManager EntityManager;

		public EntityPreset(EntityManager entityManager)
		{
			if (entityManager == null)
				throw new ArgumentNullException("entityManager");

			EntityManager = entityManager;
		}

		public abstract Entity Create(EntityPresetArgs args);
	}
}

