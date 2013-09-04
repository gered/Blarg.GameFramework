using System;
using System.IO;
using Newtonsoft.Json;
using Blarg.GameFramework.Content;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework.Graphics.Atlas.Json
{
	public static class TextureAtlasLoader
	{
		public static TextureAtlas Load(string file, TextureAtlasAnimator animator = null)
		{
			using (var stream = Framework.FileSystem.Open(file))
			{
				string path = null;
				if (file.Contains("/"))
					path = file.Substring(0, file.LastIndexOf('/') + 1);
					
				return Load(stream, animator, path);
			}
		}

		public static TextureAtlas Load(Stream file, string texturePath = null)
		{
			return Load(file, null, texturePath);
		}

		public static TextureAtlas Load(Stream file, TextureAtlasAnimator animator, string texturePath = null)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			var reader = new StreamReader(file);
			var definition = JsonConvert.DeserializeObject<JsonTextureAtlasDefinition>(reader.ReadToEnd());
			reader.Dispose();

			if (definition.Texture == null)
				throw new ConfigFileException("No texture file specified.");
			if (definition.Tiles == null || definition.Tiles.Count == 0)
				throw new ConfigFileException("No tiles defined.");

			var contentManager = Framework.Services.Get<ContentManager>();
			if (contentManager == null)
				throw new ServiceLocatorException("Could not find a ContentManager object.");

			string textureFile = definition.Texture;
			if (!String.IsNullOrEmpty(texturePath))
				textureFile = texturePath + textureFile;

			var texture = contentManager.Get<Texture>(textureFile);
			var atlas = new CustomTextureAtlas(texture);

			for (int i = 0; i < definition.Tiles.Count; ++i) {
				var tile = definition.Tiles[i];
				// TODO: parameter value error checking
				if (tile.Autogrid)
					atlas.AddGrid(tile.X, tile.Y, tile.TileWidth, tile.TileHeight, tile.NumTilesX, tile.NumTilesY, tile.Border);
				else
					atlas.Add(tile.X, tile.Y, tile.Width, tile.Height);
			}

			if (definition.Animations != null && definition.Animations.Count > 0 && animator != null)
			{
				for (int i = 0; i < definition.Animations.Count; ++i)
				{
					var animation = definition.Animations[i];
					// TODO: parameter value error checking
					animator.AddSequence(animation.Name, 
					                     atlas, 
					                     animation.TileIndex,
					                     animation.StartIndex,
					                     animation.EndIndex,
					                     animation.Delay,
					                     animation.Loop);
				}

			}

			return atlas;
		}
	}
}

