using System;
using UnityEngine;

public class GoodsHandler : MonoBehaviour
{
    // collection of goods
    // define all the different goods and their types inside a SO
    [SerializeField] private GoodsPlacement currentGoodsPlacer;
    [SerializeField] private GoodsPlacement nextGoodsPlacer;

    [SerializeField] private GoodsSO goodsSO;

    [SerializeField] private int minGoods;
    [SerializeField] private int maxGoods;

    public void InitCurrentAndNextGoods(int splitGoodsCount)
    {
        var totalGoodsToGive = UnityEngine.Random.Range(minGoods, maxGoods);
        int firstSetCount = 0, secondSetCount = 0;

        if (splitGoodsCount == 1)
        {
            firstSetCount = UnityEngine.Random.Range(minGoods, totalGoodsToGive);
        }
        else if (splitGoodsCount == 2)
        {
            firstSetCount = UnityEngine.Random.Range(minGoods, totalGoodsToGive);
            secondSetCount = UnityEngine.Random.Range(firstSetCount + 1, maxGoods);
        }

        
    }
}
