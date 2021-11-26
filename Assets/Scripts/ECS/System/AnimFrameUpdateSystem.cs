using Unity.Entities;
using Unity.Collections;
using System.Collections.Generic;
namespace Project.ECS
{
    using Project.GameData;
    public class AnimFrameUpdateSystem : SystemBase
    {
        //NativeHashMap<int, NativeArray<AnimationKeyFrame>> _keyframeContainer = new NativeHashMap<int, NativeArray<AnimationKeyFrame>>();
        //NativeHashMap<int, AnimationClip> _clipContainer = new NativeHashMap<int, AnimationClip>();

        protected override void OnCreate()
        {
            base.OnCreate();

            //List<AnimationClipAsset> assetList = AnimationManager.Instance.ClipList;
            //foreach(var asset in assetList)
            //{
            //    AnimationClip clip = new AnimationClip()
            //    {
            //        TextureID = AnimationManager.Instance.GetTextureID(asset),
            //        IsLoop = asset.IsLoop,
            //        FrameCount = asset.Sprites.Length,
            //        FrameRateDelay = asset.FrameRateDelay
            //    };
            //    _clipContainer.Add(asset.ID, clip);
            //
            //    NativeArray<AnimationKeyFrame> keyFrames = new NativeArray<AnimationKeyFrame>(asset.Sprites.Length, Allocator.Persistent);
            //    for(int i =0; i < asset.Sprites.Length; ++i)
            //    {
            //        keyFrames[i] = new AnimationKeyFrame()
            //        {
            //            CustomUV = new CustomUV() { Value = AnimationManager.ConvertSpriteToUV(asset.Sprites[i]) }
            //        };
            //        _keyframeContainer.Add(asset.ID, keyFrames);
            //    }
            //}
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.WithName("AnimFrameUpdate").WithBurst().ForEach((ref AnimationUpdate animUpdate, ref Render2DUVComponent renderAnimation) =>
            {
                animUpdate.CurrentTimer += deltaTime;
                if(animUpdate.CurrentTimer >= animUpdate.Clip.FrameRateDelay)
                {
                    int prevFrame = animUpdate.CurrentFrame;
                    if (animUpdate.Clip.IsLoop)
                        animUpdate.CurrentFrame = (animUpdate.CurrentFrame + 1) % animUpdate.Clip.FrameCount;
                    else
                    {
                        animUpdate.CurrentFrame++;
                        if (animUpdate.CurrentFrame >= animUpdate.Clip.FrameCount)
                            animUpdate.CurrentFrame = animUpdate.Clip.FrameCount - 1;
                    }
                    if(prevFrame != animUpdate.CurrentFrame)
                    {
                        //change Frame
                        DynamicBuffer<AnimationKeyFrame> buffer = GetBuffer<AnimationKeyFrame>(animUpdate.ClipEntity);
                        AnimationKeyFrame newFrame = buffer[animUpdate.CurrentFrame];
                        renderAnimation.CustomUV = newFrame.CustomUV;
                    }
                    animUpdate.CurrentTimer = 0f;   //todo while remove time
                }

            }).Run();
        }
    }
}
