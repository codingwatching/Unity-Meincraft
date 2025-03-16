using System.Collections.Generic;
using UnityEngine;

public class ChunkPooling : MonoBehaviour
{
    [SerializeField] GameObject chunkPrefab;
    private Queue<GameObject> pool;
    public int PoolSize => pool.Count;

    public void InitializePool(int count)
    {
        pool = new Queue<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject poolObject = Instantiate(chunkPrefab);
            poolObject.SetActive(false);
            pool.Enqueue(poolObject);
        }
    }

    public GameObject GetObjectFromPool()
    {
        if (pool.Count > 0)
        {
            GameObject poolObject = pool.Dequeue();
            poolObject.SetActive(true);
            return poolObject;
        }

        GameObject obj = Instantiate(chunkPrefab);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
