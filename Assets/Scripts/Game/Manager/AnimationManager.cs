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
        public List<TextureAsset> TextureList;

        Dictionary<int, AnimationClipAsset> _clipContainer = new Dictionary<int, AnimationClipAsset>();
        Dictionary<int, Entity> _entityContainer = new Dictionary<int, Entity>();
        Dictionary<int, Texture2D> _textureContainer = new Dictionary<int, Texture2D>();
        private void Awake()
        {
            foreach (var item in ClipList)
                _clipContainer.Add(item.ID, item);
            foreach (var item in TextureList)
                _textureContainer.Add(item.ID, item.Texture);
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
                int textureID = GetTextureID(asset);
                world.EntityManager.SetName(entity, asset.name);
                AnimationClip clip = new AnimationClip()
                {
                    TextureID = textureID,
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

                _entityContainer.Add(asset.ID,entity);
            }
        }

        public AnimationClipAsset GetAnimationClipAsset(int id)
        {
            AnimationClipAsset result = null;
            _clipContainer.TryGetValue(id, out result);
            return result;
        }

        public Texture2D GetTexture(int id)
        {
            Texture2D result = null;
            _textureContainer.TryGetValue(id, out result);
            return result;
        }

        public int GetTextureID(AnimationClipAsset asset)
        {
            foreach(var item in _textureContainer)
            {
                if (item.Value == asset.Sprites[0].texture)
                    return item.Key;
            }
            return 0;
        }

        public Entity GetAnimtionEntity(int id)
        {
            Entity result = Entity.Null;
            _entityContainer.TryGetValue(id, out result);
            return result;
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
