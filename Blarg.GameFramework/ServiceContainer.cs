using System;
using System.Collections.Generic;

namespace Blarg.GameFramework
{
	public class ServiceContainer : IDisposable
	{
		Dictionary<Type, object> _services = new Dictionary<Type, object>();

		public void Register(object service)
		{
			if (service == null)
				throw new ArgumentNullException("service");
			var type = service.GetType();
			if (type.IsValueType)
				throw new ArgumentException("ServiceContainer cannot be used with value types.", "service");

			if (_services.ContainsKey(type))
				throw new InvalidOperationException("Service object of this type has already been registered.");

			if (service is IService)
				((IService)service).OnRegister();

			_services.Add(type, service);
			Platform.Logger.Debug("ServiceContainer", "Registered object of type {0}.", type);
		}

		public void Unregister(object service)
		{
			if (service == null)
				throw new ArgumentNullException("service");
			var type = service.GetType();
			if (type.IsValueType)
				throw new ArgumentException("ServiceContainer cannot be used with value types.", "service");

			object registeredService;
			_services.TryGetValue(type, out registeredService);
			if (registeredService == null)
				return;

			if (registeredService != service)
				throw new InvalidOperationException("This is not the service object that was registered under this type.");

			_services.Remove(type);
			Platform.Logger.Debug("ServiceContainer", "Unregistered object of type {0}.", type);


			if (registeredService is IService)
				((IService)registeredService).OnUnregister();
		}

		public T Get<T>() where T : class
		{
			var type = typeof(T);
			return Get(type) as T;
		}

		public object Get(Type type)
		{
			if (type.IsValueType)
				throw new ArgumentException("ServiceContainer cannot be used with value types.", "type");

			object service;
			_services.TryGetValue(type, out service);
			return service;
		}

		public void Dispose()
		{
			Platform.Logger.Debug("ServiceContainer", "Disposing.");

			foreach (var i in _services)
			{
				var service = i.Value;
				if (service is IService)
					((IService)service).OnUnregister();
			}

			_services.Clear();
		}
	}
}

