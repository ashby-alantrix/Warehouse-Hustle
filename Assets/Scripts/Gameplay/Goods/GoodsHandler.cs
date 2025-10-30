using System;
using UnityEngine;

public class GoodsSet
{
    public byte setCount;
    public GoodsType type;
}

public class GoodsHandler : MonoBehaviour
{
    // collection of goods
    // define all the different goods and their types inside a SO
    [SerializeField] private GoodsPlacement currentGoodsPlacer;
    [SerializeField] private GoodsPlacement nextGoodsPlacer;

    //[SerializeField] private GoodsSO goodsSO;

    [SerializeField] private GoodsType[] goodsType;

    [SerializeField] private byte minGoods = 1;
    [SerializeField] private byte maxGoods = 12;

    public void InitCurrentAndNextGoods(int splitGoodsCount)
    {
        byte totalGoodsToGive = (byte)UnityEngine.Random.Range(minGoods, maxGoods); // 1 && 12 => 7
        GoodsSet[] goodsSet = new GoodsSet[splitGoodsCount];

        for (int i = 0; i < splitGoodsCount; i++)
        {
            goodsSet[i] = new GoodsSet();
            goodsSet[i].type = GenerateRandomGoodsType();
            goodsSet[i].setCount = GenerateRandomSetCount(totalGoodsToGive);
        }

        currentGoodsPlacer.InitGoodsView(goodsSet);
    }

    private GoodsType GenerateRandomGoodsType()
    {
        return goodsType[UnityEngine.Random.Range(minGoods - 1, maxGoods)];
    }

    private byte GenerateRandomSetCount(int totalGoodsToGive)
    {
        return (byte)UnityEngine.Random.Range(minGoods, totalGoodsToGive);
    }
}
