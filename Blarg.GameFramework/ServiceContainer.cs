using System;
using System.Collections.Generic;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework
{
	public class ServiceContainer : IDisposable
	{
		const string LOG_TAG = "SERVICES";

		Dictionary<Type, object> _services;

		public ServiceContainer()
		{
			_services = new Dictionary<Type, object>();
			Framework.Logger.Debug(LOG_TAG, "Ready for use.");
		}

		public void Register<T>(T service) where T : class
		{
			if (service == null)
				throw new ArgumentNullException("service");
			var type = typeof(T);
			if (_services.ContainsKey(type))
				throw new InvalidOperationException("Service object of this type has already been registered.");

			if (service is IService)
				((IService)service).OnRegister();

			_services.Add(type, service);
			Framework.Logger.Debug(LOG_TAG, "Registered object of type {0}.", type);
		}

		public void Unregister<T>() where T : class
		{
			var type = typeof(T);

			var registeredService = _services.Get(type);
			if (registeredService == null)
				return;

			_services.Remove(type);
			Framework.Logger.Debug(LOG_TAG, "Unregistered object of type {0}.", type);

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
			else
				return _services.Get(type);
		}

		public void Dispose()
		{
			Framework.Logger.Debug(LOG_TAG, "Disposing.");

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

