using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using UnityEngine;

public class GoodsSortingManager : MonoBehaviour, IBase, IBootLoader
{
    private NodeManager nodeManager;
    private GoodsPlacementManager goodsPlacementManager;

    private Tween currentTweener = null;
    private Node currentSelectedNode = null;

    private bool isLastKey = false;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsSortingManager>(this);
    }

    public void CheckNeighbors(Node selectedNode, bool useDebug = false)
    {
        Debug.Log($"Test4 --------------------------------------");
        Debug.Log($"Test4 CheckNeighbors");
        currentSelectedNode = selectedNode;
        SetNodeManager();
        SetGoodsPlacementManager();
        var setKeys = selectedNode.GetSetKeys();

        Debug.Log($"Recursion :: setKeysCount: {setKeys.Count}");
        foreach (var key in setKeys)
            Debug.Log($"Recursion :: key: {key}");

        foreach (var key in setKeys)
        {
            ExploreNeighbors(key);

            Debug.Log($"Recursion :: {key}");
        }
    }

    private void ExploreNeighbors(ItemType itemType)
    {
        int neighborsCount = currentSelectedNode.GetNeighborsCount();

        for (int index = 0; index < neighborsCount; index++)
        {
            // Debug.Log($"### Test4: Neighbor index: " + index);
            SetNodeManager();

            var isNeighborsNodeAvailable = nodeManager.IsNeighborNodeAvailableInGrid(currentSelectedNode.GetNeighborHexOffset(index).ToString(), out Node neighborNode);
            Debug.Log($"Test4 IsNeighborNodeAvailable: {isNeighborsNodeAvailable}");
            if (isNeighborsNodeAvailable)
            {
                Debug.Log($"Test4 IsNeighborNodeAvailable :: index: {index}, position: {neighborNode.transform.position}");
                if (neighborNode.CheckIfSetItemsMatchesWithNeighbor(itemType, out int itemsCountInNeighbor))
                {
                    Debug.Log($"Test4 CheckIfSetItemsMatchesWithNeighbor :: itemType: {itemType}, itemsCountInNeighbor: {itemsCountInNeighbor}");
                    isLastKey = neighborNode.IsLastKey(itemType);
                    
                    MoveMatchedSetToNeighbor(itemType, neighborNode, itemsCountInNeighbor);

                    currentSelectedNode.SortItemBases(); // need to rearrange

                    goodsPlacementManager.RearrangeBasedOnSorting(currentSelectedNode); // do on tween completion if needed
                    //goodsPlacementManager.RearrangeBasedOnSorting(neighborNode); // do on tween completion

                    Debug.Log($"Test4IsLastKey: {isLastKey}");
                    if (!isLastKey)
                    {
                        Debug.Log("Recursion: calling recursive function");
                        CheckNeighbors(neighborNode, true); // 1st call left here 
                    }

                    Debug.Log($"Test4: selectedNode.ItemBases: {currentSelectedNode.GetItemBaseCount()}");
                    Debug.Log($"Test4: neighborNode.ItemBases: {neighborNode.GetItemBaseCount()}");

                    currentSelectedNode.UpdateOccupiedSlotsState();
                    neighborNode.UpdateOccupiedSlotsState();
                }
            }
        }
    }

    private void MoveMatchedSetToNeighbor(ItemType itemType, Node neighborNode, int itemsCountInNeighbor)
    {
        currentSelectedNode.RemoveItemsDataFromNode(itemType, itemsCountInNeighbor);
        neighborNode.AddItemsDataToNode(itemType, itemsCountInNeighbor);

        goodsPlacementManager.RearrangeGoodsBetweenSelectedNodeAndNeighbor(itemType, neighborNode, currentSelectedNode, out currentTweener);

        for (int indexJ = 0; indexJ < itemsCountInNeighbor; indexJ++)
        {
            ItemBase removedItem = currentSelectedNode.RemoveFromItemBasesCollection(itemType);
            if (removedItem)
                neighborNode.AddToItemBasesCollection(removedItem);
        }

        currentTweener?.OnComplete(() =>
        {
            neighborNode.CheckIfNodeIsFullOrCleared();
            currentSelectedNode.CheckIfNodeIsFullOrCleared();
        });
    }

    private void SetNodeManager()
    {
        nodeManager = nodeManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<NodeManager>() : nodeManager;
    }
    
    private void SetGoodsPlacementManager()
    {
        goodsPlacementManager = goodsPlacementManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsPlacementManager>() : goodsPlacementManager;
    }
}
