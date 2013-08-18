using System;
using Blarg.GameFramework.Resources;

namespace Blarg.GameFramework.Graphics.BuiltinShaders
{
	public class SimpleTextureVertexSkinningShader : VertexSkinningShader
	{
		public SimpleTextureVertexSkinningShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			var vertexSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.vertexskinning_texture.vert.glsl");
			var fragmentSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.vertexskinning_texture.frag.glsl");

			LoadCompileAndLinkInlineSources(vertexSources, fragmentSources);

			MapToVBOAttribute("a_jointIndex", 0);
			MapToVBOStandardAttribute("a_position", VertexStandardAttributes.Position3D);
			MapToVBOStandardAttribute("a_texcoord0", VertexStandardAttributes.TexCoord);
		}
	}
}
