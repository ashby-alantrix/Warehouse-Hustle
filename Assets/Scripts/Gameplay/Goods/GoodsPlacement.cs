using UnityEngine;

public class GoodsPlacement : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    private GoodsSet[] m_GoodsSet = null;

    private ObjectPoolManager objectPoolManager = null;

    public void PlaceGoods()
    {
        // get the goods object from the pool here and spawn based on the provided 
        objectPoolManager = InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>();
        int spawnPointIndex = 0;

        if (objectPoolManager != null)
        {
            for (int i = 0; i < m_GoodsSet.Length; i++)
            {
                for (int j = 0; j < m_GoodsSet[i].setCount; j++)
                {
                    ItemBase baseObj = objectPoolManager.GetObjectFromPool(m_GoodsSet[i].type);
                    if (spawnPointIndex < spawnPoints.Length)
                    {
                        baseObj.transform.position = spawnPoints[spawnPointIndex++].position;
                        baseObj.transform.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError("Index out of range: spawnPointIndex");
                    }
                }
            }
        }
    }

    public void InitGoodsView(GoodsSet[] goodsSet)
    {
        this.m_GoodsSet = goodsSet;

        PlaceGoods();
    }
}
