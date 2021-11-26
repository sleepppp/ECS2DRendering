using Unity.Entities;

namespace Project.ECS
{
    public struct WorldComponent : IComponentData, IComponentSingleton
    {
        public int ObjectCount;
    }
}