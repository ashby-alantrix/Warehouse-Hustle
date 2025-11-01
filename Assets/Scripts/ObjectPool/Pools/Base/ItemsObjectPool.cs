using System;
using UnityEngine;

public class ItemsObjectPool<T> : ObjectPoolBase<T> where T : ItemBase
{
    [SerializeField] protected ItemType goodsType;

    public override ItemType GetPoolType() => goodsType;// goodsType;

    public override void InitPoolFirstTime()
    {
        return;
        for (int i = 0; i < initialPoolCount; i++)
        {
            EnqueueNewInst();
        }
    }
}

public class ItemsObjectPool : ItemsObjectPool<ItemBase>
{
    
}