using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private ItemsObjectPool[] objectPoolBases;

    private ItemsObjectPool poolToUse = null;
    private Dictionary<ItemType, ItemsObjectPool> objectPoolBasesDict = new Dictionary<ItemType, ItemsObjectPool>();

    public void Initialize()
    {

        InterfaceManager.Instance?.RegisterInterface<ObjectPoolManager>(this);
        foreach (var pool in objectPoolBases)
        {
            objectPoolBasesDict.Add(pool.GetPoolType(), pool);
            pool.InitPoolFirstTime();
        }
    }

    public ItemBase GetObjectFromPool(ItemType poolType)
    {
        ItemBase objectBase = null;

        poolToUse = GetUsedPool(poolType);
        if (poolToUse != null)
        {
            if (poolToUse.IsEmpty())
            {
                // enqueue new objects
                objectBase = poolToUse.EnqueueAndReturnItem();
            }
            else
            {
                objectBase = poolToUse.Dequeue();
            }
        }

        return objectBase;
    }

    public void PassObjectToPool(ItemType poolType, ItemBase objectBase)
    {
        poolToUse = GetUsedPool(poolType);
        if (poolToUse != null)
        {
            poolToUse.Enqueue(objectBase);
        }
    }
    
    public ItemsObjectPool GetUsedPool(ItemType poolType)
    {
        if (objectPoolBasesDict.ContainsKey(poolType))
            return objectPoolBasesDict[poolType];

        return null;
    }
}
