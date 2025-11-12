using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GoodsSortingManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private float sortingDelay = 0.75f;
    private NodeManager nodeManager;
    private GoodsPlacementManager goodsPlacementManager;

    private Node currentSelectedNode = null;
    private Dictionary<ItemType, List<string>> connectedNodesDict = new Dictionary<ItemType, List<string>>();

    private bool foundDifferentKey = false;
    public bool isInitialized = false;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsSortingManager>(this);
    }

    public Node GetCurrentSelectedNode() => currentSelectedNode;

    public void CheckNeighbors(Node selectedNode)
    {
        SetNodeManager();
        currentSelectedNode = selectedNode;

        var currentNodesSetKeys = selectedNode.GetSetKeys();
        foreach (var key in currentNodesSetKeys)
            Debug.Log($"each key: {key}");

        foreach (var key in currentNodesSetKeys) // . // o // *
        {
            InitializeConnectedNodesForItem(key);
        }

        foreach (var key in currentNodesSetKeys)
        {
            CheckConnectedNodes(key);
        }
    }
    
    private void InitializeConnectedNodesForItem(ItemType setItemKey)
    {
        int neighborsCount = currentSelectedNode.GetNeighborsCount();
        bool isNeighborsNodeAvailable = false;

        StoreConnectedNodesForEachType(setItemKey, currentSelectedNode.GetNodePos());

        for (int indexI = 0; indexI < neighborsCount; indexI++)
        {
            isNeighborsNodeAvailable = nodeManager.IsNodeAvailableInGrid($"{currentSelectedNode.GetNeighborHexOffset(indexI)}", out Node neighborNode);
            if (isNeighborsNodeAvailable && neighborNode.HasGoodsSet(setItemKey))
            {
                StoreConnectedNodesForEachType(setItemKey, $"{currentSelectedNode.GetNeighborHexOffset(indexI)}");
            }
        }
    }

    public void StoreConnectedNodesForEachType(ItemType setItemKey, string nodePosStr)
    {
        if (connectedNodesDict.ContainsKey(setItemKey) && !connectedNodesDict[setItemKey].Contains(nodePosStr))
        {
            connectedNodesDict[setItemKey].Add(nodePosStr);
        }
        else
        {
            connectedNodesDict.Add(setItemKey, new List<string>() { nodePosStr });
        }
    }

    private Node firstNode = null, secondNode = null;
    private int currentAvailSlots = 0, itemsToMove = 0, cacheCount = 0;

    private void CheckConnectedNodes(ItemType currentSetItemKey)
    {

        if (!connectedNodesDict.ContainsKey(currentSetItemKey))
        {
            Debug.LogError($"Connected nodes dictionary doesn't contain setItemKey: {currentSetItemKey}");
            return;
        }

        if (connectedNodesDict.ContainsKey(currentSetItemKey) && connectedNodesDict[currentSetItemKey].Count <= 1)
        {
            connectedNodesDict[currentSetItemKey].Clear();
            return;
        }

        nodeManager.IsNodeAvailableInGrid(connectedNodesDict[currentSetItemKey][0], out firstNode);
        nodeManager.IsNodeAvailableInGrid(connectedNodesDict[currentSetItemKey][1], out secondNode);

        if (firstNode.HasEmptySlots(out currentAvailSlots)) // if firstNode has slots THEN 
        {
            itemsToMove = secondNode.GetGoodsSetCount(currentSetItemKey);
            itemsToMove = itemsToMove > currentAvailSlots ? currentAvailSlots : itemsToMove;

            MoveMatchedSetFromSourceToTarget(currentSetItemKey, sourceNode: secondNode, targetNode: firstNode, itemsToMove);

            if (firstNode.HasCachedData())
            {
                UpdateSecondNodeWithCachedData(firstNode.GetCachedData()); // TODO :: Finish up rem logic
            }
        }
        else if (firstNode.GetNextKeyAfterCurrent(currentSetItemKey, out ItemType otherSetItemKey) && secondNode.HasGoodsSet(currentSetItemKey)) // swapping scenario when multiple keys are involved
        {
            cacheCount = Mathf.Min(firstNode.GetGoodsSetCount(otherSetItemKey), secondNode.GetGoodsSetCount(currentSetItemKey));
            firstNode.StoreCachedData(cacheCount, otherSetItemKey);
            firstNode.FreeUpGoodsSet(cacheCount, otherSetItemKey);
        }
        // else if () // recursing for another key if firstNode runs out of empty slots
        // {

        // }
        else if (secondNode.HasEmptySlots(out currentAvailSlots))
        {
            itemsToMove = firstNode.GetGoodsSetCount(currentSetItemKey);
            itemsToMove = itemsToMove > currentAvailSlots ? currentAvailSlots : itemsToMove;

            MoveMatchedSetFromSourceToTarget(currentSetItemKey, sourceNode: firstNode, targetNode: secondNode, itemsToMove);

            if (secondNode.HasCachedData())
            {
                UpdateFirstNodeWithCachedData(secondNode.GetCachedData()); // TODO :: Finish up rem logic
            }
        }
        else
        {

        }

        UpdateConnectedNodeStates();
    }

    private void UpdateFirstNodeWithCachedData(int cachedData)
    {

    }

    private void UpdateSecondNodeWithCachedData(int cachedData)
    {

    }
    
    private void UpdateConnectedNodeStates()
    {
        // foreach node in connectedNodes
        // var isFilled = node.IsFilled
        // var nodeHasNoItem = !node.Contain(itemKey)
        // var isNodeEmpty = node.IsEmpty()

        // if (isFilled || nodeHasNoItem || isNodeEmpty)
        // {   
        // remove(node) from dictionary for that particular type
        // if (node has an item type which is present in dictionary but not added to hashset) // when nodes are rearranged the items could go on to nodes that weren't previously added onto the connectedNodes list.
        //     add node to hashset for that particular itemType
        // if (isNodeEmpty)
        //     // check the dictionary to see which types have this node in their hashset and if node is present remove it.

        // }

        bool isFilled, nodeHasNoItem, isNodeEmpty;
        foreach (var connectedNodeSet in connectedNodesDict)
        {
            // isFilled = connectedNodeSet
        }
    }

    #region OLD FLOW FOR SORTING AND MERGING
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
            var isNeighborsNodeAvailable = nodeManager.IsNodeAvailableInGrid(currentSelectedNode.GetNeighborHexOffset(index).ToString(), out Node neighborNode);
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
    #endregion 

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
