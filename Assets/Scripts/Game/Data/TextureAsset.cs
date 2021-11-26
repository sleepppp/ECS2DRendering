using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.GameData
{
    [CreateAssetMenu(fileName = "TextureAsset.asset", menuName = "Project/ECS/TextureAsset", order = 2)]
    public class TextureAsset : ScriptableObject
    {
        public int ID;
        public Texture2D Texture;
    }
}