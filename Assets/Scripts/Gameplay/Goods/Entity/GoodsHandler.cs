using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        //CreateGoodsSetTest();
    }

    private void CreateGoodsSetTest()
    {
        testGoodTypeIndex = 0;
        for (int i=0; i<3; i++)
        {
            var goodsSetObj = new GoodsSet();
            UseTestData(ref goodsSetObj);

            lastUpdatedGoodsSet.Add(goodsSetObj);
        }
    }

    private void CreateGoodsSet()
    {
        int remCountInSet = (byte)UnityEngine.Random.Range(minGoods, maxGoods);
        while (remCountInSet >= minGoods)
        {
            var goodsSetObj = new GoodsSet();
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
        goodsSetObj.setCount = 3; // check 2 and 3

        testGoodTypeIndex++;
        // testGoodTypeIndex = testGoodTypeIndex == 0 ? 1 : 0;
    }

    public void UpdateGoodsInputPlatform()
    {
        currentGoodsPlacer.InitGoodsView(new List<GoodsSet>(nextGoodsPlacer.GetGoodsDataSet()));

        currentGoodsPlacer.SetBaseObjects(new List<ItemBase>(nextGoodsPlacer.GetBaseObjects()));

        TweenNextObjectsToCurrentInputPlatform();
    }

    public void SwapInputPlatformsData()
    {
        var storedNextGoodsSet = new List<GoodsSet>(nextGoodsPlacer.GetGoodsDataSet()); //
        var storedNextBaseObjects = new List<ItemBase>(nextGoodsPlacer.GetBaseObjects()); //

        int count = currentGoodsPlacer.GetBaseObjectsCount();
        Tween inputPlatformTween1 = null, inputPlatformTween2 = null;
        ItemBase itemBase = null;

        for (int index = 0; index < count; index++)
        {
            itemBase = currentGoodsPlacer.GetItemBasedOnIndex(index);
            inputPlatformTween1 = itemBase.transform.DOMove(
                nextGoodsPlacer.GetSpawnPointTransform(index).position,
                1f
            );
        }

        nextGoodsPlacer.InitGoodsView(new List<GoodsSet>(currentGoodsPlacer.GetGoodsDataSet()));
        nextGoodsPlacer.SetBaseObjects(new List<ItemBase>(currentGoodsPlacer.GetBaseObjects()));

        count = storedNextBaseObjects.Count;
        for (int index = 0; index < count; index++)
        {
            itemBase = storedNextBaseObjects[index];
            inputPlatformTween2 = itemBase.transform.DOMove(
                currentGoodsPlacer.GetSpawnPointTransform(index).position,
                1f
            );
        }

        currentGoodsPlacer.InitGoodsView(new List<GoodsSet>(storedNextGoodsSet));
        currentGoodsPlacer.SetBaseObjects(new List<ItemBase>(storedNextBaseObjects));
    }

    private void UpdateNextInputGoods()
    {
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
