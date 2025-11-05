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
        var totalItems = selectedNode.GetItemBaseCount();
        int counter = 0, customIndexer = -1;

        var keys = selectedNode.GetKeysForItems(); // 0 1 2

        canPlaceGoods = false;

        for (int indexI = 0; indexI < totalItems; indexI++) // 0 1 2 3 // 4 5 6 7 // 8 9 10 11 //
        {
            customIndexer++;
            nodePlacementData = selectedNode.RetrieveNodePlacementData(indexI);
            nodePlacementData.isOccupied = true;

            nodesMoverTween = selectedNode.GetItemBase(customIndexer, keys[counter]).transform.DOMove(nodePlacementData.transform.position, 1f);

            if (customIndexer == selectedNode.GetSetsCountForItemType(keys[counter]) - 1) // 3 == 4 - 1
            {
                customIndexer = -1;
                counter++;
            }
        }

        nodesMoverTween.OnComplete(() =>
        {
            canPlaceGoods = true;
            goodsSortingManager.CheckNeighbors(selectedNode);
        });
    }

    public void RearrangeGoodsBetweenSelectedNodeAndNeighbor(ItemType itemType, Node selectedNode, Node neighborNode)
    {
        // Debug.Log($"Test5: Neighbors position: {neighborNode.transform.position}");
        var itemBases = neighborNode.GetSpecificItems(itemType);

        var itemBaseCount = selectedNode.GetItemBaseCount();
        var totalSlots = selectedNode.GetTotalSlotsInNode();

        Debug.Log($"Test 11: itemBases: {itemBases.Count}");
        Debug.Log($"Test 11: itemType: {itemType}");

        for (int indexJ = itemBaseCount; indexJ < itemBaseCount + itemBases.Count; indexJ++)
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
