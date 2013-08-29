using System;
using System.IO;
using Newtonsoft.Json;
using Blarg.GameFramework.Graphics.Atlas;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.TileMap.Meshes.Json
{
	public static class TileMeshCollectionLoader
	{
		public static TileMeshCollection Load(string file, TextureAtlas atlas)
		{
			var stream = Framework.FileSystem.Open(file);
			string path = null;
			if (file.Contains("/"))
				path = file.Substring(0, file.LastIndexOf('/') + 1);

			return Load(stream, atlas, path);
		}

		public static TileMeshCollection Load(Stream file, TextureAtlas atlas, string filePath = null)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			if (atlas == null)
				throw new ArgumentNullException("atlas");

			var reader = new StreamReader(file);
			var definition = JsonConvert.DeserializeObject<JsonTileMeshCollection>(reader.ReadToEnd());

			var collection = new TileMeshCollection(atlas);

			if (definition.Tiles == null)
				throw new ConfigFileException("Missing 'Tiles' section.");

			// TODO: material mapping section

			foreach (var tileDef in definition.Tiles)
			{
				if (!tileDef.Cube && tileDef.Model == null && tileDef.Models == null)
					throw new ConfigFileException("One of Cube, Model, or Models must be specified in a tile definition.");
				if (tileDef.CollisionModel != null && tileDef.CollisionShape != null)
					throw new ConfigFileException("CollisionModel and CollisionShape cannot both be set.");

				TextureRegion? texture = null;
				TextureRegion? topTexture = null;
				TextureRegion? bottomTexture = null;
				TextureRegion? frontTexture = null;
				TextureRegion? backTexture = null;
				TextureRegion? leftTexture = null;
				TextureRegion? rightTexture = null;
				byte faces = 0;
				byte opaqueSides = 0;
				Color color = Color.White;

				if (tileDef.OpaqueSides != null)
				{
					if (tileDef.OpaqueSides.Contains("ALL"))
						opaqueSides = TileMesh.SIDE_ALL;
					else
					{
						if (tileDef.OpaqueSides.Contains("TOP"))
							opaqueSides = opaqueSides.SetBit(TileMesh.SIDE_TOP);
						if (tileDef.OpaqueSides.Contains("BOTTOM"))
							opaqueSides = opaqueSides.SetBit(TileMesh.SIDE_BOTTOM);
						if (tileDef.OpaqueSides.Contains("FRONT"))
							opaqueSides = opaqueSides.SetBit(TileMesh.SIDE_FRONT);
						if (tileDef.OpaqueSides.Contains("BACK"))
							opaqueSides = opaqueSides.SetBit(TileMesh.SIDE_BACK);
						if (tileDef.OpaqueSides.Contains("LEFT"))
							opaqueSides = opaqueSides.SetBit(TileMesh.SIDE_LEFT);
						if (tileDef.OpaqueSides.Contains("RIGHT"))
							opaqueSides = opaqueSides.SetBit(TileMesh.SIDE_RIGHT);
					}
				}

				if (tileDef.Color != null)
					color = tileDef.Color.Value;

				if (tileDef.Cube)
				{
					if (tileDef.Textures != null)
					{
						if (tileDef.Textures.Top.HasValue)
							topTexture = atlas.GetTile(tileDef.Textures.Top.Value);
						if (tileDef.Textures.Bottom.HasValue)
							bottomTexture = atlas.GetTile(tileDef.Textures.Bottom.Value);
						if (tileDef.Textures.Front.HasValue)
							frontTexture = atlas.GetTile(tileDef.Textures.Front.Value);
						if (tileDef.Textures.Back.HasValue)
							backTexture = atlas.GetTile(tileDef.Textures.Back.Value);
						if (tileDef.Textures.Left.HasValue)
							leftTexture = atlas.GetTile(tileDef.Textures.Left.Value);
						if (tileDef.Textures.Right.HasValue)
							rightTexture = atlas.GetTile(tileDef.Textures.Right.Value);
					}
					else if (tileDef.Texture.HasValue)
					{
						texture = atlas.GetTile(tileDef.Texture.Value);
						if (tileDef.Faces != null)
						{
							if (tileDef.Faces.Contains("ALL"))
								faces = TileMesh.SIDE_ALL;
							else
							{
								if (tileDef.Faces.Contains("TOP"))
									faces = faces.SetBit(TileMesh.SIDE_TOP);
								if (tileDef.Faces.Contains("BOTTOM"))
									faces = faces.SetBit(TileMesh.SIDE_BOTTOM);
								if (tileDef.Faces.Contains("FRONT"))
									faces = faces.SetBit(TileMesh.SIDE_FRONT);
								if (tileDef.Faces.Contains("BACK"))
									faces = faces.SetBit(TileMesh.SIDE_BACK);
								if (tileDef.Faces.Contains("LEFT"))
									faces = faces.SetBit(TileMesh.SIDE_LEFT);
								if (tileDef.Faces.Contains("RIGHT"))
									faces = faces.SetBit(TileMesh.SIDE_RIGHT);
							}
						}
					}
					else
						throw new ConfigFileException("No cube texture specified.");

					if (texture != null)
						collection.AddCube(texture.Value, faces, opaqueSides, tileDef.Light, tileDef.Alpha, tileDef.Translucency, color);
					else
						collection.AddCube(topTexture, bottomTexture, frontTexture, backTexture, leftTexture, rightTexture, opaqueSides, tileDef.Light, tileDef.Alpha, tileDef.Translucency, color);
				}
				else
					throw new NotImplementedException("Only cube tile definitions are implemented currently.");
			}

			return collection;
		}
	}
}

