using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GoodsPlacementManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private GoodsSortingManager goodsSortingManager;

    private bool canPlaceGoods = true;

    public bool CanPlaceGoods => canPlaceGoods;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsPlacementManager>(this);
    }

    public void PlaceGoodsInsideNode(Node selectedNode)
    {
        selectedNode.InitItemsData();
        NodePlacementData nodePlacementData = null;
        Tween nodesMoverTween = null;
        var itemBaseCount = selectedNode.GetItemBaseCount();
        int counter = 0, customIndexer = -1;

        var keys = selectedNode.GetKeysForItems(); // Weapon, Armour, Boots
        Debug.Log($"Keys count: " + keys.Count);
        Debug.Log($"itemBaseCount: " + itemBaseCount);

        canPlaceGoods = false;

        for (int indexI = 0; indexI < itemBaseCount; indexI++) // 0 1 2 3 (4) // 4 5 6 // 7 8 9 //
        {
            customIndexer++;
            nodePlacementData = selectedNode.RetrieveNodePlacementData(indexI);
            nodePlacementData.isOccupied = true;

            nodesMoverTween = selectedNode.GetItemBase(customIndexer, keys[counter]).transform.DOMove(nodePlacementData.transform.position, 1f);

            if (customIndexer == selectedNode.GetSetsCountForItemType(keys[counter]) - 1) // 3 == 4 - 1
            {
                customIndexer = 0;
                counter++;
            }
        }

        nodesMoverTween.OnComplete(() =>
        {
            Debug.Log($"### Test4 nodesMoverTween.OnComplete");
            canPlaceGoods = true;
            goodsSortingManager.CheckNeighbors(selectedNode);
        });
    }

    public void RearrangeGoodsBetweenSelectedNodeAndNeighbor(ItemType itemType, Node selectedNode, Node neighborNode)
    {
        var itemBases = neighborNode.GetSpecificItems(itemType);
        var itemBaseCount = selectedNode.GetItemBaseCount();
        var totalSlots = selectedNode.GetTotalSlotsInNode;

        for (int indexJ = itemBaseCount; indexJ < totalSlots; indexJ++)
        {
            NodePlacementData nodePlacementData = selectedNode.RetrieveNodePlacementData(indexJ);
            if (!nodePlacementData.isOccupied)
            {
                var itemBase = itemBases[indexJ - itemBaseCount];
                itemBase.transform.DOMove(nodePlacementData.transform.position, 1f);
            }
        }
    }
}
