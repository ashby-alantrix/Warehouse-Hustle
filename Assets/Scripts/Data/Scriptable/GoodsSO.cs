using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GoodsView
{
    public GoodsType goodsType;
    public GameObject gameObj;
}

[CreateAssetMenu(fileName = "GoodsSO", menuName = "GoodsSO")]
public class GoodsSO : ScriptableObject
{
    // public GoodsType
    public List<GoodsView> goodsModel = new List<GoodsView>();
}
