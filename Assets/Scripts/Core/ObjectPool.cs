using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private static int MIN_OBJECT_COUNT = 10;
    private GameObject prefab;
    private List<GameObject> activeObjects = new List<GameObject>();
    private Stack<GameObject> pool = new Stack<GameObject>();

    public ObjectPool(GameObject prefab)
    {
        this.prefab = prefab;
        for (int i = 0; i < MIN_OBJECT_COUNT; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.SetActive(false);
            pool.Push(obj);
        }
    }
    public GameObject GetObject()
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Pop();
            obj.SetActive(true);
        }
        else // Expand if pool is empty
        {
            obj = GameObject.Instantiate(prefab);
        }
        activeObjects.Add(obj);
        return obj;
    }
    public void ReturnObject(GameObject obj)
    {
        if (activeObjects.Contains(obj))
        {
            activeObjects.Remove(obj);
            pool.Push(obj);
            obj.SetActive(false);
        } 
        else
        {
            Debug.LogError($"Trying to return an object \"{obj.name}\" that is not active in the pool: {this.prefab.name}");
        }
    }
}