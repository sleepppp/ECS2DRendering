using UnityEngine;
using Unity.Entities;

namespace Project.ECS
{
    public interface IComponentSingleton
    {

    }

    public class EntitySingleton<Component> : Singleton<EntitySingleton<Component>> where Component : struct, IComponentData, IComponentSingleton
    {
        public Entity Entity;

        World _world;

        public EntitySingleton()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            Entity = _world.EntityManager.CreateEntity(typeof(Component));
        }

        public static Component GetComponentData()
        {
            return Instance._world.EntityManager.GetComponentData<Component>(Instance.Entity);
        }

        public static void SetComponentData(Component component)
        {
            Instance._world.EntityManager.SetComponentData<Component>(Instance.Entity, component);
        }
    }
}
