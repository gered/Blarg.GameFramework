using System;
using Blarg.GameFramework.Resources;

namespace Blarg.GameFramework.Graphics.BuiltinShaders
{
	public class SimpleColorTextureShader : StandardShader
	{
		public SimpleColorTextureShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			var vertexSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.simple_color_texture.vert.glsl");
			var fragmentSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.simple_color_texture.frag.glsl");

			LoadCompileAndLinkInlineSources(vertexSources, fragmentSources);

			MapToVBOStandardAttribute("a_position", VertexStandardAttributes.Position3D);
			MapToVBOStandardAttribute("a_color", VertexStandardAttributes.Color);
			MapToVBOStandardAttribute("a_texcoord0", VertexStandardAttributes.TexCoord);
		}
	}
}
