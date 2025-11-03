using System.Collections.Generic;
using UnityEngine;

public class GoodsPlacement : MonoBehaviour
{
    [SerializeField] private Transform goodsParent;
    [SerializeField] private Transform[] spawnPoints;

    private List<GoodsSet> m_GoodsSet = null;

    private ObjectPoolManager objectPoolManager = null;

    public void PlaceGoods()
    {
        // get the goods object from the pool here and spawn based on the provided 
        objectPoolManager = InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>();
        int spawnPointIndex = 0;

        if (objectPoolManager != null)
        {
            for (int i = 0; i < m_GoodsSet.Count; i++)
            {
                for (int j = 0; j < m_GoodsSet[i].setCount; j++)
                {
                    ItemBase baseObj = objectPoolManager.GetObjectFromPool(m_GoodsSet[i].type);
                    if (spawnPointIndex < spawnPoints.Length)
                    {
                        baseObj.name = baseObj.name + " " + spawnPointIndex;
                        baseObj.transform.position = spawnPoints[spawnPointIndex].position;
                        Debug.Log("### spawnPointIndex: " + spawnPointIndex + ", pos: " + baseObj.transform.position + " objName: " + baseObj.transform.name);
                        baseObj.transform.gameObject.SetActive(true);
                        baseObj.transform.parent = goodsParent;
                        spawnPointIndex++;
                    }
                    else
                    {
                        Debug.LogError("Index out of range: spawnPointIndex");
                    }
                }
            }
        }
        else
        {
            Debug.Log("Object pool manager is null");
        }
    }

    public void InitGoodsView(List<GoodsSet> goodsSet)
    {
        this.m_GoodsSet = goodsSet;

        PlaceGoods();
    }
}
