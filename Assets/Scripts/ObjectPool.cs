using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolingClass
{
    public string name;
    public GameObject prefab;
    public int startingAmount;
}


public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public PoolingClass[] poolingClasses;

    private List<GameObject>[] pooledObjects;


    // Awake is called immediately when the object becomes active
    void Awake()
    {
        pooledObjects = new List<GameObject>[poolingClasses.Length];
        for (int i = 0; i < poolingClasses.Length; i++)
        {
            List<GameObject> objectList = new List<GameObject>();
            for (int j = 0; j < poolingClasses[i].startingAmount; j++) {
                GameObject newObject = Instantiate(poolingClasses[i].prefab);
                objectList.Add(newObject);
                newObject.SetActive(false);
            }
            pooledObjects[i] = objectList;
        }
    }

    public GameObject GetReadyObject(string name)
    {
        int indexOfList = GetListIndexFromName(name);
        if (indexOfList == -1)
            throw new System.Exception("Object pool with name '" + name + "' does not exist");
        
        List<GameObject> objectList = pooledObjects[indexOfList];
        
        // Find and return an inactive (free) object, if possible
        foreach (GameObject obj in objectList)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }

        // All of the objects were active, so a new one is created
        GameObject newObject = Instantiate(poolingClasses[indexOfList].prefab);
        objectList.Add(newObject);
        newObject.SetActive(false);
        return newObject;
    }

    public int GetListIndexFromName(string name)
    {
        for (int i = 0; i < poolingClasses.Length; i++)
        {
            if (poolingClasses[i].name == name)
            {
                return i;
            }
        }
        return -1;
    }
}
