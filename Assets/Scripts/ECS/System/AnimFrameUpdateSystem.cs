using Unity.Entities;
namespace Project.ECS
{
    public class AnimFrameUpdateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.WithName("AnimFrameUpdate").ForEach((ref AnimationUpdate animUpdate, ref Render2DAnimationComponent renderAnimation) =>
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
