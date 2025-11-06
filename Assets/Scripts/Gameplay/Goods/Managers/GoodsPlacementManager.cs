using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GoodsPlacementManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private GoodsSortingManager goodsSortingManager;
    [SerializeField] private float tweenDelay = 1f;

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
        Debug.Log($"OnNodeClicked: PlaceGoodsInsideNode :: name: {selectedNode.transform.name}, canPlaceGoods: {canPlaceGoods}");
        IterateAndMoveNodesUsingDictionary(selectedNode, totalItems, ref nodesMoverTween);

        nodesMoverTween.OnComplete(() =>
        {
            canPlaceGoods = true;
            Debug.Log($"OnNodeClicked: nodesMoverTween.OnComplete :: name: {selectedNode.transform.name}, canPlaceGoods: {canPlaceGoods}");
            goodsSortingManager.CheckNeighbors(selectedNode);
        });
    }

    private void IterateAndMoveNodesUsingDictionary(Node selectedNode, int totalItems, ref Tween nodesMoverTween)
    {
        var keys = selectedNode.GetKeysForItems();
        NodePlacementData nodePlacementData = null;
        int counter = 0, customIndexer = -1;
        ItemBase itemBase = null;

        for (int indexI = 0; indexI < totalItems; indexI++) // 0 1 2 3 // 4 5 6 7 // 8 9 10 11 //
        {
            customIndexer++;
            nodePlacementData = selectedNode.RetrieveNodePlacementData(indexI);
            nodePlacementData.isOccupied = true;

            itemBase = selectedNode.GetItemBase(customIndexer, keys[counter]);
            itemBase.nodePlacementIndex = indexI;
            nodesMoverTween = itemBase.transform.DOMove(nodePlacementData.transform.position, tweenDelay);

            if (customIndexer == selectedNode.GetSetsCountForItemType(keys[counter]) - 1) // 3 == 4 - 1
            {
                customIndexer = -1;
                counter++;
            }
        }
    }

    public void RearrangeGoodsBetweenSelectedNodeAndNeighbor(ItemType itemType, Node neighborNode, Node currentSelectedNode) //, out Tween tweener)
    {
        // Debug.Log($"Test5: Neighbors position: {neighborNode.transform.position}");
        Tweener tweener = null;
        var currentItemBases = currentSelectedNode.GetSpecificItems(itemType);

        var neighborItemBaseCount = neighborNode.GetItemBaseCount();

        Debug.Log($"Test 11: itemBases: {currentItemBases.Count}");
        Debug.Log($"Test 11: itemType: {itemType}");

        for (int indexJ = neighborItemBaseCount; indexJ < neighborItemBaseCount + currentItemBases.Count; indexJ++) // TODO :: logic needs to be updated
        {
            NodePlacementData nodePlacementData = neighborNode.RetrieveNodePlacementData(indexJ);
            if (!nodePlacementData.isOccupied) // change the state periodically
            {
                var itemBase = currentItemBases[indexJ - neighborItemBaseCount];
                itemBase.nodePlacementIndex = indexJ;
                tweener = itemBase.transform.DOMove(nodePlacementData.transform.position, 1f);
            }
        }

        tweener.OnComplete(() =>
        {
            Debug.Log($"MoveMatchedSetToNeighbor OnComplete");
            Debug.Log($"MoveMatchedSetToNeighbor OnComplete: currentSelectedNode :: name: {currentSelectedNode.transform.name}, pos: {currentSelectedNode.transform.position}");
            Debug.Log($"MoveMatchedSetToNeighbor OnComplete: neighborNode :: name: {neighborNode.transform.name}, pos: {neighborNode.transform.position}");

            currentSelectedNode.CheckIfNodeIsFullOrCleared();
            neighborNode.CheckIfNodeIsFullOrCleared();
        });
    }

    public void RearrangeBasedOnSorting(Node currentNode)
    {
        var itemBaseCount = currentNode.GetItemBaseCount();
        Debug.Log($"Sorting :: itemBaseCount: {itemBaseCount}");
        Tween nodesMoverTween = null;

        IterateAndMoveNodesUsingDictionary(currentNode, itemBaseCount, ref nodesMoverTween);
    }
}
