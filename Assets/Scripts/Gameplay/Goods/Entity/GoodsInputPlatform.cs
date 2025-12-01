using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GoodsInputPlatform : MonoBehaviour
{
    [SerializeField] private Transform goodsParent;
    [SerializeField] private Transform[] spawnPoints;

    private List<GoodsSet> goodsDataSet = null;
    private List<ItemBase> baseObjects = new List<ItemBase>();

    private ObjectPoolManager objectPoolManager = null;

    public int GetBaseObjectsCount()
    {
        return baseObjects.Count;
    }

    public List<GoodsSet> GetGoodsDataSet()
    {
        return goodsDataSet;
    }

    public List<ItemBase> GetBaseObjects()
    {
        return baseObjects;
    }

    public void SetBaseObjects(List<ItemBase> baseObjects)
    {
        this.baseObjects.Clear();
        this.baseObjects = baseObjects;
    }

    public void ClearGoodsSetAndBaseObjects()
    {
        goodsDataSet.Clear();

        foreach (var baseObj in baseObjects)
        {
            baseObj.gameObject.SetActive(false);
            objectPoolManager.PassObjectToPool<ItemBase>($"{baseObj.ItemType}", PoolType.Item, baseObj);
        }

        baseObjects.Clear();
    }

    public void PlaceGoods()
    {
        // get the goods object from the pool here and spawn based on the provided 
        objectPoolManager = objectPoolManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>() : objectPoolManager;
        int spawnPointIndex = 0;
        ItemBase baseObj = null;

        baseObjects.Clear();

        if (objectPoolManager != null)
        {
            for (int i = 0; i < goodsDataSet.Count; i++)
            {
                for (int j = 0; j < goodsDataSet[i].setCount; j++)
                {
                    // Debug.Log($"Object Pool :: {goodsDataSet[i].type.ToString()}");
                    baseObj = objectPoolManager.GetObjectFromPool<ItemBase>(goodsDataSet[i].type.ToString(), PoolType.Item);
                    
                    if (spawnPointIndex < spawnPoints.Length)
                    {
                        SetBaseObjectProps();
                        baseObjects.Add(baseObj);
                        spawnPointIndex++;
                    }
                    else
                    {
                        Debug.LogError("Index out of range: for spawnPointIndex");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Object pool manager is null");
        }

        void SetBaseObjectProps()
        {
            baseObj.transform.position = spawnPoints[spawnPointIndex].position;
            baseObj.transform.parent = goodsParent;
            baseObj.transform.gameObject.SetActive(true);
        }
    }

    public ItemBase GetItemBasedOnIndex(int index)
    {
        return baseObjects[index];
    }

    public Transform GetSpawnPointTransform(int index)
    {
        return spawnPoints[index];
    }
    
    public void InitGoodsView(List<GoodsSet> goodsSet)
    {
        this.goodsDataSet = goodsSet;
    }
}
