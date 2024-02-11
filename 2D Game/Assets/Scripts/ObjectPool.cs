using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using JetBrains.Annotations;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    public ObjectPool<GameObject> pool;

    public ObjectPool(GameObject gameObject, Vector3 position, Quaternion rotation, int defaultCapacity, int maxCapacity)
    {
        GameObject = gameObject;
        Position = position;
        Rotation = rotation;
        DefaultCapacity = defaultCapacity;
        MaxCapacity = maxCapacity;
    }

    public GameObject GameObject { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public int DefaultCapacity {  get; private set; }
    public int MaxCapacity { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pool = new ObjectPool<GameObject>(CreateObject, OnTakeObjectFromPool, OnReturnGameObjectToPool, OnDestroyGameObject, true, DefaultCapacity, MaxCapacity);
    }

    private GameObject CreateObject()
    {
        GameObject gameObject = Instantiate(GameObject, Position, Rotation);

        return gameObject;
    }

    private void OnTakeObjectFromPool(GameObject gameObject)
    {
        gameObject.transform.position = Position;
        gameObject.transform.rotation = Rotation;

        gameObject.SetActive(true);
    }

    private void OnReturnGameObjectToPool(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    private void OnDestroyGameObject(GameObject gameObject)
    { 
        Destroy(gameObject);
    }
}
