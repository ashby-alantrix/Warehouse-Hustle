using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GoodsSortingManager : MonoBehaviour, IBase, IBootLoader
{
    private NodeManager nodeManager;
    private GoodsPlacementManager goodsPlacementManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsSortingManager>(this);
    }

    public void CheckNeighbors(Node selectedNode)
    {
        Debug.Log($"### Test4 CheckNeighbors");
        SetNodeManager();
        SetGoodsPlacementManager();
        var setKeys = selectedNode.GetSetKeys();

        // try
        // {
            foreach (var key in setKeys)
            {
                ExploreNeighbors(selectedNode, (ItemType)key);
            }
        // }
        // catch (Exception ex)
        // {
        //     Debug.LogError($"Caught exception: {ex.Message}");
        // }
    }

    private void ExploreNeighbors(Node selectedNode, ItemType itemType)
    {
        int neighborsCount = selectedNode.GetNeighborsCount();
        Debug.Log($"### Test4 ExploreNeighbors: " + neighborsCount);

        int availSlots = 0;
        for (int index = 0; index < neighborsCount; index++)
        {
            // Debug.Log($"### Test4: Neighbor index: " + index);
            SetNodeManager();

            // Debug.Log($"### Test4 GetNeighborHexOffsetLength: {selectedNode.GetNeighborHexOffsetLength()}");

            var isNeighborsNodeAvailable = nodeManager.IsNeighborNodeAvailableInGrid(selectedNode.GetNeighborHexOffset(index).ToString(), out Node neighborNode);
            Debug.Log($"### Test4: isNeighborsNodeAvailable: {isNeighborsNodeAvailable}");
            if (isNeighborsNodeAvailable)
            {
                Debug.Log($"### Test4 IsNeighborNodeAvailable :: index: {index}, position: {neighborNode.transform.position}");
                if (neighborNode.DoesNeighborHaveSimilarItem(itemType, out int itemsCountInNeighbor))
                {
                    Debug.Log($"### Test4 DoesNeighborHaveSimilarItem: {itemType}, itemsCountInNeighbor: {itemsCountInNeighbor}");
                    // move the nodes to the neighbor: (use a recursion based approach)
                    //  -> case 1: if there are slots already available in the current node then take 
                    //             the matching nodes and move them to current node followed by 
                    //             sorting/refreshing the neighboring node and the current node
                    //  -> case 2: if there are no slots available, check the neighbor's neighbors to see if there are matching items (using recursion)

                    if (selectedNode.HasEmptySlots(out availSlots) && itemsCountInNeighbor <= availSlots)
                    {
                        Debug.Log($"### Test4 HasEmptySlots: availSlots: {availSlots}");
                        // update datas:
                        //  -> update the goods data set in both nodes ||
                        //  -> update the goods items collection (item bases) in both nodes 
                        // change the occupied props

                        neighborNode.RemoveItemsDataFromNode(itemType, itemsCountInNeighbor);
                        selectedNode.AddItemsDataToNode(itemType, itemsCountInNeighbor);

                        goodsPlacementManager.RearrangeGoodsBetweenSelectedNodeAndNeighbor(itemType, selectedNode, neighborNode);
                        
                        for (int indexJ = 0; indexJ < itemsCountInNeighbor; indexJ++)
                        {
                            ItemBase removedItem = neighborNode.RemoveFromItemBasesCollection(itemType);
                            selectedNode.AddToItemBasesCollection(removedItem);
                        }
                    }
                }
            }
        }
    }
    
    private void SortAndRefreshNode()
    {
        
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
