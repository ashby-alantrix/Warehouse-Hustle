using UnityEngine;

public class ItemsObjectPool : ObjectPoolBase<ItemBase>
{
    [SerializeField] protected ItemType goodsType;
    public ItemType GetPoolType() => goodsType;// goodsType;

    public override void InitPoolFirstTime()
    {
        base.InitPoolFirstTime();
    }
}