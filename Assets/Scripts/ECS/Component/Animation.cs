using Unity.Entities;
using Unity.Mathematics;

namespace Project.ECS
{
    public struct AnimationKeyFrame : IBufferElementData
    {
        public CustomUV CustomUV;
    }

    public struct AnimationClip : IComponentData
    {
        public int TextureID;
        public int FrameCount;
        public float FrameRateDelay;
        public bool IsLoop;
    }

    public struct AnimationUpdate : IComponentData
    {
        public float CurrentTimer;
        public int CurrentFrame;
        public Entity ClipEntity;
        public AnimationClip Clip;
    }
}

