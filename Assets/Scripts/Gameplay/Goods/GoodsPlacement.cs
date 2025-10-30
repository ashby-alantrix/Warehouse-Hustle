using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsPlacement : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    private GoodsSet[] m_GoodsSet = null;

    public void PlaceGoods()
    {
        
    }

    public void InitGoodsView(GoodsSet[] goodsSet)
    {
        this.m_GoodsSet = goodsSet;

        PlaceGoods();
    }
}
