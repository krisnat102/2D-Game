using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject temporary;
        for(int i = 0; i < amountToPool; i++)
        {
            temporary = Instantiate(objectToPool);
            temporary.SetActive(false);
            pooledObjects.Add(temporary);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
