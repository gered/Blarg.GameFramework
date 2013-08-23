using System;
using System.Collections.Generic;

namespace Blarg.GameFramework.Graphics.ScreenEffects
{
	public class ScreenEffectManager : IDisposable
	{
		LinkedList<EffectInfo> _effects;
		int _numLocalEffects;
		int _numGlobalEffects;

		public ScreenEffectManager()
		{
			_effects = new LinkedList<EffectInfo>();
			_numLocalEffects = 0;
			_numGlobalEffects = 0;
		}

		#region Get / Add / Remove

		public T Get<T>() where T : ScreenEffect
		{
			var type = typeof(T);
			var node = GetNodeFor(type);
			if (node == null)
				return null;
			else
				return (T)node.Value.Effect;
		}

		public T Add<T>(bool isLocalEffect = true) where T : ScreenEffect
		{
			var type = typeof(T);
			if (GetNodeFor(type) != null)
				throw new InvalidOperationException("Cannot add an effect of the same type as an existing effect already being managed.");

			T effect = (T)Activator.CreateInstance(type);
			var effectInfo = new EffectInfo(effect, isLocalEffect);
			Add(effectInfo);

			return effect;
		}

		public void Remove<T>() where T : ScreenEffect
		{
			var type = typeof(T);
			var node = GetNodeFor(type);
			if (node != null)
				Remove(node);
		}

		public void RemoveAll()
		{
			while (_effects.Count > 0)
				Remove(_effects.First);
		}

		private void Add(EffectInfo effectInfo)
		{
			if (effectInfo == null)
				throw new ArgumentNullException("effectInfo");

			_effects.AddLast(effectInfo);
			effectInfo.Effect.OnAdd();

			if (effectInfo.IsLocal)
				++_numLocalEffects;
			else
				++_numGlobalEffects;
		}

		private void Remove(LinkedListNode<EffectInfo> node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			var effectInfo = node.Value;
			if (effectInfo.IsLocal)
				--_numLocalEffects;
			else
				--_numGlobalEffects;

			effectInfo.Effect.OnRemove();
			effectInfo.Effect.Dispose();
			_effects.Remove(node);
		}

		#endregion

		#region Events

		public void OnAppGainFocus()
		{
			for (var node = _effects.First; node != null; node = node.Next)
				node.Value.Effect.OnAppGainFocus();
		}

		public void OnAppLostFocus()
		{
			for (var node = _effects.First; node != null; node = node.Next)
				node.Value.Effect.OnAppLostFocus();
		}

		public void OnAppPause()
		{
			for (var node = _effects.First; node != null; node = node.Next)
				node.Value.Effect.OnAppPause();
		}

		public void OnAppResume()
		{
			for (var node = _effects.First; node != null; node = node.Next)
				node.Value.Effect.OnAppResume();
		}

		public void OnLostContext()
		{
			for (var node = _effects.First; node != null; node = node.Next)
				node.Value.Effect.OnLostContext();
		}

		public void OnNewContext()
		{
			for (var node = _effects.First; node != null; node = node.Next)
				node.Value.Effect.OnNewContext();
		}

		public void OnRenderLocal(float delta)
		{
			if (_numLocalEffects == 0)
				return;

			for (var node = _effects.First; node != null; node = node.Next)
			{
				if (node.Value.IsLocal)
					node.Value.Effect.OnRender(delta);
			}
		}

		public void OnRenderGlobal(float delta)
		{
			if (_numGlobalEffects == 0)
				return;

			for (var node = _effects.First; node != null; node = node.Next)
			{
				if (!node.Value.IsLocal)
					node.Value.Effect.OnRender(delta);
			}
		}

		public void OnResize()
		{
			for (var node = _effects.First; node != null; node = node.Next)
				node.Value.Effect.OnResize();
		}

		public void OnUpdate(float delta)
		{
			var node = _effects.First;
			while (node != null)
			{
				var effect = node.Value.Effect;
				if (!effect.IsActive)
				{
					var next = node.Next;
					Remove(node);
					node = next;
				}
				else
				{
					effect.OnUpdate(delta);
					node = node.Next;
				}
			}
		}

		#endregion

		#region Misc

		private LinkedListNode<EffectInfo> GetNodeFor(Type effectType)
		{
			if (effectType == null)
				throw new ArgumentNullException("effectType");

			for (var node = _effects.First; node != null; node = node.Next)
			{
				if (node.Value.Effect.GetType() == effectType)
					return node;
			}
			return null;
		}

		#endregion

		public void Dispose()
		{
			if (_effects == null)
				return;

			RemoveAll();
			_effects = null;
		}
	}
}
