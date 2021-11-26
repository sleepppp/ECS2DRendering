using Unity.Entities;
using Unity.Mathematics;
namespace Project.ECS
{ 
    public struct Render2DComponent : ISharedComponentData
    {
        public int TextureID;
    }

    public struct Render2DUVComponent : IComponentData
    {
        public CustomUV CustomUV;
    }

    public struct CustomUV
    {
        public float4 Value;

        public static CustomUV Default = new CustomUV() { Value = new float4(1f,1f,0f,0f) };
    }
}