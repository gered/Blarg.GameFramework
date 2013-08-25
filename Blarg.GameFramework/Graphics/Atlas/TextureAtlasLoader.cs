using System;
using System.IO;
using Newtonsoft.Json;
using Blarg.GameFramework.Content;
using Blarg.GameFramework.IO;

namespace Blarg.GameFramework.Graphics.Atlas
{
	public static class TextureAtlasLoader
	{
		public static TextureAtlas Load(Stream file, string texturePath = null)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			var reader = new StreamReader(file);

			var definition = JsonConvert.DeserializeObject<JsonTextureAtlasDefinition>(reader.ReadToEnd());

			if (definition.Texture == null)
				throw new Exception("No texture file specified.");
			if (definition.Tiles == null || definition.Tiles.Count == 0)
				throw new Exception("No tiles defined.");

			var contentManager = Framework.Services.Get<ContentManager>();
			if (contentManager == null)
				throw new InvalidOperationException("Cannot find a ContentManager object.");

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

			return atlas;
		}
	}
}

