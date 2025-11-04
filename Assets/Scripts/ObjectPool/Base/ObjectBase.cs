using UnityEngine;

public class ItemBase : MonoBehaviour
{
    [SerializeField] private ItemType itemType;

    public ItemType ItemType => itemType;
}
