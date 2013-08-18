using System;
using Blarg.GameFramework.Resources;

namespace Blarg.GameFramework.Graphics.BuiltinShaders
{
	public class DebugShader : StandardShader
	{
		public DebugShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			var vertexSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.debug.vert.glsl");
			var fragmentSources = ResourceUtils.GetTextResource("Blarg.GameFramework.Resources.Shaders.debug.frag.glsl");

			LoadCompileAndLinkInlineSources(vertexSources, fragmentSources);

			MapToVBOStandardAttribute("a_position", VertexStandardAttributes.Position3D);
			MapToVBOStandardAttribute("a_color", VertexStandardAttributes.Color);
		}
	}
}
