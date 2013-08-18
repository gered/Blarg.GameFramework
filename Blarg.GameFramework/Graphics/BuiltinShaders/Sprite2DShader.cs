using System;
using Blarg.GameFramework.Resources;

namespace Blarg.GameFramework.Graphics.BuiltinShaders
{
	public class Sprite2DShader : SpriteShader
	{
		public Sprite2DShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			var vertexSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.sprite2d.vert.glsl");
			var fragmentSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.sprite2d.frag.glsl");

			LoadCompileAndLinkInlineSources(vertexSources, fragmentSources);

			MapToVBOStandardAttribute("a_position", VertexStandardAttributes.Position2D);
			MapToVBOStandardAttribute("a_color", VertexStandardAttributes.Color);
			MapToVBOStandardAttribute("a_texcoord0", VertexStandardAttributes.TexCoord);
		}
	}
}
