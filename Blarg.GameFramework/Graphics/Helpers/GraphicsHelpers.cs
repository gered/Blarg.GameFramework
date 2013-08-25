using System;
using Blarg.GameFramework.Graphics.Atlas;

namespace Blarg.GameFramework.Graphics.Helpers
{
	public static class GraphicsHelpers
	{
		public static void RenderCoordinateAxis(GeometryDebugRenderer debugRenderer, SpriteBatch spriteBatch, Vector3 origin, float axisLength)
		{
			var font = Framework.GraphicsDevice.SansSerifFont;

			var upLine = new LineSegment(origin.X, origin.Y, origin.Z, origin.X + 0.0f, origin.Y + axisLength, origin.Z + 0.0f);
			var downLine = new LineSegment(origin.X, origin.Y, origin.Z, origin.X + 0.0f, origin.Y + -axisLength, origin.Z + 0.0f);
			var leftLine = new LineSegment(origin.X, origin.Y, origin.Z, origin.X + -axisLength, origin.Y + 0.0f, origin.Z + 0.0f);
			var rightLine = new LineSegment(origin.X, origin.Y, origin.Z, origin.X + axisLength, origin.Y + 0.0f, origin.Z + 0.0f);
			var forwardLine = new LineSegment(origin.X, origin.Y, origin.Z, origin.X + 0.0f, origin.Y + 0.0f, origin.Z + -axisLength);
			var backwardLine = new LineSegment(origin.X, origin.Y, origin.Z, origin.X + 0.0f, origin.Y + 0.0f, origin.Z + axisLength);

			var upPos = new Vector3(origin.X, origin.Y + axisLength, origin.Z);
			var downPos = new Vector3(origin.X, origin.Y - axisLength, origin.Z);
			var leftPos = new Vector3(origin.X - axisLength, origin.Y, origin.Z);
			var rightPos = new Vector3(origin.X + axisLength, origin.Y, origin.Z);
			var forwardPos = new Vector3(origin.X, origin.Y, origin.Z - axisLength);
			var backwardPos = new Vector3(origin.X, origin.Y, origin.Z + axisLength);

			debugRenderer.Render(upLine, Color.White);
			spriteBatch.Render(font, upPos, Color.White, "UP (+Y)");

			debugRenderer.Render(downLine, Color.Black);
			spriteBatch.Render(font, downPos, Color.Black, "DOWN (-Y)");

			debugRenderer.Render(leftLine, Color.Green);
			spriteBatch.Render(font, leftPos, Color.Green, "LEFT (-X)");

			debugRenderer.Render(rightLine, Color.Red);
			spriteBatch.Render(font, rightPos, Color.Red, "RIGHT (+X)");

			debugRenderer.Render(forwardLine, Color.Cyan);
			spriteBatch.Render(font, forwardPos, Color.Cyan, "FORWARD (-Z)");

			debugRenderer.Render(backwardLine, Color.Yellow);
			spriteBatch.Render(font, backwardPos, Color.Yellow, "BACKWARD (+Z)");
		}

		public static Vector2 ScaleTexCoord(Vector2 texCoord, TextureRegion tile) {
			Vector2 result;
			result.X = ScaleTexCoordU(texCoord.X, tile);
			result.Y = ScaleTexCoordV(texCoord.Y, tile);
			return result;
		}

		public static Vector2 ScaleTexCoord(Vector2 texCoord, float srcMin, float srcMax, TextureRegion tile) {
			Vector2 result;
			result.X = ScaleTexCoordU(texCoord.X, srcMin, srcMax, tile);
			result.Y = ScaleTexCoordV(texCoord.Y, srcMin, srcMax, tile);
			return result;
		}

		public static float ScaleTexCoordU(float u, TextureRegion tile) {
			return MathHelpers.ScaleRange(u, 0.0f, 1.0f, tile.TexCoords.Left, tile.TexCoords.Right);
		}

		public static float ScaleTexCoordU(float u, float srcMinU, float srcMaxU, TextureRegion tile) {
			return MathHelpers.ScaleRange(u, srcMinU, srcMaxU, tile.TexCoords.Left, tile.TexCoords.Right);
		}

		public static float ScaleTexCoordV(float v, TextureRegion tile) {
			return MathHelpers.ScaleRange(v, 0.0f, 1.0f, tile.TexCoords.Top, tile.TexCoords.Bottom);
		}

		public static float ScaleTexCoordV(float v, float srcMinV, float srcMaxV, TextureRegion tile) {
			return MathHelpers.ScaleRange(v, srcMinV, srcMaxV, tile.TexCoords.Top, tile.TexCoords.Bottom);
		}
	}
}

