using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Project.GameData
{
    [CreateAssetMenu(fileName = "AnimationClipAsset.asset", menuName = "Project/ECS/AnimationClip", order = 2)]
    public class AnimationClipAsset : ScriptableObject
    {
        public int ID;
        public float FrameRateDelay;
        public bool IsLoop;
        public Sprite[] Sprites;

        public float4 GetCustomUV(int index)
        {
            return AnimationManager.ConvertSpriteToUV(Sprites[index]);
        }
    }
}
