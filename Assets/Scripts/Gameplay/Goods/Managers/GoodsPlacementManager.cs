using System;
using System.Collections.Generic;
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
        Tween nodesMoverTween = null;
        var totalItems = selectedNode.GetItemBaseCount();

        canPlaceGoods = false;
        IterateAndMoveNodesUsingDictionary(selectedNode, totalItems, ref nodesMoverTween);

        nodesMoverTween.OnComplete(() =>
        {
            canPlaceGoods = true;
            goodsSortingManager.CheckNeighbors(selectedNode);
        });
    }

    private void IterateAndMoveNodesUsingDictionary(Node selectedNode, int totalItems, ref Tween nodesMoverTween)
    {
        var keys = selectedNode.GetKeysForItems();
        NodePlacementData nodePlacementData = null;
        int counter = 0, customIndexer = -1;

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
    }

    public void RearrangeGoodsBetweenSelectedNodeAndNeighbor(ItemType itemType, Node selectedNode, Node neighborNode, out Tween tweener)
    {
        // Debug.Log($"Test5: Neighbors position: {neighborNode.transform.position}");
        tweener = null;
        var itemBases = neighborNode.GetSpecificItems(itemType);

        var itemBaseCount = selectedNode.GetItemBaseCount();

        Debug.Log($"Test 11: itemBases: {itemBases.Count}");
        Debug.Log($"Test 11: itemType: {itemType}");

        for (int indexJ = itemBaseCount; indexJ < itemBaseCount + itemBases.Count; indexJ++)
        {
            NodePlacementData nodePlacementData = selectedNode.RetrieveNodePlacementData(indexJ);
            if (!nodePlacementData.isOccupied) // change the state periodically
            {
                var itemBase = itemBases[indexJ - itemBaseCount];
                tweener = itemBase.transform.DOMove(nodePlacementData.transform.position, 1f);
                //itemBase.nodePlacementIndex = indexJ;
            }
        }
    }

    public void RearrangeBasedOnSorting(Node currentNode)
    {
        var itemBaseCount = currentNode.GetItemBaseCount();
        Debug.Log($"Sorting :: itemBaseCount: {itemBaseCount}");
        Tween nodesMoverTween = null;

        IterateAndMoveNodesUsingDictionary(currentNode, itemBaseCount, ref nodesMoverTween);
    }
}
