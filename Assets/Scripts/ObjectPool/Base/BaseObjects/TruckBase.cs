using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckBase : ObjectBase
{
    [SerializeField] private TruckType truckType;

    private TruckGoodsLoader truckGoodsLoader;

    public TruckType TruckType => truckType;

    public TruckGoodsLoader TruckGoodsLoader => truckGoodsLoader;

    void Awake()
    {
        truckGoodsLoader = GetComponent<TruckGoodsLoader>();
    }
}
