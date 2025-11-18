using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GoodsQueuerData
{
    public GoodsQueuer goodsQueuer;
    public bool isOccupied;
}

public class GoodsWaitingQueuer : MonoBehaviour
{
    [SerializeField] private List<GoodsQueuerData> goodsQueuers = new List<GoodsQueuerData>();

    private int lastFilledIndex = -1;

    public GoodsQueuerData GetGoodsQueuer()
    {
        int unoccupiedIndex = goodsQueuers.FindIndex(goodsQueuers => !goodsQueuers.isOccupied);
        Debug.Log($"Unoccupied index: {unoccupiedIndex}");
        
        return goodsQueuers.FirstOrDefault(goodsQueuer => !goodsQueuer.isOccupied);
    }

    public void InitGoods(List<ItemBase> itemBases, out GoodsQueuerData goodsQueuerData)
    {
        goodsQueuerData = GetGoodsQueuer();
        if (goodsQueuerData == null) 
        {
            Debug.LogError($"Goods queuer data is null");
            return;
        }

        goodsQueuerData.isOccupied = true;
        Tween tweener = null;

        for (int indexI = 0; indexI < goodsQueuerData.goodsQueuer.SlotsPlacer.TotalSlotsInNode; indexI++)
        {
            tweener = itemBases[indexI].transform.DOMove(goodsQueuerData.goodsQueuer.SlotsPlacer.GetPosDataBasedOnIndex(indexI), 1f);
        }

        tweener.OnComplete(() => tweener.Kill());
    }
}
