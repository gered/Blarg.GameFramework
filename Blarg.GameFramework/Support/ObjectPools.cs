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
			ConstructorInfo _constructor;

			public ObjectObjectPool(Type type, ConstructorInfo constructor)
				: base(INITIAL_CAPACITY)
			{
				_type = type;
				_constructor = constructor;
			}

			protected override object Allocate()
			{
				// TODO: use a compiled lambda expression instead?
				//       need to check if possible when AOT compiled
				return _constructor.Invoke(null);
			}
		}

		static Dictionary<Type, ObjectObjectPool> _pools = new Dictionary<Type, ObjectObjectPool>();

		static ConstructorInfo GetParameterlessConstructor(Type type)
		{
			var constructors = type.GetConstructors();
			for (int i = 0; i < constructors.Length; ++i)
			{
				if (constructors[i].GetParameters().Length == 0)
					return constructors[i];
			}

			return null;
		}

		static ObjectObjectPool GetPool(Type type)
		{
			ObjectObjectPool pool = _pools.Get(type);
			if (pool == null)
			{
				// why do we do this instead of just using a "new()" generic type constraint?
				// because doing it this way saves us from having to litter that same type
				// constraint on every other class method (in other classes) which might be
				// using ObjectPools to auto manage their object pools. Sometimes it gets
				// a bit restrictive ...0
				var constructor = GetParameterlessConstructor(type);
				if (constructor == null)
					throw new InvalidOperationException("ObjectPools can only be used for types with parameterless constructors.");

				pool = new ObjectObjectPool(type, constructor);
				_pools.Add(type, pool);
			}
			return pool;
		}

		public static T Take<T>() where T : class
		{
			var pool = GetPool(typeof(T));
			return (T)pool.Take();
		}

		public static void Free<T>(T obj) where T : class
		{
			var pool = GetPool(typeof(T));
			pool.Free(obj);
		}

		public static void Free(Type type, object obj)
		{
			var pool = GetPool(type);
			pool.Free(obj);
		}
	}
}

