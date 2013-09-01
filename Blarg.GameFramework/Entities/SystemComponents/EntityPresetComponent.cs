using System;

namespace Blarg.GameFramework.Entities.SystemComponents
{
	public class EntityPresetComponent : Component
	{
		public Type PresetType;

		public override void Reset()
		{
			PresetType = null;
		}
	}
}

