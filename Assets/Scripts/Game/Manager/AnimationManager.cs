using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
namespace Project
{
    using Project.GameData;
    using Project.ECS;
    public class AnimationManager : MonoBehaviourSingleton<AnimationManager>
    {
        public List<AnimationClipAsset> ClipList;
        public List<Texture2D> TextureList;
     
        List<Entity> _animationEntityList = new List<Entity>();
        private void Awake()
        {
            GenerateAnimationEntities();
        }

        void GenerateAnimationEntities()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            EntityArchetype archetype = world.EntityManager.CreateArchetype(typeof(AnimationClip));

            for (int i = 0; i < ClipList.Count; ++i)
            {
                AnimationClipAsset asset = ClipList[i];
                Entity entity = world.EntityManager.CreateEntity(archetype);
                world.EntityManager.SetName(entity, asset.name);
                AnimationClip clip = new AnimationClip()
                {
                    TextureID = GetTextureID(asset),
                    FrameCount = asset.Sprites.Length,
                    FrameRateDelay = asset.FrameRateDelay,
                    IsLoop = asset.IsLoop
                };

                world.EntityManager.SetComponentData<AnimationClip>(entity, clip);
                DynamicBuffer<AnimationKeyFrame> keyframeBuffer = world.EntityManager.AddBuffer<AnimationKeyFrame>(entity);
                foreach(var sprite in asset.Sprites)
                {
                    AnimationKeyFrame keyframe = new AnimationKeyFrame()
                    {
                        CustomUV = new CustomUV() { Value = ConvertSpriteToUV(sprite).ToFloat4() }
                    };
                    keyframeBuffer.Add(keyframe);
                }

                _animationEntityList.Add(entity);
            }
        }

        public AnimationClipAsset GetAnimationClipAsset(int id)
        {
            return ClipList.Find((item) => { return item.ID == id; });
        }

        public Texture2D GetTexture(int id)
        {
            if (id >= TextureList.Count)
                return null;
            return TextureList[id];
        }

        public int GetTextureID(AnimationClipAsset clip)
        {
            Sprite sprite = clip.Sprites[0];
            for(int i =0; i < TextureList.Count; ++i)
            {
                if (TextureList[i] == sprite.texture)
                    return i;
            }
            return -1;
        }

        public Entity GetAnimtionEntity(int id)
        {
            if (id >= _animationEntityList.Count)
                return Entity.Null;
            return _animationEntityList[id];
        }

        public AnimationClip GetAnimClip(Entity entity)
        {
            return World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<AnimationClip>(entity);
        }

        public static Vector4 ConvertSpriteToUV(Sprite sprite)
        {
            Vector2[] uvs = sprite.uv;
            float width = Mathf.Abs(uvs[2].x - uvs[1].x);
            float height = Mathf.Abs(uvs[2].y - uvs[1].y);
            float offsetX = uvs[2].x;
            float offsetY = uvs[2].y;
            return new Vector4(width, height, offsetX, offsetY);
        }
    }
}
