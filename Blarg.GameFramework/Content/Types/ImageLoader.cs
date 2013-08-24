using System;
using Blarg.GameFramework.Graphics;

namespace Blarg.GameFramework.Content.Types
{
	public class ImageLoader : DictionaryStoreContentLoader<Image>
	{
		public ImageLoader(ContentManager contentManager, string defaultPath = "assets://images/")
			: base(contentManager, "ImageLoader", defaultPath)
		{
		}

		protected override Image LoadContent(string file, object contentParameters)
		{
			Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: loading \"{1}\"", LoggingTag, file);

			Image content = null;

			using (var stream = Framework.FileSystem.Open(file))
			{
				if (stream == null)
					return null;

				content = new Image(stream);
			}

			return content;
		}
	}
}
