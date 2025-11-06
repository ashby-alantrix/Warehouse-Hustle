using DG.Tweening;
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
    [SerializeField] private GoodsInputPlatform currentGoodsPlacer;
    [SerializeField] private GoodsInputPlatform nextGoodsPlacer;
    [SerializeField] private byte minGoods = 2;
    [SerializeField] private byte maxGoods = 12;

    private ItemType[] goodsType;
    private List<GoodsSet> lastUpdatedGoodsSet = new List<GoodsSet>();

    public GoodsInputPlatform CurrentGoodsPlacer => currentGoodsPlacer;
    public GoodsInputPlatform NextGoodsPlacer => nextGoodsPlacer;

    public void InitGoodsInfo()
    {
        InitGoodsTypes();
        InitCurrentAndNextGoods();
    }

    private void InitGoodsTypes()
    {
        int maxElements = (int)ItemType.MAX;
        goodsType = new ItemType[maxElements];

        for (int index = 0; index < maxElements; index++)
            goodsType[index] = (ItemType)index;
    }

    public void InitCurrentAndNextGoods()
    {
        InitGoods();
        currentGoodsPlacer.InitGoodsView(new List<GoodsSet>(lastUpdatedGoodsSet));
        currentGoodsPlacer.PlaceGoods();

        InitGoods();
        nextGoodsPlacer.InitGoodsView(new List<GoodsSet>(lastUpdatedGoodsSet));
        nextGoodsPlacer.PlaceGoods();
    }

    private void InitGoods()
    {
        lastUpdatedGoodsSet.Clear();

        CreateGoodsSet();
        // CreateGoodsSetTest();
    }

    private void CreateGoodsSetTest()
    {
        for (int i=0; i<2; i++)
        {
            var goodsSetObj = new GoodsSet();
            UseTestData(ref goodsSetObj);
            //goodsSetObj.type = GenerateRandomGoodsType();
            //goodsSetObj.setCount = GenerateRandomSetCount(remCountInSet);

            lastUpdatedGoodsSet.Add(goodsSetObj);
        }
    }

    private void CreateGoodsSet()
    {
        int remCountInSet = (byte)UnityEngine.Random.Range(minGoods, maxGoods);
        while (remCountInSet >= minGoods)
        {
            var goodsSetObj = new GoodsSet();
            // UseTestData(ref goodsSetObj);
            goodsSetObj.type = GenerateRandomGoodsType();
            goodsSetObj.setCount = GenerateRandomSetCount(remCountInSet);

            lastUpdatedGoodsSet.Add(goodsSetObj);
            remCountInSet -= goodsSetObj.setCount;
        }
    }

    private int testGoodTypeIndex = 0;

    public void UseTestData(ref GoodsSet goodsSetObj)
    {
        goodsSetObj.type = goodsType[testGoodTypeIndex];
        goodsSetObj.setCount = 2;

        testGoodTypeIndex = testGoodTypeIndex == 0 ? 1 : 0;
    }

    public void UpdateGoodsInputPlatform()
    {
        currentGoodsPlacer.InitGoodsView(new List<GoodsSet>(lastUpdatedGoodsSet));

        currentGoodsPlacer.SetBaseObjects(new List<ItemBase>(nextGoodsPlacer.GetBaseObjects()));

        TweenNextObjectsToCurrentInputPlatform();
    }

    private void UpdateNextInputGoods()
    {
        Debug.Log($"{name}.UpdateNextInputGoods :: ");
        InitGoods();
        nextGoodsPlacer.InitGoodsView(new List<GoodsSet>(lastUpdatedGoodsSet));
        nextGoodsPlacer.PlaceGoods();
    }

    public void TweenNextObjectsToCurrentInputPlatform()
    {
        int count = currentGoodsPlacer.GetBaseObjectsCount();
        Tween inputPlatformTween = null;
        ItemBase itemBase = null;

        for (int index = 0; index < count; index++)
        {
            itemBase = currentGoodsPlacer.GetItemBasedOnIndex(index);
            inputPlatformTween = itemBase.transform.DOMove(
                currentGoodsPlacer.GetSpawnPointTransform(index).position,
                1f
            );
        }

        inputPlatformTween?.OnComplete(() => UpdateNextInputGoods());
    }

    private ItemType GenerateRandomGoodsType()
    {
        return goodsType[UnityEngine.Random.Range(0, goodsType.Length)];
    }

    private byte GenerateRandomSetCount(int availCountInSet)
    {
        return (byte)UnityEngine.Random.Range(minGoods, availCountInSet);
    }
}
