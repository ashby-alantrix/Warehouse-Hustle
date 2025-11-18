using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public enum PoolType
{
    Item = 0,
    Truck = 1
}

public class ObjectPoolManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private ItemsObjectPool[] itemPoolBases;
    [SerializeField] private TruckObjectPool[] truckPoolBases;

    private ObjectPoolBase<ObjectBase> poolToUse = null;
    // private ItemsObjectPool poolToUse = null;

    private Dictionary<ItemType, ItemsObjectPool> itemsPoolBasesDict = new Dictionary<ItemType, ItemsObjectPool>();
    private Dictionary<TruckType, TruckObjectPool> truckPoolBasesDict = new Dictionary<TruckType, TruckObjectPool>();
    // private Dictionary<ItemType, ObjectPoolBase<ItemBase>> itemsPoolBasesDict = new Dictionary<ItemType, ObjectPoolBase<ItemBase>>();
    // private Dictionary<TruckType, ObjectPoolBase<TruckBase>> truckPoolBasesDict = new Dictionary<TruckType, ObjectPoolBase<TruckBase>>();

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<ObjectPoolManager>(this);
        foreach (var pool in itemPoolBases)
        {
            itemsPoolBasesDict.Add(pool.GetPoolType(), pool);
            pool.InitPoolFirstTime();
        }

        foreach (var pool in truckPoolBases)
        {
            truckPoolBasesDict.Add(pool.GetPoolType(), pool);
            pool.InitPoolFirstTime();
        }
    }
 
    public T GetObjectFromPool<T>(string poolItemTypeInfo, PoolType poolType) where T : ObjectBase //ItemType poolItemType) where T : ObjectBase
    {
        T objectBase = null;
        ObjectPoolBase poolToUse = GetUsedPool(GetPoolInfoBasedOnType(poolItemTypeInfo, poolType), poolType);

        if (poolToUse != null)
        {
            if (poolToUse.IsEmpty())
            {
                Debug.Log($"Object Pool: CreateNewPooledItem");
                objectBase = (T)poolToUse.CreateNewPooledItem();
            }
            else
            {
                Debug.Log($"Object Pool: Use existing item");
                objectBase = (T)poolToUse.Dequeue();
            }
        }

        return objectBase;
    }
    
    public void PassObjectToPool<T>(string poolItemTypeInfo, PoolType poolType, T objectBase) where T : ObjectBase
    {
        ObjectPoolBase poolToUse = GetUsedPool(GetPoolInfoBasedOnType(poolItemTypeInfo, poolType), poolType);

        Debug.Log($"Object Pool: PassObjectToPool: PoolToUse {poolToUse != null}");
        if (poolToUse != null)
        {
            poolToUse.Enqueue(objectBase);
        }
    }

    private int GetPoolInfoBasedOnType(string poolItemTypeInfo, PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Item:
                return (int)Enum.Parse(typeof(ItemType), poolItemTypeInfo);
            case PoolType.Truck:
                return (int)Enum.Parse(typeof(TruckType), poolItemTypeInfo);
            default: 
                return -1;
        }
    }
    
    public ObjectPoolBase GetUsedPool(int poolIndex, PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Item:
                var objectType = (ItemType)poolIndex;
                if (itemsPoolBasesDict.ContainsKey(objectType))
                    return itemsPoolBasesDict[objectType];
                else 
                    return null;

            case PoolType.Truck:
                var objectType1 = (TruckType)poolIndex;
                if (truckPoolBasesDict.ContainsKey(objectType1))
                    return truckPoolBasesDict[objectType1];
                else 
                    return null;
                    
            default:
                return null;
        }
    }
}
