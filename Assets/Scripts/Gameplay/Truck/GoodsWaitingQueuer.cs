using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsWaitingQueuer : MonoBehaviour
{
    [SerializeField] private List<GoodsQueuer> goodsQueuers;

    private int lastFilledIndex = -1;

    public GoodsQueuer GetGoodsQueuer()
    {
        lastFilledIndex += 1;
        if (lastFilledIndex >= goodsQueuers.Count) Debug.LogError($"Index out of range");

        return goodsQueuers[lastFilledIndex];
    }

    public GoodsQueuer GetInitGoodsQueuer()
    {
        if (lastFilledIndex == -1) return null;

        GoodsQueuer goodsQueuer = goodsQueuers[0];
        goodsQueuers.RemoveAt(0);
        return goodsQueuer;
    }

    public void InitGoods(List<ItemBase> itemBases)
    {
        GoodsQueuer goodsQueuer = GetGoodsQueuer();
        Tween tweener = null;

        for (int indexI = 0; indexI < goodsQueuer.SlotsPlacer.TotalSlotsInNode; indexI++)
        {
            tweener = itemBases[indexI].transform.DOMove(goodsQueuer.SlotsPlacer.GetPosDataBasedOnIndex(indexI), 1f);
        }

        tweener.OnComplete(() => tweener.Kill());
    }
}
