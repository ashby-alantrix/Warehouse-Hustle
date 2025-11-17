using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckBase : ObjectBase
{
    [SerializeField] private TruckType truckType;

    private TruckGoodsLoader truckGoodsLoader;
    private TruckMover truckMover;

    public TruckType TruckType => truckType;

    public TruckGoodsLoader TruckGoodsLoader => truckGoodsLoader;
    public TruckMover TruckMover => truckMover;

    void Awake()
    {
        truckMover = GetComponent<TruckMover>();
        truckGoodsLoader = GetComponent<TruckGoodsLoader>();
    }
}
