using System;
using Blarg.GameFramework.Graphics;

namespace Blarg.GameFramework.TileMap
{
	public class TileMapRenderer
	{
		ChunkRenderer _chunkRenderer = new ChunkRenderer();

		public void Render(TileMap tileMap, Shader shader = null)
		{
			var graphicsDevice = Framework.GraphicsDevice;

			if (shader == null)
			{
				graphicsDevice.BindShader(Framework.GraphicsDevice.SimpleColorTextureShader);
				graphicsDevice.SimpleColorTextureShader.SetModelViewMatrix(graphicsDevice.ViewContext.ModelViewMatrix);
				graphicsDevice.SimpleColorTextureShader.SetProjectionMatrix(graphicsDevice.ViewContext.ProjectionMatrix);
			}
			else
			{
				graphicsDevice.BindShader(shader);
			}

			for (int y = 0; y < tileMap.HeightInChunks; ++y)
			{
				for (int z = 0; z < tileMap.DepthInChunks; ++z)
				{
					for (int x = 0; x < tileMap.WidthInChunks; ++x)
					{
						var chunk = tileMap.GetChunk(x, y, z);
						var bounds = chunk.Bounds;
						if (graphicsDevice.ViewContext.Camera.Frustum.Test(ref bounds))
							_chunkRenderer.Render(chunk);
					}
				}
			}

			graphicsDevice.UnbindShader();
		}

		public void RenderAlpha(TileMap tileMap, Shader shader = null)
		{
			var graphicsDevice = Framework.GraphicsDevice;

			if (shader == null)
			{
				graphicsDevice.BindShader(Framework.GraphicsDevice.SimpleColorTextureShader);
				graphicsDevice.SimpleColorTextureShader.SetModelViewMatrix(graphicsDevice.ViewContext.ModelViewMatrix);
				graphicsDevice.SimpleColorTextureShader.SetProjectionMatrix(graphicsDevice.ViewContext.ProjectionMatrix);
			}
			else
			{
				graphicsDevice.BindShader(shader);
			}

			for (int y = 0; y < tileMap.HeightInChunks; ++y)
			{
				for (int z = 0; z < tileMap.DepthInChunks; ++z)
				{
					for (int x = 0; x < tileMap.WidthInChunks; ++x)
					{
						var chunk = tileMap.GetChunk(x, y, z);
						if (chunk.AlphaMesh != null)
						{
							var bounds = chunk.Bounds;
							if (graphicsDevice.ViewContext.Camera.Frustum.Test(ref bounds))
								_chunkRenderer.RenderAlpha(chunk);
						}
					}
				}
			}

			graphicsDevice.UnbindShader();
		}
	}
}

