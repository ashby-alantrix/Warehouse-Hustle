using UnityEngine;

public class TruckObjectPool : ObjectPoolBase<TruckBase>
{
    [SerializeField] protected TruckType truckType;
    public TruckType GetPoolType() => truckType;// goodsType;

    public override void InitPoolFirstTime()
    {
        base.InitPoolFirstTime();
    }
}