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

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsSortingManager>(this);
    }

    public void CheckNeighbors(Node selectedNode)
    {
        Debug.Log($"Test4 --------------------------------------");
        Debug.Log($"Test4 CheckNeighbors");
        currentSelectedNode = selectedNode;
        SetNodeManager();
        SetGoodsPlacementManager();
        var setKeys = selectedNode.GetSetKeys();

        foreach (var key in setKeys)
        {
            ExploreNeighbors(key);
        }
    }

    private bool isLastKey = false;

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
                    isLastKey = neighborNode.IsLastKey(itemType);
                    MoveMatchedSetToCurrentNode(itemType, neighborNode, itemsCountInNeighbor);
                    currentSelectedNode.SortItemBases(); // need to rearrange
                    goodsPlacementManager.RearrangeBasedOnSorting(currentSelectedNode);
                    
                    Debug.Log($"Test4IsLastKey: {isLastKey}");
                    if (!isLastKey)
                    {
                        CheckNeighbors(neighborNode);
                    }

                    Debug.Log($"Test4: selectedNode.ItemBases: {currentSelectedNode.GetItemBaseCount()}");
                    Debug.Log($"Test4: neighborNode.ItemBases: {neighborNode.GetItemBaseCount()}");
                }
            }
        }
    }

    private void MoveMatchedSetToCurrentNode(ItemType itemType, Node neighborNode, int itemsCountInNeighbor)
    {
        neighborNode.RemoveItemsDataFromNode(itemType, itemsCountInNeighbor);
        currentSelectedNode.AddItemsDataToNode(itemType, itemsCountInNeighbor);

        goodsPlacementManager.RearrangeGoodsBetweenSelectedNodeAndNeighbor(itemType, currentSelectedNode, neighborNode, out currentTweener);

        for (int indexJ = 0; indexJ < itemsCountInNeighbor; indexJ++)
        {
            ItemBase removedItem = neighborNode.RemoveFromItemBasesCollection(itemType);
            currentSelectedNode.AddToItemBasesCollection(removedItem);
        }

        currentTweener?.OnComplete(() =>
        {
            currentSelectedNode.CheckIfNodeIsFullOrCleared();
            neighborNode.CheckIfNodeIsFullOrCleared();
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
