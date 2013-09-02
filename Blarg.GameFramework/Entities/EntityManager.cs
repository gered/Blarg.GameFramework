using System;
using System.Collections.Generic;
using System.Reflection;
using Blarg.GameFramework.Entities.SystemComponents;
using Blarg.GameFramework.Events;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.Entities
{
	using EntityList = List<Entity>;
	using ComponentList = List<Component>;
	using EntityToComponentMap = Dictionary<Entity, Component>;
	using ComponentStore = Dictionary<Type, Dictionary<Entity, Component>>;
	using GlobalComponentStore = Dictionary<Type, Component>;
	using ComponentSystemList = List<ComponentSystem>;
	using EntityPresetMap = Dictionary<Type, EntityPreset>;

	public class EntityManager : IDisposable
	{
		public readonly EventManager EventManager;

		EntityList _entities;
		ComponentStore _components;
		GlobalComponentStore _globalComponents;
		ComponentSystemList _componentSystems;
		EntityPresetMap _presets;

		EntityPool _entityPool;

		EntityList _tempEntityList;

		public EntityManager(EventManager eventManager)
		{
			if (eventManager == null)
				throw new ArgumentNullException("eventManager");

			EventManager = eventManager;
			_entities = new EntityList();
			_components = new ComponentStore();
			_globalComponents = new GlobalComponentStore();
			_componentSystems = new ComponentSystemList();
			_presets = new EntityPresetMap();

			_entityPool = new EntityPool(this);

			_tempEntityList = new EntityList(100);
		}

		#region ComponentSystem management

		public T AddSubsystem<T>() where T : ComponentSystem
		{
			if (GetSubsystem<T>() != null)
				throw new InvalidOperationException("ComponentSystem of that type is already registered.");

			T subsystem = (T)Activator.CreateInstance(typeof(T), this, EventManager);
			_componentSystems.Add(subsystem);
			return subsystem;
		}

		public T GetSubsystem<T>() where T : ComponentSystem
		{
			int i = GetSubsystemIndex(typeof(T));
			if (i == -1)
				return null;
			else
				return (T)_componentSystems[i];
		}

		public void RemoveSubsystem<T>() where T : ComponentSystem
		{
			int i = GetSubsystemIndex(typeof(T));
			if (i == -1)
				return;
			else
				_componentSystems.RemoveAt(i);
		}

		public void RemoveAllSubsystems()
		{
			for (int i = 0; i < _componentSystems.Count; ++i)
				_componentSystems[i].Dispose();
			_componentSystems.Clear();
		}

		#endregion

		#region Preset management

		public void AddPreset<T>() where T : EntityPreset
		{
			if (_presets.ContainsKey(typeof(T)))
			    throw new InvalidOperationException("EntityPreset of that type is already registered.");

			T preset = (T)Activator.CreateInstance(typeof(T), this);
			_presets.Add(typeof(T), preset);
		}

		public void RemovePreset<T>() where T : EntityPreset
		{
			_presets.Remove(typeof(T));
		}

		public void RemoveAllPresets()
		{
			_presets.Clear();
		}

		#endregion

		#region Entity management

		public Entity Add()
		{
			var entity = _entityPool.Take();
			_entities.Add(entity);
			return entity;
		}

		public Entity AddUsingPreset<T>() where T : EntityPreset
		{
			return AddUsingPreset(typeof(T), new EntityPreset.EmptyEntityPresetArgs());
		}

		public Entity AddUsingPreset<T>(EntityPresetArgs args) where T : EntityPreset
		{
			return AddUsingPreset(typeof(T), args);
		}

		public Entity AddUsingPreset(Type presetType)
		{
			return AddUsingPreset(presetType, new EntityPreset.EmptyEntityPresetArgs());
		}

		public Entity AddUsingPreset(Type presetType, EntityPresetArgs args)
		{
			if (presetType == null)
				throw new ArgumentNullException("presetType");
			if (!typeof(EntityPreset).IsAssignableFrom(presetType))
				throw new ArgumentException("Type specified is not an EntityPreset sub type.");

			var preset = _presets.Get(presetType);
			if (preset == null)
				throw new InvalidOperationException("Cannot add entity using a non-existant preset.");

			var entity = preset.Create(args);
			entity.Add<EntityPresetComponent>().PresetType = presetType;

			return entity;
		}

		public Entity GetFirstWith<T>() where T : Component
		{
			EntityToComponentMap componentEntities = _components.Get(typeof(T));
			if (componentEntities == null)
				return null;

			var enumerator = componentEntities.Keys.GetEnumerator();
			enumerator.MoveNext();
			return enumerator.Current;
		}

		public void GetAllWith<T>(EntityList list, bool clearListFirst = true) where T : Component
		{
			if (list == null)
				throw new ArgumentNullException("list", "Must provide a list to store matching Entity's in.");

			if (clearListFirst)
				list.Clear();

			EntityToComponentMap componentEntities = _components.Get(typeof(T));
			if (componentEntities == null)
				return;
			else
			{
				foreach (var i in componentEntities)
					list.Add(i.Key);
			}
		}

		public void Remove(Entity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			RemoveAllComponentsFrom(entity);
			_entities.Remove(entity);
			_entityPool.Free(entity);
		}

		public void RemoveAll()
		{
			foreach (var entity in _entities)
			{
				RemoveAllComponentsFrom(entity);
				_entityPool.Free(entity);
			}
			_entities.Clear();
		}

		#endregion

		#region Component management

		public T AddComponent<T>(Entity entity) where T : Component
		{
			if (GetComponent<T>(entity) != null)
				throw new InvalidOperationException("Component of that type has been added to this entity already.");

			EntityToComponentMap componentEntities = _components.Get(typeof(T));
			if (componentEntities == null)
			{
				// need to create the component-to-entity list
				componentEntities = new EntityToComponentMap();
				_components.Add(typeof(T), componentEntities);
			}

			T component = ObjectPools.Take<T>();

			componentEntities.Add(entity, component);
			return component;
		}

		public T GetComponent<T>(Entity entity) where T : Component
		{
			EntityToComponentMap componentEntities = _components.Get(typeof(T));
			if (componentEntities == null)
				return null;

			Component existing = componentEntities.Get(entity);
			if (existing == null)
				return null;
			else
				return (T)existing;
		}

		public void RemoveComponent<T>(Entity entity) where T : Component
		{
			EntityToComponentMap componentEntities = _components.Get(typeof(T));
			if (componentEntities == null)
				return;

			Component existing = componentEntities.Get(entity);
			if (existing == null)
				throw new InvalidOperationException("Entity does not have this component.");
			componentEntities.Remove(entity);
			ObjectPools.Free<T>((T)existing);
		}

		public bool HasComponent<T>(Entity entity) where T : Component
		{
			EntityToComponentMap componentEntities = _components.Get(typeof(T));
			if (componentEntities == null)
				return false;

			return componentEntities.ContainsKey(entity);
		}

		public void GetAllComponentsFor(Entity entity, ComponentList list)
		{
			if (list == null)
				throw new ArgumentNullException("Must provide a list to store the Components for this Entity.");

			foreach (var i in _components)
			{
				EntityToComponentMap entitiesWithComponent = i.Value;
				Component component = entitiesWithComponent.Get(entity);
				if (component != null)
					list.Add(component);
			}
		}

		#endregion

		#region Global Component management

		public T AddGlobalComponent<T>() where T : Component
		{
			if (GetGlobalComponent<T>() != null)
				throw new InvalidOperationException("Global component of that type has been added already.");

			T component = ObjectPools.Take<T>();

			_globalComponents.Add(typeof(T), component);
			return component;
		}

		public T GetGlobalComponent<T>() where T : Component
		{
			Component existing = _globalComponents.Get(typeof(T));
			if (existing == null)
				return null;
			else
				return (T)existing;
		}

		public void RemoveGlobalComponent<T>() where T : Component
		{
			Component existing = _globalComponents.Get(typeof(T));
			if (existing == null)
				throw new InvalidOperationException("No global component of that type exists.");
			else
			{
				_globalComponents.Remove(typeof(T));
				ObjectPools.Free<T>((T)existing);
			}
		}

		public bool HasGlobalComponent<T>() where T : Component
		{
			return _globalComponents.ContainsKey(typeof(T));
		}

		public void RemoveAllGlobalComponents()
		{
			foreach (var i in _globalComponents)
				ObjectPools.Free(i.Key, i.Value);
		}

		#endregion

		#region Events

		public void OnAppResume()
		{
			for (int i = 0; i < _componentSystems.Count; ++i)
				_componentSystems[i].OnAppResume();
		}

		public void OnAppPause()
		{
			for (int i = 0; i < _componentSystems.Count; ++i)
				_componentSystems[i].OnAppPause();
		}

		public void OnResize()
		{
			for (int i = 0; i < _componentSystems.Count; ++i)
				_componentSystems[i].OnResize();
		}

		public void OnRender(float delta)
		{
			for (int i = 0; i < _componentSystems.Count; ++i)
				_componentSystems[i].OnRender(delta);
		}

		public void OnUpdate(float delta)
		{
			_tempEntityList.Clear();
			GetAllWith<InactiveComponent>(_tempEntityList);
			foreach (var entity in  _tempEntityList)
				Remove(entity);

			for (int i = 0; i < _componentSystems.Count; ++i)
				_componentSystems[i].OnUpdate(delta);
		}

		#endregion

		#region Misc support methods

		private void RemoveAllComponentsFrom(Entity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			foreach (var i in _components)
			{
				var entitiesWithComponent = i.Value;
				Component component = entitiesWithComponent.Get(entity);
				if (component != null)
				{
					ObjectPools.Free(i.Key, component);
					entitiesWithComponent.Remove(entity);
				}
			}
		}

		private int GetSubsystemIndex(Type type)
		{
			for (int i = 0; i < _componentSystems.Count; ++i)
			{
				if (_componentSystems[i].GetType() == type)
					return i;
			}
			return -1;
		}

		#endregion

		#region Disposable

		public void Dispose()
		{
			RemoveAll();
			RemoveAllGlobalComponents();
			RemoveAllSubsystems();
		}

		#endregion
	}
}

