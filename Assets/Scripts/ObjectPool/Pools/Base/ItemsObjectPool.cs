using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ItemsObjectPool<T> : ObjectPoolBase<T> where T : ItemBase
{
    [SerializeField] protected ItemType goodsType;

    public override void InitPoolFirstTime()
    {
        T itemInst = null;
        for (int i = 0; i < initialPoolCount; i++)
        {
            itemInst = CreateNewPoolItem();
            Enqueue(itemInst);
        }
    }

    public override ItemType GetPoolType() => goodsType;// goodsType;
}

public class ItemsObjectPool : ItemsObjectPool<ItemBase>
{
}