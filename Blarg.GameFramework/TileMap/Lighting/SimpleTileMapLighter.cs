using System;

namespace Blarg.GameFramework.TileMap.Lighting
{
	public class SimpleTileMapLighter : BaseTileMapLighter
	{
		public override void Light(TileMap tileMap)
		{
			ResetLightValues(tileMap);
			CastSkyLightDown(tileMap);
		}
	}
}

