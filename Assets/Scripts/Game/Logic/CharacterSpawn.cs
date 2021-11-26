using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
namespace Project
{
    using Project.ECS;
    public class CharacterSpawn : MonoBehaviourSingleton<CharacterSpawn>
    {
        const float _minX = -10f;
        const float _maxX = 10f;
        const float _minY = -5f;
        const float _maxY = 5f;

        public int InstanceCount;
        private void Start()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            EntityArchetype archetype = world.EntityManager.CreateArchetype
                (
                    typeof(Translation),
                    typeof(Rotation),
                    typeof(Scale),
                    typeof(LocalToWorld),
                    typeof(Render2DComponent),
                    typeof(Render2DUVComponent),
                    typeof(AnimationUpdate)
                );

            for(int i = 0; i < InstanceCount; ++i)
            {
                Entity entity = world.EntityManager.CreateEntity(archetype);
                world.EntityManager.SetName(entity, "Character" + i);
                world.EntityManager.SetComponentData<Translation>(entity, new Translation()
                {
                    Value = GetRandomSpawnPosition()
                });
                world.EntityManager.SetComponentData<Scale>(entity, new Scale()
                {
                    Value = 1f
                });
                world.EntityManager.SetSharedComponentData<Render2DComponent>(entity, new Render2DComponent() 
                {
                    TextureID = 0   //todo 지금은 테스트용으로 0번 텍스쳐를 사용합니다
                });
                world.EntityManager.SetComponentData<Render2DUVComponent>(entity, new Render2DUVComponent() 
                {
                    CustomUV = new CustomUV() { Value = AnimationManager.Instance.GetAnimationClipAsset(0).GetCustomUV(0) }
                });
                world.EntityManager.SetComponentData<AnimationUpdate>(entity, new AnimationUpdate()
                {
                    CurrentTimer = UnityEngine.Random.Range(0f,0.3f),
                    CurrentFrame = UnityEngine.Random.Range(0,2),
                    ClipEntity = AnimationManager.Instance.GetAnimtionEntity(0),
                    Clip = AnimationManager.Instance.GetAnimClip(AnimationManager.Instance.GetAnimtionEntity(0))
                }) ;
            }

            EntitySingleton<WorldComponent>.SetComponentData(new WorldComponent() { ObjectCount = InstanceCount });
        }

        float3 GetRandomSpawnPosition()
        {
            float x = UnityEngine.Random.Range(_minX, _maxX);
            float y = UnityEngine.Random.Range(_minY, _maxY);
            float z = 0f;
            return new float3(x, y, z);
        }
    }
}