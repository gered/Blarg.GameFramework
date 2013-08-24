using System;
using System.Collections.Generic;

namespace Blarg.GameFramework.Content
{
	public abstract class DictionaryStoreContentLoader<T> : IContentLoader<T> where T : class
	{
		string _defaultPath;

		protected readonly ContentManager ContentManager;
		protected readonly Dictionary<string, ContentContainer<T>> ContentStore;
		protected readonly string LoggingTag;

		public Type ContentType
		{
			get { return typeof(T); }
		}

		public DictionaryStoreContentLoader(ContentManager contentManager, string loggingTag, string defaultPath)
		{
			if (contentManager == null)
				throw new ArgumentNullException("contentManager");
			if (String.IsNullOrEmpty(loggingTag))
				throw new ArgumentException("Logging tag is required.");

			ContentManager = contentManager;
			ContentStore = new Dictionary<string, ContentContainer<T>>();

			LoggingTag = loggingTag;

			if (!String.IsNullOrEmpty(defaultPath))
			{
				_defaultPath = defaultPath;
				if (!_defaultPath.EndsWith("/"))
					_defaultPath += "/";

				Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: default path set to \"{1}\"", LoggingTag, _defaultPath);
			}
			else
				_defaultPath = "";
		}

		public T Get(string name, object contentParameters)
		{
			var content = GetContent(name, true, contentParameters);
			if (content != null)
				return content.Content;
			else
				return null;
		}

		public T Pin(string name, object contentParameters)
		{
			var content = GetContent(name, true, contentParameters);
			if (content != null)
			{
				content.IsPinned = true;
				return content.Content;
			}
			else
				return null;
		}

		private ContentContainer<T> GetContent(string name, bool loadIfNotAlready, object contentParameters)
		{
			if (!ContentManager.IsLoaded)
				throw new InvalidOperationException("Cannot load content until OnLoad has passed.");

			ContentContainer<T> content = null;

			string filename = AddDefaultPathIfNeeded(name);
			filename = ProcessFilename(filename, contentParameters);

			var found = FindContent(filename, contentParameters);
			if (found != null)
			{
				content = found.Value.Value;
			}
			else
			{
				if (loadIfNotAlready)
				{
					T actualContentObject = LoadContent(filename, contentParameters);
					if (actualContentObject != null)
					{
						content = new ContentContainer<T>(actualContentObject);
						ContentStore.Add(filename, content);
					}
				}
			}

			return content;
		}

		public void RemoveAll(bool removePinnedContent = false)
		{
			if (ContentStore.Count == 0)
				return;

			if (removePinnedContent)
			{
				Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: freeing all content.", LoggingTag);

				// we're removing everything here. go through all the content and
				// call Dispose() on all IDisposable's and then just clear the dictionary
				foreach (var item in ContentStore)
				{
					var content = item.Value.Content;
					if (content is IDisposable)
						(content as IDisposable).Dispose();
				}
				ContentStore.Clear();
			}
			else
			{
				Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: freeing all non-pinned content.", LoggingTag);

				// can't remove items from a dictionary inside a foreach loop iterating through it
				// build a list of all the content (names) that need to be removed
				var keysToRemove = new List<string>();
				foreach (var item in ContentStore)
				{
					if (item.Value.IsPinned == false)
						keysToRemove.Add(item.Key);
				}

				// now remove each one by name
				foreach (var key in keysToRemove)
				{
					var content = ContentStore[key].Content;
					if (content is IDisposable)
						(content as IDisposable).Dispose();
					ContentStore.Remove(key);
				}
			}
		}

		public string GetNameOf(T content)
		{
			if (content == null)
				throw new ArgumentNullException("content");

			foreach (var item in ContentStore)
			{
				if (item.Value.Content == content)
					return item.Key;
			}

			return null;
		}

		public virtual void OnLoad()
		{
		}

		public virtual void OnUnload()
		{
		}

		public virtual void OnNewContext()
		{
		}

		public virtual void OnLostContext()
		{
		}

		protected virtual KeyValuePair<string, ContentContainer<T>>? FindContent(string file, object contentParameters)
		{
			// default implementation here ignores any additional content parameters completely

			KeyValuePair<string, ContentContainer<T>>? result = null;

			ContentContainer<T> content;
			ContentStore.TryGetValue(file, out content);
			if (content != null)
				result = new KeyValuePair<string, ContentContainer<T>>(file, content);

			return result;
		}

		protected abstract T LoadContent(string file, object contentParameters);

		protected string AddDefaultPathIfNeeded(string filename)
		{
			if (filename.StartsWith("/") || String.IsNullOrEmpty(_defaultPath) || filename.StartsWith("assets://"))
				return filename;
			else
				return _defaultPath + filename;
		}

		protected virtual string ProcessFilename(string filename, object contentParameters)
		{
			return filename;
		}

		public void Dispose()
		{
			Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: disposing. Forcibly freeing any content still loaded.", LoggingTag);
			RemoveAll(true);
		}
	}
}
