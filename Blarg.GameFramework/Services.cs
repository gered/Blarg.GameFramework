using System;
using System.Collections.Generic;

namespace Blarg.GameFramework
{
	public static class Services
	{
		static Dictionary<Type, object> _services = new Dictionary<Type, object>();

		public static void Register(object service)
		{
			if (service == null)
				throw new ArgumentNullException("service");
			var type = service.GetType();
			if (type.IsValueType)
				throw new ArgumentException("Services cannot be used with value types.", "service");

			if (_services.ContainsKey(type))
				throw new InvalidOperationException("Service object of this type has already been registered.");

			if (service is IService)
				((IService)service).OnRegister();

			_services.Add(type, service);
		}

		public static void Unregister(object service)
		{
			if (service == null)
				throw new ArgumentNullException("service");
			var type = service.GetType();
			if (type.IsValueType)
				throw new ArgumentException("Services cannot be used with value types.", "service");

			object registeredService;
			_services.TryGetValue(type, out registeredService);
			if (registeredService == null)
				return;

			if (registeredService != service)
				throw new InvalidOperationException("This is not the service object that was registered under this type.");

			_services.Remove(type);

			if (registeredService is IService)
				((IService)registeredService).OnUnregister();
		}

		public static T Get<T>() where T : class
		{
			var type = typeof(T);
			return Get(type) as T;
		}

		public static object Get(Type type)
		{
			if (type.IsValueType)
				throw new ArgumentException("Services cannot be used with value types.", "type");

			object service;
			_services.TryGetValue(type, out service);
			return service;
		}
	}
}

