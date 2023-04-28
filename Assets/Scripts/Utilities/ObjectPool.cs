using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    //public static ObjectPool SharedInstance;
    public GameObject objectToPool;
    public int amountToPool;
    
    public List<GameObject> pooledObjects;

    void Start()
    {
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < amountToPool; i++)
        {
            GameObject tmp = Instantiate(objectToPool, transform);
            tmp.SetActive(false);

            pooledObjects.Add(tmp);
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

        GameObject tmp = Instantiate(objectToPool, transform);
        tmp.SetActive(false);
        pooledObjects.Add(tmp);

        return tmp;
    }
}
