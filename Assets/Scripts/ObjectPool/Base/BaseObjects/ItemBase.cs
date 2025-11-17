using UnityEngine;

public class ItemBase : ObjectBase
{
    [SerializeField] private ItemType itemType;

    public int nodePlacementIndex;

    public ItemType ItemType => itemType;
}
