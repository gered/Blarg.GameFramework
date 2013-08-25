using System;
using Blarg.GameFramework.Graphics;

namespace Blarg.GameFramework.Content.Types
{
	public class TextureLoader : DictionaryStoreContentLoader<Texture>
	{
		public TextureLoader(ContentManager contentManager, string defaultPath = "assets://textures/")
			: base(contentManager, "TextureLoader", defaultPath)
		{
		}

		protected override Texture LoadContent(string file, object contentParameters)
		{
			Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: loading \"{1}\"", LoggingTag, file);

			Texture content = null;

			// image loading is being done manually since probably 99% of the time after
			// the Texture object is created, the Image object is no longer needed and so
			// it doesn't make sense to keep it around in the Image content loader
			var image = LoadImage(file);
			if (image != null)
				content = new Texture(Framework.GraphicsDevice, image);

			return content;
		}

		public override void OnNewContext()
		{
			if (ContentStore.Count == 0)
				return;

			Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: OnNewContext.", LoggingTag);

			// at this point, the Texture object's should have been recreated by
			// GraphicsDevice, but the image data will not have been repopulated
			// (unless they were loaded via other means)
			foreach (var item in ContentStore)
			{
				string file = item.Key;
				var texture = item.Value.Content;

				Framework.Logger.Info(ContentManager.LOG_TAG, "{0}: Reloading texture ID {1} with image \"{2}\".", LoggingTag, texture.ID, file);

				var image = LoadImage(file);
				if (image == null)
					throw new ContentManagementException("Failed to load image when reloading texture.");

				if (texture.Width != image.Width || texture.Height != image.Height)
					throw new ContentManagementException("Image dimensions have changed since original texture creation.");

				texture.Update(image, 0, 0);
			}
		}

		private Image LoadImage(string file)
		{
			using (var stream = Framework.FileSystem.Open(file))
			{
				if (stream == null)
					return null;

				return new Image(stream);
			}
		}
	}
}
