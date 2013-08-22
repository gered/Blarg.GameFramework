using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blarg.GameFramework.Support
{
	public static class ObjectPools
	{
		const int INITIAL_CAPACITY = 100;

		class ObjectObjectPool : ObjectPool<Object>
		{
			Type _type;

			public ObjectObjectPool(Type type)
				: base(INITIAL_CAPACITY)
			{
				_type = type;
			}

			protected override object Allocate()
			{
				return Activator.CreateInstance(_type);
			}
		}

		static Dictionary<Type, ObjectObjectPool> _pools = new Dictionary<Type, ObjectObjectPool>();

		static ObjectObjectPool GetPool(Type type)
		{
			ObjectObjectPool pool;
			_pools.TryGetValue(type, out pool);
			if (pool == null)
			{
				pool = new ObjectObjectPool(type);
				_pools.Add(type, pool);
			}
			return pool;
		}

		public static T Take<T>() where T : class, new()
		{
			var pool = GetPool(typeof(T));
			return (T)pool.Take();
		}

		public static void Free<T>(T obj) where T : class, new()
		{
			var pool = GetPool(typeof(T));
			pool.Free(obj);
		}
	}
}

