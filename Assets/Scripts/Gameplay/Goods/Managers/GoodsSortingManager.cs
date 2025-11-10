using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class GoodsSortingManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private float sortingDelay = 0.75f;
    private NodeManager nodeManager;
    private GoodsPlacementManager goodsPlacementManager;

    private Node currentSelectedNode = null;

    private bool foundDifferentKey = false;
    public bool isInitialized = false;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsSortingManager>(this);
    }

    public Node GetCurrentSelectedNode() => currentSelectedNode;

    public void CheckNeighbors(Node selectedNode, bool useDebug = false)
    {
        Debug.Log($"Test4 --------------------------------------");
        Debug.Log($"Test4 CheckNeighbors of selectedNode.position: {selectedNode.transform.position}, {selectedNode.transform.name}");

        currentSelectedNode = selectedNode;
        
        SetNodeManager();
        SetGoodsPlacementManager();

        var setKeys = selectedNode.GetSetKeys();
        Debug.Log($"setKeysCount: {setKeys.Count}");
        foreach (var key in setKeys)
            Debug.Log($"Keys Log :: key: {key}");

        foreach (var key in setKeys)
        {
            Debug.Log($"setKey :: {key}");
            ExploreNeighbors(key);
        }
    }

    private void ExploreNeighbors(ItemType currentItemKey)
    {
        int neighborsCount = currentSelectedNode.GetNeighborsCount();
        int itemsToMove, goodsCount = 0, availSlots = 0;

        for (int index = 0; index < neighborsCount; index++)
        {
            if (!currentSelectedNode.HasGoodsSet(currentItemKey))
                continue;

            // Debug.Log($"### Test4: Neighbor index: " + index);
            SetNodeManager();

            Debug.Log($"currentSelectedNode.name: {currentSelectedNode.transform.name}");
            Debug.Log($"NeighborsCount: {currentSelectedNode.GetNeighborsCount()}");
            Debug.Log($"NeighborsIndex: {index}");
            var isNeighborsNodeAvailable = nodeManager.IsNeighborNodeAvailableInGrid(currentSelectedNode.GetNeighborHexOffset(index).ToString(), out Node neighborNode);
            Debug.Log($"IsNeighborNodeAvailable: {isNeighborsNodeAvailable}");
            if (isNeighborsNodeAvailable)
            {
                Debug.Log($"IsNeighborNodeAvailable :: index: {index}, position: {neighborNode.transform.position}, name: {neighborNode.transform.name}");
                var matchFound = neighborNode.CheckIfSetItemMatches(currentItemKey, out goodsCount);
                Debug.Log($"CheckIfSetItemMatches :: currentItemKey: {currentItemKey}, goodsCount: {goodsCount}");
                if (matchFound)
                {
                    UpdateSlotStates(neighborNode);
                    if (goodsCount == neighborNode.GetTotalSlotsInNode())
                        continue;

                    Debug.Log($"CheckIfSetItemsMatchesWithNeighbor :: itemType: {currentItemKey}");
                    foundDifferentKey = neighborNode.IsThereDifferentKey(currentItemKey);

                    var goodsCountInSelectedNode = currentSelectedNode.GetGoodsSetCount(currentItemKey);
                    var goodsCountInNeighbor = neighborNode.GetGoodsSetCount(currentItemKey);

                    Debug.Log($"currentSelectedNode.GetSetKeysCount(): {currentSelectedNode.GetSetKeysCount()}, neighborNode.GetSetKeysCount(): {neighborNode.GetSetKeysCount()}");
                    if (currentSelectedNode.GetSetKeysCount() <= neighborNode.GetSetKeysCount()) // || goodsCountInSelectedNode > goodsCountInNeighbor)
                    {
                        var slotsRemaining = currentSelectedNode.GetTotalSlotsInNode() - currentSelectedNode.GetGoodsSetCount(currentItemKey);

                        Debug.Log($"current selected node has less keys");
                        if (currentSelectedNode.HasEmptySlots(out availSlots))// && slotsRemaining == availSlots)
                        {
                            Debug.Log($"current selected node has empty slots");
                            itemsToMove = neighborNode.GetGoodsSetCount(currentItemKey);
                            itemsToMove = itemsToMove > availSlots ? availSlots : itemsToMove;
                            MoveMatchedSetFromSourceToTarget(currentItemKey, sourceNode: neighborNode, targetNode: currentSelectedNode, itemsToMove);
                        }
                        else
                        {
                            // if there are still no slots available, try to free up the slots
                            // use a caching system
                            break;
                        }
                    }
                    else
                    {
                        if (neighborNode.HasEmptySlots(out availSlots))
                        {
                            itemsToMove = currentSelectedNode.GetGoodsSetCount(currentItemKey);
                            itemsToMove = itemsToMove > availSlots ? availSlots : itemsToMove;
                            MoveMatchedSetFromSourceToTarget(currentItemKey, sourceNode: currentSelectedNode, targetNode: neighborNode, itemsToMove);
                        }
                        else
                        {
                            // if there are still no slots available, try to free up the slots
                            // use a caching system
                            break;
                        }
                    }
                    

                    currentSelectedNode.SortItemBases();
                    neighborNode.SortItemBases();

                    goodsPlacementManager.RearrangeBasedOnSorting(currentSelectedNode); // do on tween completion if needed
                    goodsPlacementManager.RearrangeBasedOnSorting(neighborNode); // do on tween completion if needed
                    //goodsPlacementManager.RearrangeBasedOnSorting(neighborNode); // do on tween completion

                    Debug.Log($"IsLastKey: {foundDifferentKey}");
                    UpdateSlotStates(neighborNode);
                    if (foundDifferentKey) // TODO :: add a better check for recursive calls
                    {
                        Debug.Log("Recursion: calling recursive function");
                        CheckNeighbors(neighborNode, true); // 1st call left here 
                    }

                    // checking due to recursion

                    neighborsCount = currentSelectedNode.GetNeighborsCount();
                    if (!currentSelectedNode.HasGoodsSet(currentItemKey))
                        continue;

                    Debug.Log($"CheckItemBases selectedNode.ItemBases: {currentSelectedNode.GetItemBaseCount()}");
                    Debug.Log($"CheckItemBases neighborNode.ItemBases: {neighborNode.GetItemBaseCount()}");
                    Debug.Log($"CheckItemBases selectedNode.ItemBases: {currentSelectedNode.GetGoodsSetsCount()}");
                    Debug.Log($"CheckItemBases neighborNode.ItemBases: {neighborNode.GetGoodsSetsCount()}");

                }
            }
        }
    }

    private void UpdateSlotStates(Node neighborNode)
    {
        currentSelectedNode.UpdateOccupiedSlotsState();
        neighborNode.UpdateOccupiedSlotsState();
    }

    private Tween currentTweener = null;

    private void MoveMatchedSetFromSourceToTarget(ItemType itemType, Node sourceNode, Node targetNode, int itemsCountInNeighbor)
    {
        Debug.Log($"MoveMatchedSetToNeighbor {itemType}, {targetNode.transform.name}, {itemsCountInNeighbor}");
        sourceNode.RemoveItemsDataFromNode(itemType, itemsCountInNeighbor);
        targetNode.AddItemsDataToNode(itemType, itemsCountInNeighbor);

        goodsPlacementManager.RearrangeGoodsBetweenSelectedNodeAndNeighbor(itemType, targetNode, sourceNode); //, out currentTweener);

        for (int indexJ = 0; indexJ < itemsCountInNeighbor; indexJ++)
        {
            ItemBase removedItem = sourceNode.RemoveFromItemBasesCollection(itemType);
            if (removedItem)
                targetNode.AddToItemBasesCollection(removedItem);
        }
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
