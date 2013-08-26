using System;
using Blarg.GameFramework.Graphics;

namespace Blarg.GameFramework.TileMap
{
	public class ChunkRenderer
	{
		RenderState _renderState = RenderState.Default;
		BlendState _defaultBlendState = BlendState.Default;
		BlendState _alphaBlendState = BlendState.AlphaBlend;

		public void Render(TileChunk chunk)
		{
			if (chunk.Mesh == null)
				return;

			var texture = chunk.TileMap.TileMeshes.Atlas.Texture;

			_renderState.Apply(Framework.GraphicsDevice);
			_defaultBlendState.Apply(Framework.GraphicsDevice);
			Framework.GraphicsDevice.BindTexture(texture);
			Framework.GraphicsDevice.BindVertexBuffer(chunk.Mesh);
			Framework.GraphicsDevice.RenderTriangles();
			Framework.GraphicsDevice.UnbindVertexBuffer();
		}

		public void RenderAlpha(TileChunk chunk)
		{
			if (chunk.AlphaMesh == null)
				return;

			var texture = chunk.TileMap.TileMeshes.Atlas.Texture;

			_renderState.Apply(Framework.GraphicsDevice);
			_alphaBlendState.Apply(Framework.GraphicsDevice);
			Framework.GraphicsDevice.BindTexture(texture);
			Framework.GraphicsDevice.BindVertexBuffer(chunk.AlphaMesh);
			Framework.GraphicsDevice.RenderTriangles();
			Framework.GraphicsDevice.UnbindVertexBuffer();

		}
	}
}

