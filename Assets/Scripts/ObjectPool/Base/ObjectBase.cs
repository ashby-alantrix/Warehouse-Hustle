using UnityEngine;

public class ItemBase : MonoBehaviour
{
    [SerializeField] private ItemType itemType;

    public int nodePlacementIndex;

    public ItemType ItemType => itemType;
}
