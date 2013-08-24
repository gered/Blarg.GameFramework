using System;
using Blarg.GameFramework.Graphics;

namespace Blarg.GameFramework.Content.Types
{
	public class SpriteFontLoader : DictionaryStoreContentLoader<SpriteFont>
	{
		public SpriteFontLoader(ContentManager contentManager, string defaultPath = "assets://fonts/")
			: base(contentManager, "SpriteFontLoader", defaultPath)
		{
		}

		protected override SpriteFont LoadContent(string file, object contentParameters)
		{
			Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: loading sprite font \"{1}\"", LoggingTag, file);

			string fontFilename;
			int size;
			DecomposeFilename(file, out fontFilename, out size);

			using (var stream = Framework.FileSystem.Open(fontFilename))
			{
				if (stream == null)
					return null;

				return SpriteFontTrueTypeLoader.Load(Framework.GraphicsDevice, stream, size);
			}
		}

		public override void OnNewContext()
		{
			if (ContentStore.Count == 0)
				return;

			Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: OnNewContext.", LoggingTag);

			foreach (var item in ContentStore)
			{
				string file = item.Key;
				var font = item.Value.Content;

				Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: Reloading sprite font \"{1}\".", LoggingTag, file);

				string fontFilename;
				int size;
				DecomposeFilename(file, out fontFilename, out size);

				using (var stream = Framework.FileSystem.Open(fontFilename))
				{
					if (stream == null)
						throw new Exception("Failed to load font file when reloading sprite font.");

					SpriteFontTrueTypeLoader.Load(Framework.GraphicsDevice, stream, size, font);
				}
			}
		}

		protected override string ProcessFilename(string filename, object contentParameters)
		{
			if (String.IsNullOrEmpty(filename))
				throw new ArgumentNullException("filename");
			if (contentParameters == null)
				throw new ArgumentNullException("contentParameters");

			int? size = contentParameters as int?;
			if (size == null)
				throw new ArgumentNullException("contentParameters");

			return String.Format("{0}:{1}", filename, size.Value);
		}

		private void DecomposeFilename(string filename, out string fontFilename, out int size)
		{
			if (String.IsNullOrEmpty(filename))
				throw new ArgumentNullException("filename");

			int startOfSize = filename.LastIndexOf(':');
			if (startOfSize == -1)
				throw new InvalidOperationException("Font filename does not contain any size information.");

			fontFilename = filename.Substring(0, startOfSize);
			size = Convert.ToInt32(filename.Substring(startOfSize + 1));
		}
	}
}
