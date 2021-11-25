using Unity.Mathematics;
using UnityEngine;

namespace Project
{
    public static class MathExternal
    {
        public static float4 ToFloat4(this Vector4 v)
        {
            return new float4(v.x, v.y, v.z, v.w);
        }

        public static Vector4 ToVector4(this float4 f)
        {
            return new Vector4(f.x, f.y, f.z, f.w);
        }

        public static Vector3 ToVector3(this float3 f)
        {
            return new Vector3(f.x, f.y, f.z);
        }
    }
}
