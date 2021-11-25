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
        //public EntityManager EntityManager;   //todo EntityManager�� ������ BurstComiler�� �ȸ���
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
        public Translation Translation;     //todo LocalToWorld�� ����
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
            int instanceCount = CharacterSpawn.Instance.InstanceCount;  //todo Entity�� �������ִ� �Ŵ�����Ʈ ������Ʈ�� ���� ��� �� �� �ְ� ó��
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
                block.SetTexture(_textureID, AnimationManager.Instance.GetTexture(0));//todo ������ �ϳ��� �ؽ�ó�θ� ó�������� �Ŀ� ���� �ؽ�ó ���� ��ġ �ؼ� ������

                Graphics.DrawMeshInstanced(mesh, 0, material, arrMatrix, instanceCount, block); //�ϵ���� �ν��Ͻ��� �ִ� 1023�� ������ ������
                Debug.Log("Render2DAnimationSystem draw Count : " + arrUV.Length);
            }
        }
    }
}
