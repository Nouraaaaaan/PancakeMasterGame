using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public Dictionary<string, Queue<GameObject>> PoolDirectory;
    public List<Pool> Pools;

    # region Singelton
    public static ObjectPooler Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        PoolDirectory = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in Pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject obj = Instantiate(pool.Prefab,gameObject.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            PoolDirectory.Add(pool.Tag, objectPool);
        }

    }
    #endregion

    public GameObject SpwanFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!PoolDirectory.ContainsKey(tag))
            return null;

        if (PoolDirectory[tag].Count == 0)
            return null;

        GameObject objectToSpawn = PoolDirectory[tag].Dequeue();

        if (objectToSpawn.activeInHierarchy == true)
            return null;

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        PoolDirectory[tag].Enqueue(objectToSpawn);

        if (objectToSpawn.activeInHierarchy == true)
            return null;

        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

}
