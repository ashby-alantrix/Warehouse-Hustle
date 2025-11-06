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

    private Queue<Tween> currentTweeners = new Queue<Tween>();
    private Node prevSelectedNode = null, currentSelectedNode = null;

    private bool isLastKey = false;
    public bool isInitialized = false;
    private int tweeners = 3;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsSortingManager>(this);
    }

    public Node GetCurrentSelectedNode() => currentSelectedNode;

    public void CheckNeighbors(Node selectedNode, bool useDebug = false)
    {
        Debug.Log($"Test4 --------------------------------------");
        Debug.Log($"Test4 CheckNeighbors of selectedNode.position: {selectedNode.transform.position}, {selectedNode.transform.name}");

        if (!isInitialized)
        {
            prevSelectedNode = selectedNode;
            isInitialized = true;
        }
        else
            prevSelectedNode = currentSelectedNode;
            
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
                if (neighborNode.CheckIfSetItemMatches(currentItemKey, out goodsCount))
                {
                    if (goodsCount == neighborNode.GetTotalSlotsInNode())
                        continue;

                    itemsToMove = currentSelectedNode.GetGoodsSetCount(currentItemKey);
                    Debug.Log($"CheckIfSetItemsMatchesWithNeighbor :: itemType: {currentItemKey}, itemsCountInNeighbor: {itemsToMove}");
                    isLastKey = neighborNode.IsLastKey(currentItemKey);

                    if (neighborNode.HasEmptySlots(out availSlots))
                    {
                        itemsToMove = itemsToMove > availSlots ? availSlots : itemsToMove;
                        MoveMatchedSetToNeighbor(currentItemKey, neighborNode, itemsToMove);
                    }

                    // need to rearrange
                    currentSelectedNode.SortItemBases();
                    neighborNode.SortItemBases();

                    goodsPlacementManager.RearrangeBasedOnSorting(currentSelectedNode); // do on tween completion if needed
                    goodsPlacementManager.RearrangeBasedOnSorting(neighborNode); // do on tween completion if needed
                    //goodsPlacementManager.RearrangeBasedOnSorting(neighborNode); // do on tween completion

                    Debug.Log($"IsLastKey: {isLastKey}");
                    UpdateSlotStates(neighborNode);
                    if (!isLastKey) // add a better check for recursive calls
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

    private void MoveMatchedSetToNeighbor(ItemType itemType, Node neighborNode, int itemsCountInNeighbor)
    {
        Debug.Log($"MoveMatchedSetToNeighbor {itemType}, {neighborNode.transform.name}, {itemsCountInNeighbor}");
        currentSelectedNode.RemoveItemsDataFromNode(itemType, itemsCountInNeighbor);
        neighborNode.AddItemsDataToNode(itemType, itemsCountInNeighbor);

        if (currentTweeners.Count > tweeners)
            currentTweener = currentTweeners.Dequeue();

        goodsPlacementManager.RearrangeGoodsBetweenSelectedNodeAndNeighbor(itemType, neighborNode, currentSelectedNode); //, out currentTweener);

        for (int indexJ = 0; indexJ < itemsCountInNeighbor; indexJ++)
        {
            ItemBase removedItem = currentSelectedNode.RemoveFromItemBasesCollection(itemType);
            if (removedItem)
                neighborNode.AddToItemBasesCollection(removedItem);
        }

        // currentTweener.OnComplete(() =>
        // {
        //     Debug.Log($"MoveMatchedSetToNeighbor OnComplete");
        //     Debug.Log($"MoveMatchedSetToNeighbor OnComplete: currentSelectedNode :: name: {currentSelectedNode.transform.name}, pos: {currentSelectedNode.transform.position}");
        //     Debug.Log($"MoveMatchedSetToNeighbor OnComplete: neighborNode :: name: {neighborNode.transform.name}, pos: {neighborNode.transform.position}");

        //     currentSelectedNode.CheckIfNodeIsFullOrCleared();
        //     neighborNode.CheckIfNodeIsFullOrCleared();
        // });

        AddTweenToQueue();

        void AddTweenToQueue()
        {
            currentTweeners.Enqueue(currentTweener);
        }

        Tween DequeueTween()
        {
            return currentTweeners.Dequeue();
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
