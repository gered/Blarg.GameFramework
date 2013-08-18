using System;
using Blarg.GameFramework.Resources;

namespace Blarg.GameFramework.Graphics.BuiltinShaders
{
	public class Sprite3DShader : SpriteShader
	{
		public Sprite3DShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			var vertexSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.sprite3d.vert.glsl");
			var fragmentSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.sprite3d.frag.glsl");

			LoadCompileAndLinkInlineSources(vertexSources, fragmentSources);

			MapToVBOStandardAttribute("a_position", VertexStandardAttributes.Position3D);
			MapToVBOStandardAttribute("a_color", VertexStandardAttributes.Color);
			MapToVBOStandardAttribute("a_texcoord0", VertexStandardAttributes.TexCoord);
		}
	}
}
