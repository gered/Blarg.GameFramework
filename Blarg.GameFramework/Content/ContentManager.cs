using System;
using System.Collections.Generic;

namespace Blarg.GameFramework.Content
{
	public class ContentManager : IDisposable
	{
		public const string LOG_TAG = "CONTENT";

		Dictionary<Type, IContentLoaderBase> _loaders;

		public bool IsLoaded { get; private set; }

		public ContentManager()
		{
			_loaders = new Dictionary<Type, IContentLoaderBase>();
			IsLoaded = false;
		}

		public void RegisterLoader(IContentLoaderBase loader)
		{
			if (loader == null)
				throw new ArgumentNullException("loader");
			if (_loaders.ContainsKey(loader.ContentType))
				throw new InvalidOperationException("Content loader already registered for this content type.");

			Framework.Logger.Info(LOG_TAG, "Registering content loader for type: {0}", loader.ContentType.ToString());
			_loaders.Add(loader.ContentType, loader);
		}

		public T Get<T>(string name) where T : class
		{
			var loader = GetLoader<T>();
			if (loader == null)
				throw new ContentManagementException("No registered loader for this content type.");
			else
				return loader.Get(name, null);

		}

		public T Get<T>(string name, object contentParameters) where T : class
		{
			var loader = GetLoader<T>();
			if (loader == null)
				throw new ContentManagementException("No registered loader for this content type.");
			else
				return loader.Get(name, contentParameters);
		}

		public T Pin<T>(string name) where T : class
		{
			var loader = GetLoader<T>();
			if (loader == null)
				throw new ContentManagementException("No registered loader for this content type.");
			else
				return loader.Pin(name, null);
		}

		public T Pin<T>(string name, object contentParameters) where T : class
		{
			var loader = GetLoader<T>();
			if (loader == null)
				throw new ContentManagementException("No registered loader for this content type.");
			else
				return loader.Pin(name, contentParameters);
		}

		public void RemoveAllContent(bool removePinnedContent = false)
		{
			foreach (var loader in _loaders)
				loader.Value.RemoveAll(removePinnedContent);
		}

		public void RemoveAllContent<T>(bool removePinnedContent = false) where T : class
		{
			var loader = GetLoader<T>();
			if (loader == null)
				throw new ContentManagementException("No registered loader for this content type.");
			else
				loader.RemoveAll(removePinnedContent);
		}

		public string GetNameOf<T>(T content) where T : class
		{
			if (content == null)
				throw new ArgumentNullException("content");

			var loader = GetLoader<T>();
			if (loader == null)
				throw new ContentManagementException("No registered loader for this content type.");
			else
				return loader.GetNameOf(content);
		}

		public IContentLoader<T> GetLoader<T>() where T : class
		{
			IContentLoaderBase loader;
			_loaders.TryGetValue(typeof(T), out loader);
			if (loader == null)
				return null;
			else
				return loader as IContentLoader<T>;
		}

		public void OnLoad()
		{
			IsLoaded = true;

			Framework.Logger.Info(LOG_TAG, "Running all content loader OnLoad events.");
			foreach (var loader in _loaders)
				loader.Value.OnLoad();
		}

		public void OnUnload()
		{
			IsLoaded = false;

			Framework.Logger.Info(LOG_TAG, "Running all content loader OnUnload events.");
			foreach (var loader in _loaders)
				loader.Value.OnUnload();
		}

		public void OnLostContext()
		{
			Framework.Logger.Info(LOG_TAG, "Running all content loader OnLostContext events.");
			foreach (var loader in _loaders)
				loader.Value.OnLostContext();
		}

		public void OnNewContext()
		{
			Framework.Logger.Info(LOG_TAG, "Running all content loader OnNewContext events.");
			foreach (var loader in _loaders)
				loader.Value.OnNewContext();
		}

		public void Dispose()
		{
			Framework.Logger.Info(LOG_TAG, "Disposing");
			foreach (var loader in _loaders)
				loader.Value.Dispose();
			_loaders.Clear();
		}
	}
}
