using System;

namespace Blarg.GameFramework.Input
{
	public interface ITouchScreen
	{
		bool IsMultitouchAvailable { get; }
		bool IsTouching { get; }
		bool WasTapped { get; }
		int DownPointersCount { get; }

		ITouchPointer[] Pointers { get; }
		ITouchPointer PrimaryPointer { get; }
		ITouchPointer GetPointerById(int id);

		void OnPostUpdate(float delta);

		void Reset();

		void RegisterListener(ITouchListener listener);
		void UnregisterListener(ITouchListener listener);
	}
}
