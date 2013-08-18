using System;
using Blarg.GameFramework.Resources;

namespace Blarg.GameFramework.Graphics.BuiltinShaders
{
	public class SimpleTextureShader : StandardShader
	{
		public SimpleTextureShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			var vertexSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.simple_texture.vert.glsl");
			var fragmentSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.simple_texture.frag.glsl");

			LoadCompileAndLinkInlineSources(vertexSources, fragmentSources);

			MapToVBOStandardAttribute("a_position", VertexStandardAttributes.Position3D);
			MapToVBOStandardAttribute("a_texcoord0", VertexStandardAttributes.TexCoord);
		}
	}
}
