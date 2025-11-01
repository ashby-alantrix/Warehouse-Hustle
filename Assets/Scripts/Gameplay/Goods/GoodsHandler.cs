using System;
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
    [SerializeField] private byte minGoods = 1;
    [SerializeField] private byte maxGoods = 12;

    private GoodsSet[] goodsSet;
    private ItemType[] goodsType;

    private void Awake()
    {
        InitGoodsTypes();
        InitCurrentAndNextGoods(2);
    }

    private void InitGoodsTypes()
    {
        int maxElements = (int)ItemType.MAX;
        goodsType = new ItemType[maxElements];

        for (int index = 0; index < maxElements; index++)
        {
            goodsType[index] = (ItemType)index;
        }
    }

    public void InitCurrentAndNextGoods(int splitGoodsCount)
    {
        InitGoods(splitGoodsCount);
        currentGoodsPlacer.InitGoodsView(goodsSet);

        InitGoods(splitGoodsCount);
        nextGoodsPlacer.InitGoodsView(goodsSet);
    }

    private void InitGoods(int splitGoodsCount)
    {
        byte totalGoodsToGive = (byte)UnityEngine.Random.Range(minGoods, maxGoods); // 1 && 12 => 7
        goodsSet = new GoodsSet[splitGoodsCount];

        for (int i = 0; i < splitGoodsCount; i++)
        {
            goodsSet[i] = new GoodsSet();
            goodsSet[i].type = GenerateRandomGoodsType();
            goodsSet[i].setCount = GenerateRandomSetCount(totalGoodsToGive);
        }
    }

    private ItemType GenerateRandomGoodsType()
    {
        return goodsType[UnityEngine.Random.Range(minGoods - 1, goodsType.Length)];
    }

    private byte GenerateRandomSetCount(int totalGoodsToGive)
    {
        return (byte)UnityEngine.Random.Range(minGoods, totalGoodsToGive);
    }
}
