using System.Collections.Generic;
using UnityEngine;

public class GoodsInputPlatform : MonoBehaviour
{
    [SerializeField] private Transform goodsParent;
    [SerializeField] private Transform[] spawnPoints;

    private List<GoodsSet> m_GoodsDataSet = null;
    private List<ItemBase> baseObjects = new List<ItemBase>();

    private ObjectPoolManager objectPoolManager = null;

    public int GetBaseObjectsCount()
    {
        return baseObjects.Count;
    }

    public List<GoodsSet> GetGoodsDataSet()
    {
        return m_GoodsDataSet;
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

    public void PlaceGoods()
    {
        // get the goods object from the pool here and spawn based on the provided 
        objectPoolManager = objectPoolManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>() : objectPoolManager;
        int spawnPointIndex = 0;
        ItemBase baseObj = null;

        baseObjects.Clear();

        if (objectPoolManager != null)
        {
            for (int i = 0; i < m_GoodsDataSet.Count; i++)
            {
                for (int j = 0; j < m_GoodsDataSet[i].setCount; j++)
                {
                    baseObj = objectPoolManager.GetObjectFromPool(m_GoodsDataSet[i].type);
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
            // baseObj.name = baseObj.name + " " + spawnPointIndex;
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
        this.m_GoodsDataSet = goodsSet;
    }
}
