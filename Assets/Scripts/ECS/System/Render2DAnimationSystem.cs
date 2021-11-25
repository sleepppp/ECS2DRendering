using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
namespace Project.ECS
{
    [BurstCompile]
    struct CollectRenderingDataJob : IJobEntityBatch
    {
        public NativeArray<RenderingData> ArrRenderingData;
        //public EntityManager EntityManager;   //todo EntityManager가 있으면 BurstComiler가 안먹힘
        public SharedComponentTypeHandle<Render2DComponent> Render2DTypeHandle;
        [ReadOnly] public ComponentTypeHandle<Translation> TranslationTypeHandle;
        [ReadOnly] public ComponentTypeHandle<Render2DAnimationComponent> Render2DAnimationTypeHandle;

        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            //Render2DComponent render2DComponent = GetSharedRender2D(batchInChunk);
            NativeArray<Translation> arrTranslation = batchInChunk.GetNativeArray(TranslationTypeHandle);
            NativeArray<Render2DAnimationComponent> arrAnimation = batchInChunk.GetNativeArray(Render2DAnimationTypeHandle);
            RenderingData output = new RenderingData();
            //output.Render2D = render2DComponent;
            for(int i =0; i < batchInChunk.Count; ++i)
            {
                output.Translation = arrTranslation[i];
                output.UV = arrAnimation[i].CustomUV.Value.ToVector4();

                int resultIndex = batchIndex + i;

                ArrRenderingData[resultIndex] = output;
            }
        }

        //[BurstDiscard]
        //Render2DComponent GetSharedRender2D(ArchetypeChunk chunk)
        //{
        //    return chunk.GetSharedComponentData(Render2DTypeHandle, EntityManager);
        //}
    }

    [System.Serializable]
    struct RenderingData
    {
        public Translation Translation;     //todo LocalToWorld로 변경
        public Render2DComponent Render2D;
        public Vector4 UV;
    }

    public class Render2DAnimationSystem : SystemBase
    {
        static readonly int _textureID = Shader.PropertyToID("_MainTex");
        static readonly int _uvID = Shader.PropertyToID("_UV");

        EntityQuery _entityQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityQuery = World.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Render2DAnimationComponent>(),
                ComponentType.ReadOnly<Render2DComponent>());
        }
        protected override void OnUpdate()
        {
            int instanceCount = CharacterSpawn.Instance.InstanceCount;  //todo Entity들 관리해주는 매니지먼트 컴포넌트로 부터 얻어 올 수 있게 처리
            ComponentTypeHandle<Translation> translationTypeHandle = GetComponentTypeHandle<Translation>(true);
            ComponentTypeHandle<Render2DAnimationComponent> animationTypeHandle = GetComponentTypeHandle<Render2DAnimationComponent>(true);
            SharedComponentTypeHandle<Render2DComponent> rendererTypeHandle = GetSharedComponentTypeHandle<Render2DComponent>();

            using (NativeArray<RenderingData> arrRenderingData = new NativeArray<RenderingData>(instanceCount, Allocator.TempJob))
            {
                CollectRenderingDataJob job = new CollectRenderingDataJob()
                {
                    ArrRenderingData = arrRenderingData,
                    //EntityManager = EntityManager,
                    Render2DTypeHandle = rendererTypeHandle,
                    Render2DAnimationTypeHandle = animationTypeHandle,
                    TranslationTypeHandle = translationTypeHandle
                };

                Dependency = job.Schedule(_entityQuery);
                Dependency.Complete();  //wait jobComplete

                Material material = RenderingManager.Instance.RenderingMaterial;
                Mesh mesh = RenderingManager.Instance.RenderingMesh;

                MaterialPropertyBlock block = new MaterialPropertyBlock();

                Matrix4x4[] arrMatrix = new Matrix4x4[instanceCount];
                Vector4[] arrUV = new Vector4[instanceCount];
                for (int i = 0; i < instanceCount; ++i)
                {
                    arrMatrix[i] = Matrix4x4.Translate(arrRenderingData[i].Translation.Value);
                    arrUV[i] = arrRenderingData[i].UV;
                }

                block.SetVectorArray(_uvID, arrUV);
                block.SetTexture(_textureID, AnimationManager.Instance.GetTexture(0));//todo 지금은 하나의 텍스처로만 처리하지만 후에 같은 텍스처 끼리 배치 해서 렌더링

                Graphics.DrawMeshInstanced(mesh, 0, material, arrMatrix, instanceCount, block); //하드웨어 인스턴싱은 최대 1023개 까지만 지원함
                Debug.Log("Render2DAnimationSystem draw Count : " + arrUV.Length);
            }
        }
    }
}
