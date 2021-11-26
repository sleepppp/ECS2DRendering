using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMono : MonoBehaviour
{
    const float _minX = -10f;
    const float _maxX = 10f;
    const float _minY = -5f;
    const float _maxY = 5f;

    public GameObject Origin;
    public int InstanceCount;

    private void Start()
    {
        for(int i = 0; i < InstanceCount; ++i)
        {
            Instantiate(Origin, GetRandomSpawnPosition(), Quaternion.identity, transform);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float x = UnityEngine.Random.Range(_minX, _maxX);
        float y = UnityEngine.Random.Range(_minY, _maxY);
        float z = 0f;
        return new Vector3(x, y, z);
    }
}
