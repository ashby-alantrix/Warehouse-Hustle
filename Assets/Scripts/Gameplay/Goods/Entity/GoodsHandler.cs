using DG.Tweening;
using Newtonsoft.Json;
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

    public void UpdateGoodsInputPlatform()
    {
        Debug.Log("### lastUpdatedGoodsSet: " + JsonConvert.SerializeObject(lastUpdatedGoodsSet));
        currentGoodsPlacer.InitGoodsView(new List<GoodsSet>(lastUpdatedGoodsSet));

        Debug.Log("### nextGoodsPlacer.GetBaseObjects(): " + nextGoodsPlacer.GetBaseObjects().Count);
        currentGoodsPlacer.SetBaseObjects(new List<ItemBase>(nextGoodsPlacer.GetBaseObjects()));

        Debug.Log("### currentGoodsPlacer.GetBaseObjects(): " + currentGoodsPlacer.GetBaseObjects().Count);
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

        for (int index = 0; index < count; index++)
        {
            inputPlatformTween = currentGoodsPlacer.GetItemBasedOnIndex(index).transform.DOMove(
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
