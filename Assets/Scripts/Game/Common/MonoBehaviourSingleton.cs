using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T _instace;
        public static T Instance
        {
            get
            {
                if (_instace == null)
                {
                    T[] arr = FindObjectsOfType<T>();
                    if (arr == null || arr.Length == 0)
                    {
                        GameObject newObject = new GameObject(typeof(T).Name);
                        _instace = newObject.AddComponent<T>();
                    }
                    else if (arr.Length == 1)
                    {
                        _instace = arr[0];
                    }
                    else
                    {
                        throw new System.Exception("Too many Instance");
                    }

                    DontDestroyOnLoad(_instace.gameObject);
                }
                return _instace;
            }
        }
    }

}