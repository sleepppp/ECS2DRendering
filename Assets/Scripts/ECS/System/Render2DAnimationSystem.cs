using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
namespace Project.ECS
{
    [BurstCompile]
    struct CollectRenderingDataJob : IJobEntityBatchWithIndex
    {
        [WriteOnly]public NativeArray<Vector4> ArrRenderingData;
        [WriteOnly]public NativeArray<Matrix4x4> ArrMatrixData;

        public SharedComponentTypeHandle<Render2DComponent> Render2DTypeHandle;
        [ReadOnly] public ComponentTypeHandle<Translation> TranslationTypeHandle;
        [ReadOnly] public ComponentTypeHandle<Render2DUVComponent> Render2DAnimationTypeHandle;

        public void Execute(ArchetypeChunk batchInChunk, int batchIndex, int indexOfFirstEntityInQuery)
        {
            NativeArray<Translation> arrTranslation = batchInChunk.GetNativeArray(TranslationTypeHandle);
            NativeArray<Render2DUVComponent> arrAnimation = batchInChunk.GetNativeArray(Render2DAnimationTypeHandle);
            for (int i = 0; i < batchInChunk.Count; ++i)
            {
                int resultIndex = indexOfFirstEntityInQuery + i;
                ArrMatrixData[resultIndex] = Matrix4x4.Translate(arrTranslation[i].Value.ToVector3());
                ArrRenderingData[resultIndex] = arrAnimation[i].CustomUV.Value.ToVector4();
            }
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class Render2DAnimationSystem : SystemBase
    {
        static readonly int _textureID = Shader.PropertyToID("_MainTex");
        static readonly int _uvID = Shader.PropertyToID("_UV");
        const int _maxInstanceDrawCount = 1023;

        public EntityQuery EitityQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            EitityQuery = World.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Render2DUVComponent>(),
                ComponentType.ReadOnly<Render2DComponent>());
        }
        protected override void OnUpdate()
        {
            int instanceCount = EntitySingleton<WorldComponent>.GetComponentData().ObjectCount;

            ComponentTypeHandle<Translation> translationTypeHandle = GetComponentTypeHandle<Translation>(true);
            ComponentTypeHandle<Render2DUVComponent> animationTypeHandle = GetComponentTypeHandle<Render2DUVComponent>(true);
            SharedComponentTypeHandle<Render2DComponent> rendererTypeHandle = GetSharedComponentTypeHandle<Render2DComponent>();

            using(NativeArray<Vector4> arrRenderingData = new NativeArray<Vector4>(instanceCount, Allocator.TempJob))
            using(NativeArray<Matrix4x4> arrMatrixData = new NativeArray<Matrix4x4>(instanceCount,Allocator.TempJob))
            {
                CollectRenderingDataJob job = new CollectRenderingDataJob()
                {
                    ArrRenderingData = arrRenderingData,
                    ArrMatrixData = arrMatrixData,
                    Render2DTypeHandle = rendererTypeHandle,
                    Render2DAnimationTypeHandle = animationTypeHandle,
                    TranslationTypeHandle = translationTypeHandle
                };

                Dependency = job.Schedule(EitityQuery);
                Dependency.Complete();  //wait jobComplete

                Material material = RenderingManager.Instance.RenderingMaterial;
                Mesh mesh = RenderingManager.Instance.RenderingMesh;

                MaterialPropertyBlock block = new MaterialPropertyBlock();
                Matrix4x4[] arrMatrix = new Matrix4x4[_maxInstanceDrawCount];
                Vector4[] arrUV = new Vector4[_maxInstanceDrawCount];
                int drawCall = 0;

                block.SetTexture(_textureID, AnimationManager.Instance.GetTexture(0));//todo ������ �ϳ��� �ؽ�ó�θ� ó�������� �Ŀ� ���� �ؽ�ó ���� ��ġ �ؼ� ������

                for (int i = 0; i < instanceCount; i += _maxInstanceDrawCount)
                {
                    int sliceSize = math.min(_maxInstanceDrawCount, instanceCount - i);
                    NativeArray<Vector4>.Copy(arrRenderingData, i, arrUV,0, sliceSize);
                    NativeArray<Matrix4x4>.Copy(arrMatrixData, i, arrMatrix, 0, sliceSize);
                    block.SetVectorArray(_uvID, arrUV);
                    Graphics.DrawMeshInstanced(mesh, 0, material, arrMatrix, sliceSize, block); //�ϵ���� �ν��Ͻ��� �ִ� 1023�� ������ ������
            
                    ++drawCall;
                }

                //Debug.Log("Render2DAnimationSystem Drawcall : " + drawCall);
            }
        }
    }
}
