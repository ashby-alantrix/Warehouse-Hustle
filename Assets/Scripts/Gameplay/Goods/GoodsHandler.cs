using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GoodsSet
{
    public byte setCount;
    public ItemType type;
}

public class GoodsHandler : MonoBehaviour
{
    [SerializeField] private GoodsPlacement currentGoodsPlacer;
    [SerializeField] private GoodsPlacement nextGoodsPlacer;
    [SerializeField] private byte minGoods = 2;
    [SerializeField] private byte maxGoods = 12;

    private List<GoodsSet> goodsSet;
    private ItemType[] goodsType;

    public void InitGoodsInfo()
    {
        InitGoodsTypes();
        InitCurrentAndNextGoods(2);
    }

    private void InitGoodsTypes()
    {
        int maxElements = (int)ItemType.MAX;
        goodsType = new ItemType[maxElements];

        for (int index = 0; index < maxElements; index++)
            goodsType[index] = (ItemType)index;

        Debug.Log("Goods type length: " + goodsType.Length);
    }

    public void InitCurrentAndNextGoods(int splitGoodsCount)
    {
        Debug.Log("Current Goods Placer");
        InitGoods(splitGoodsCount);
        currentGoodsPlacer.InitGoodsView(goodsSet);

        Debug.Log("Next Goods Placer");
        InitGoods(splitGoodsCount);
        nextGoodsPlacer.InitGoodsView(goodsSet);
    }

    private void InitGoods(int splitGoodsCount)
    {
        goodsSet = new List<GoodsSet>();

        int remCountInSet = (byte)UnityEngine.Random.Range(minGoods, maxGoods);
        Debug.Log("Serialized GoodsSet remCountInSet: " + remCountInSet);
        while (remCountInSet >= minGoods)
        {
            var goodsSetObj = new GoodsSet();
            goodsSetObj.type = GenerateRandomGoodsType();
            Debug.Log("Serialized GoodsSet remCountInSet in iteration: " + remCountInSet);
            goodsSetObj.setCount = GenerateRandomSetCount(remCountInSet);
            Debug.Log("Serialized GoodsSet goodsSetObj.setCount: " + goodsSetObj.setCount);

            goodsSet.Add(goodsSetObj);
            remCountInSet -= goodsSetObj.setCount;
        }

        Debug.Log("Serialized GoodsSet: " + JsonConvert.SerializeObject(goodsSet));
    }

    private ItemType GenerateRandomGoodsType()
    {
        return goodsType[UnityEngine.Random.Range(minGoods - 1, goodsType.Length)];
    }

    private byte GenerateRandomSetCount(int availCountInSet)
    {
        return (byte)UnityEngine.Random.Range(minGoods, availCountInSet);
    }
}
