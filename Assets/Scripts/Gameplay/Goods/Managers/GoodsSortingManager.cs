using System.Dynamic;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Unity.VisualScripting;

public class GoodsSortingManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private float sortingDelay = 0.75f;
    private NodeManager nodeManager;
    private GoodsPlacementManager goodsPlacementManager;

    private Node currentSelectedNode = null;
    private Dictionary<ItemType, List<string>> connectedNodesDict = new Dictionary<ItemType, List<string>>();

    private bool foundDifferentKey = false;
    public bool isInitialized = false;

    private Node firstNode = null, secondNode = null;
    private int currentAvailSlots = 0, itemsToMove = 0, cacheCount = 0;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsSortingManager>(this);
    }

    public Node GetCurrentSelectedNode() => currentSelectedNode;

    public void CheckNeighbors(Node selectedNode)
    {
        SetNodeManager();
        SetGoodsPlacementManager();
        currentSelectedNode = selectedNode;

        var currentNodesSetKeys = selectedNode.GetSetKeys();
        // foreach (var key in currentNodesSetKeys)
        //     Debug.Log($"each key: {key}");

        foreach (var key in currentNodesSetKeys) // . // o // *
        {
            InitializeConnectedNodesForItem(key);
        }

        // Debug.Log($"#### :: Initialized connected nodes for items");

        // foreach (var set in connectedNodesDict)
        // {
        //     Debug.Log($"#### :: set.key: {set.Key}");
        //     foreach (var str in connectedNodesDict[set.Key])
        //     {
        //         if (nodeManager.IsNodeAvailableInGrid(str, out Node node))
        //             Debug.Log($"#### :: set.value: {str}, nodeName: {node.name}");
        //     }
        // }

        foreach (var key in currentNodesSetKeys)
        {
            CheckConnectedNodes(key);
        }

        foreach (var connectedNodeSet in connectedNodesDict)
        {
            Debug.Log($"connectedNodeSet after update: {connectedNodeSet.Key}, {connectedNodesDict[connectedNodeSet.Key].Count}");
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

        foreach (var key in connectedNodesDict.Keys)
        {
            Debug.Log($"ConnectedNodesData: For key {key}");
            foreach (var nodeValStr in connectedNodesDict[key])
            {
                nodeManager.IsNodeAvailableInGrid(nodeValStr, out Node node);
                Debug.Log($"ConnectedNodesData: For key {key}, value: {nodeValStr}, node: {node.name}");
            }
        }
    }

    public void StoreConnectedNodesForEachType(ItemType setItemKey, string nodePosStr)
    {
        if (!connectedNodesDict.ContainsKey(setItemKey))
        {
            Debug.Log($"connectedNodesDict: {connectedNodesDict.ContainsKey(setItemKey)}");
            connectedNodesDict.Add(setItemKey, new List<string>() { nodePosStr });
        }
        else if (!connectedNodesDict[setItemKey].Contains(nodePosStr))
        {
            connectedNodesDict[setItemKey].Add(nodePosStr);
        }
    }

    private void CheckIfCachedDataIsLeft(Node source, ItemType cachedKey)
    {
        if (!source.HasCachedItemType(cachedKey))
            return;

        int availSlots = 0;
        Node foundNeighbor = null;
        if (source.HasCachedData() && source.HasEmptySlots(out availSlots) && availSlots == source.GetCachedData(cachedKey))
        {
            UpdateNodeWithCachedData(cachedKey, source: source, target: source);
        }
        else if (GetMatchingNeighborWithAvailSlots(cachedKey, source, out foundNeighbor, out availSlots))
        {
            UpdateNodeWithCachedData(cachedKey, source: source, target: foundNeighbor);
            var cacheDataLen = source.GetCachedData(cachedKey) - availSlots;
            if (cacheDataLen > 0)
            {
                CheckIfCachedDataIsLeft(source, cachedKey);
            }
        }
        else
        {
            if (FindEmptyNeighbor(source, out foundNeighbor))
            {
                UpdateNodeWithCachedData(cachedKey, source: source, target: foundNeighbor);
            }
            else 
            {
                if (FindNewNeighborWithEmptySlots(source,  neighbor: out foundNeighbor, out availSlots))
                {
                    UpdateNodeWithCachedData(cachedKey, source: source, target: foundNeighbor);
                    var cacheDataLen = source.GetCachedData(cachedKey) - availSlots;
                    if (cacheDataLen > 0)
                    {
                        CheckIfCachedDataIsLeft(source, cachedKey);
                    }
                }
            }
        }
    }

    private bool GetMatchingNeighborWithAvailSlots(ItemType cachedKey, Node source, out Node neighbor, out int availSlots)
    {
        neighbor = null;
        availSlots = 0;
        var neigbhorsCount = source.GetNeighborsCount();

        for (int indexI = 0; indexI < neigbhorsCount; indexI++)
        {
            if (nodeManager.IsNodeAvailableInGrid($"{source.GetNeighborHexOffset(indexI)}", out neighbor))
            {
                if (neighbor.HasGoodsSet(cachedKey) && neighbor.HasEmptySlots(out availSlots))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool FindEmptyNeighbor(Node source, out Node neighbor)
    {
        neighbor = null;
        var neigbhorsCount = source.GetNeighborsCount();

        for (int indexI = 0; indexI < neigbhorsCount; indexI++)
        {
            if (nodeManager.IsNodeAvailableInGrid($"{source.GetNeighborHexOffset(indexI)}", out neighbor))
            {
                if (neighbor.IsEmpty())
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool FindNewNeighborWithEmptySlots(Node source, out Node neighbor, out int availSlots)
    {
        neighbor = null;
        availSlots = 0;
        var neigbhorsCount = source.GetNeighborsCount();

        for (int indexI = 0; indexI < neigbhorsCount; indexI++)
        {
            if (nodeManager.IsNodeAvailableInGrid($"{source.GetNeighborHexOffset(indexI)}", out neighbor))
            {
                if (neighbor.HasEmptySlots(out availSlots))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void CheckConnectedNodes(ItemType currentSetItemKey)
    {
        Debug.Log($"::: CheckConnectedNodes : {currentSetItemKey}");
        if (!connectedNodesDict.ContainsKey(currentSetItemKey))
        {
            Debug.LogError($"Connected nodes dictionary doesn't contain setItemKey: {currentSetItemKey}");
            return;
        }

        Debug.Log($"::: connectedNodesDict.ContainsKey(currentSetItemKey): {connectedNodesDict.ContainsKey(currentSetItemKey)}");
        Debug.Log($"::: connectedNodesDict[currentSetItemKey].Count: {connectedNodesDict[currentSetItemKey].Count}");
        if (connectedNodesDict.ContainsKey(currentSetItemKey) && connectedNodesDict[currentSetItemKey].Count <= 1)
        {
            if (firstNode && firstNode.HasCachedData())
                foreach (var key in firstNode.GetCachedKeys())
                    CheckIfCachedDataIsLeft(firstNode, key);

            if (secondNode && secondNode.HasCachedData())
                foreach (var key in secondNode.GetCachedKeys())
                    CheckIfCachedDataIsLeft(secondNode, key);

            connectedNodesDict[currentSetItemKey].Clear();
            Debug.Log($"::: clearing connectedNodesDict[currentSetItemKey] for {currentSetItemKey}");
            return;
        }

        Debug.Log($"::: skipped clearing of connectedNodesDict[currentSetItemKey]");

        nodeManager.IsNodeAvailableInGrid(connectedNodesDict[currentSetItemKey][0], out firstNode);
        nodeManager.IsNodeAvailableInGrid(connectedNodesDict[currentSetItemKey][1], out secondNode);

        if (firstNode.HasEmptySlots(out currentAvailSlots) && firstNode.GetSetKeys().Count < secondNode.GetSetKeys().Count) // if firstNode has slots THEN 
        {
            Debug.Log($"1st condition :: First node has empty slots: {firstNode.name}, {currentAvailSlots}");
            itemsToMove = secondNode.GetGoodsSetCountForSpecificItem(currentSetItemKey);
            itemsToMove = itemsToMove > currentAvailSlots ? currentAvailSlots : itemsToMove;
            Debug.Log($"itemsToMove: {itemsToMove}");

            if (itemsToMove > 0)
            {
                MoveMatchedSetFromSourceToTarget(currentSetItemKey, sourceNode: secondNode, targetNode: firstNode, itemsToMove); // tween op
                SortAndRearrangeFirstAndSecondNode(); // tween op
            }

            // DO the logic below only after the tweening is complete from sorting
            if (firstNode.HasCachedData() && secondNode.HasCachedDataRef(firstNode.GetCachedKeys(), out ItemType foundKey))
            {
                Debug.Log($"Has cached data");
                UpdateNodeWithCachedData(foundKey, source: firstNode, target: secondNode);
                // UpdateSecondNodeWithCachedData(foundKey); 
            }
        }
        else if (secondNode.HasEmptySlots(out currentAvailSlots))
        {
            Debug.Log($"2nd condition :: Second node has empty slots: {secondNode.name}");
            itemsToMove = firstNode.GetGoodsSetCountForSpecificItem(currentSetItemKey);
            itemsToMove = itemsToMove > currentAvailSlots ? currentAvailSlots : itemsToMove;

            Debug.Log($"ItemsToMove: {itemsToMove}");
            if (itemsToMove > 0)
            {
                MoveMatchedSetFromSourceToTarget(currentSetItemKey, sourceNode: firstNode, targetNode: secondNode, itemsToMove);
                SortAndRearrangeFirstAndSecondNode();
            }

            if (secondNode.HasCachedData() && firstNode.HasCachedDataRef(secondNode.GetCachedKeys(), out ItemType foundKey))
            {
                UpdateNodeWithCachedData(foundKey, source: secondNode, target: firstNode);
                // UpdateFirstNodeWithCachedData(foundKey); 
            }
        }
        else if (firstNode.GetSetKeys().Count > 1 && firstNode.GetNextKeyAfterCurrent(currentSetItemKey, out ItemType otherSetItemKey) && secondNode.HasGoodsSet(currentSetItemKey)) // swapping scenario when multiple keys are involved
        {
            Debug.Log($"3rd condition, otherSetItemKey: {otherSetItemKey}, firstNode: {firstNode.name}");
            Debug.Log($"3rd condition, setcount: {firstNode.GetSetKeys().Count}");

            cacheCount = Mathf.Min(firstNode.GetGoodsSetCountForSpecificItem(otherSetItemKey), secondNode.GetGoodsSetCountForSpecificItem(currentSetItemKey));

            firstNode.StoreCachedData(otherSetItemKey, cacheCount);
            firstNode.FreeUpGoodsSet(otherSetItemKey, cacheCount);
            firstNode.SortItemsData();

            firstNode.CacheAndStoreItemBases(otherSetItemKey, cacheCount);
            firstNode.SortItemBases();

            goodsPlacementManager.RearrangeBasedOnSorting(firstNode); // tween op
            firstNode.UpdateOccupiedSlotsState();
        }
        else if (GetFirstOtherMatchingKeyBetweenNodes(currentSetItemKey, out ItemType otherMatchingItemKey) 
                && secondNode.HasEmptySlots(out int availSlots) && availSlots == firstNode.GetGoodsSetCountForSpecificItem(otherMatchingItemKey)) // recursing for another key if firstNode runs out of empty slots
        {
            Debug.Log($"4th condition :: Found matching key between sets");
            CheckConnectedNodes(otherMatchingItemKey);
        }
        else if (secondNode.GetNextKeyAfterCurrent(currentSetItemKey, out ItemType otherSetItemKey1) && firstNode.HasGoodsSet(currentSetItemKey)) // TODO :: Double check this case, could be useful if both first and second node is full
        {
            Debug.LogError($"5th condition: CheckConnectedNodes :: double check this logic...");
        }
        else
        {
            Debug.LogError($"6th condition: CheckConnectedNodes :: no op...");
        }

        UpdateConnectedNodeStates(currentSetItemKey);
        CheckConnectedNodes(currentSetItemKey);
    }

    private bool GetFirstOtherMatchingKeyBetweenNodes(ItemType currentSetItemKey, out ItemType otherItemKey)
    {
        otherItemKey = ItemType.MAX;

        foreach (var firstNodeKey in firstNode.GetSetKeys())
        {
            if (firstNodeKey == currentSetItemKey) continue;

            foreach (var secondNodeKey in secondNode.GetSetKeys())
            {
                if (secondNodeKey == currentSetItemKey) continue;

                if (firstNodeKey == secondNodeKey)
                {
                    otherItemKey = firstNodeKey; // TODO :: What if the matching key is already checked between the nodes
                    return true;
                }
            }
        }

        return false;
    }

    private void SortAndRearrangeFirstAndSecondNode()
    {
        firstNode.SortItemBases();
        secondNode.SortItemBases();

        goodsPlacementManager.RearrangeBasedOnSorting(firstNode); // do on tween completion if needed
        goodsPlacementManager.RearrangeBasedOnSorting(secondNode);

        firstNode.UpdateOccupiedSlotsState();
        secondNode.UpdateOccupiedSlotsState();
    }

    private void UpdateNodeWithCachedData(ItemType cachedKey, Node source, Node target)
    {
        int cacheCount = source.GetCachedData(cachedKey);
        if (target.HasEmptySlots(out int availSlots))
        {
            Debug.Log($"Second node availSlots: {availSlots}, cachedKey: {cachedKey}");
            availSlots = cacheCount > availSlots ? availSlots : cacheCount;
            Debug.Log($"updated availSlots: {availSlots}");

            source.RemoveItemsDataFromCachedData(cachedKey, availSlots);

            Debug.Log($"before goods update: " + target.GetGoodsSetCountForSpecificItem(cachedKey));
            target.AddItemsDataToNode(cachedKey, availSlots);
            Debug.Log($"after goods update: " + target.GetGoodsSetCountForSpecificItem(cachedKey));

            goodsPlacementManager.RearrangeGoodsBetweenSelectedNodeAndNeighbor(cachedKey, target: target, source: source, hasCachedKey: true); // rearranging using item bases

            for (int indexJ = 0; indexJ < availSlots; indexJ++)
            {
                ItemBase removedItem = source.RemoveAndRetrieveFromCachedItemBases(cachedKey);
                if (removedItem)
                    target.AddToItemBasesCollection(removedItem);
            }

            target.SortItemsData();
            target.SortItemBases();
        }
    }

    private void UpdateFirstNodeWithCachedData(ItemType cachedKey)
    {
        int cacheCount = secondNode.GetCachedData(cachedKey);
        if (firstNode.HasEmptySlots(out int availSlots))
        {
            availSlots = cacheCount > availSlots ? availSlots : cacheCount;

            secondNode.RemoveItemsDataFromCachedData(cachedKey, availSlots);
            firstNode.AddItemsDataToNode(cachedKey, availSlots);

            goodsPlacementManager.RearrangeGoodsBetweenSelectedNodeAndNeighbor(cachedKey, target: firstNode, source: secondNode, hasCachedKey: true); // rearranging using item bases

            for (int indexJ = 0; indexJ < availSlots; indexJ++)
            {
                ItemBase removedItem = secondNode.RemoveAndRetrieveFromCachedItemBases(cachedKey);
                if (removedItem)
                    firstNode.AddToItemBasesCollection(removedItem);
            }

            firstNode.SortItemsData();
            firstNode.SortItemBases();
        }
    }

    private void UpdateSecondNodeWithCachedData(ItemType cachedKey)
    {
        int cacheCount = firstNode.GetCachedData(cachedKey);
        if (secondNode.HasEmptySlots(out int availSlots))
        {
            Debug.Log($"Second node availSlots: {availSlots}, cachedKey: {cachedKey}");
            availSlots = cacheCount > availSlots ? availSlots : cacheCount;
            Debug.Log($"updated availSlots: {availSlots}");

            firstNode.RemoveItemsDataFromCachedData(cachedKey, availSlots);

            Debug.Log($"before goods update: " + secondNode.GetGoodsSetCountForSpecificItem(cachedKey));
            secondNode.AddItemsDataToNode(cachedKey, availSlots);
            Debug.Log($"after goods update: " + secondNode.GetGoodsSetCountForSpecificItem(cachedKey));

            goodsPlacementManager.RearrangeGoodsBetweenSelectedNodeAndNeighbor(cachedKey, target: secondNode, source: firstNode, hasCachedKey: true); // rearranging using item bases

            for (int indexJ = 0; indexJ < availSlots; indexJ++)
            {
                ItemBase removedItem = firstNode.RemoveAndRetrieveFromCachedItemBases(cachedKey);
                if (removedItem)
                    secondNode.AddToItemBasesCollection(removedItem);
            }

            secondNode.SortItemsData();
            secondNode.SortItemBases();
        }
    }
    
    private void UpdateConnectedNodeStates(ItemType currentSetItemKey)
    {
        Debug.Log($"#### :: currentSetItemKey: {currentSetItemKey}");

        int indexer = 0;
        Node currentNode = null;
        bool isFilled = false, nodeHasNoItem = false, isNodeEmpty = false;
        string[] currentSetKeys = new string[connectedNodesDict[currentSetItemKey].Count];

        foreach (var nodePosStr in connectedNodesDict[currentSetItemKey])
        {
            if (nodeManager.IsNodeAvailableInGrid(nodePosStr, out currentNode))
            {
                isFilled = currentNode.IsNodeFilled();
                nodeHasNoItem = !currentNode.HasGoodsSet(currentSetItemKey);
                isNodeEmpty = currentNode.GetTotalGoodsSetsCount() == 0;

                if (isFilled || nodeHasNoItem || isNodeEmpty)
                {
                    Debug.Log($"UpdateConnectedNodeStates for {currentNode.name}: {isFilled}, {nodeHasNoItem}, {isNodeEmpty}");
                    currentSetKeys[indexer++] = nodePosStr;

                    UpdateConnectedStateInOtherNodes(nodeHasNoItem, isNodeEmpty, nodePosStr);
                }
            }
        }

        for (int indexI = 0; indexI < currentSetKeys.Length; indexI++)
        {
            if (currentSetKeys[indexI] == null || string.IsNullOrEmpty(currentSetKeys[indexI])) continue;

            connectedNodesDict[currentSetItemKey].Remove(currentSetKeys[indexI]);
        }

        void UpdateConnectedStateInOtherNodes(bool nodeHasNoItem, bool isNodeEmpty, string nodePosStr)
        {
            if (nodeHasNoItem)
            {
                AddNodesToOtherConnectedTypes(nodePosStr);
            }
            else if (isNodeEmpty)
            {
                RemoveEmptyNodesFromOtherConnectedTypes(nodePosStr);
            }
        }

        // Summary:
        // if other keys are moved to a new node, the new node with those keys have to updated to dictionary 
        // for connecting the new node for that specific item type
        void AddNodesToOtherConnectedTypes(string nodePosStr)
        {
            foreach (var key in connectedNodesDict.Keys)
            {
                if (key == currentSetItemKey) continue;
                
                nodeManager.IsNodeAvailableInGrid(nodePosStr, out var newFoundNode);
                if (connectedNodesDict[key].Count > 0 && newFoundNode.HasGoodsSet(key) && !connectedNodesDict[key].Contains(nodePosStr))
                {
                    connectedNodesDict[key].Add(nodePosStr);
                    Debug.Log($"adding node: {newFoundNode.name} to connectedNodesDict for key: {key}");
                }
            }
        }

        void RemoveEmptyNodesFromOtherConnectedTypes(string nodePosStr)
        {
            foreach (var key in connectedNodesDict.Keys)
            {
                if (key == currentSetItemKey) continue;
                
                if (connectedNodesDict[key].Contains(nodePosStr))
                    connectedNodesDict[key].Remove(nodePosStr);
            }
        }
    }

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

                    var goodsCountInSelectedNode = currentSelectedNode.GetGoodsSetCountForSpecificItem(currentItemKey);
                    var goodsCountInNeighbor = neighborNode.GetGoodsSetCountForSpecificItem(currentItemKey);

                    Debug.Log($"currentSelectedNode.GetSetKeysCount(): {currentSelectedNode.GetSetKeysCount()}, neighborNode.GetSetKeysCount(): {neighborNode.GetSetKeysCount()}");
                    if (currentSelectedNode.GetSetKeysCount() <= neighborNode.GetSetKeysCount()) // || goodsCountInSelectedNode > goodsCountInNeighbor)
                    {
                        var slotsRemaining = currentSelectedNode.GetTotalSlotsInNode() - currentSelectedNode.GetGoodsSetCountForSpecificItem(currentItemKey);

                        Debug.Log($"current selected node has less keys");
                        if (currentSelectedNode.HasEmptySlots(out availSlots))// && slotsRemaining == availSlots)
                        {
                            Debug.Log($"current selected node has empty slots");
                            itemsToMove = neighborNode.GetGoodsSetCountForSpecificItem(currentItemKey);
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
                            itemsToMove = currentSelectedNode.GetGoodsSetCountForSpecificItem(currentItemKey);
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

                    // goodsPlacementManager.RearrangeBasedOnSorting(currentSelectedNode); // do on tween completion if needed
                    // goodsPlacementManager.RearrangeBasedOnSorting(neighborNode); // do on tween completion if needed
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
                    Debug.Log($"CheckItemBases selectedNode.ItemBases: {currentSelectedNode.GetTotalGoodsSetsCount()}");
                    Debug.Log($"CheckItemBases neighborNode.ItemBases: {neighborNode.GetTotalGoodsSetsCount()}");

                }
            }
        }
    }
    #endregion 

    private void UpdateSlotStates(Node neighborNode)
    {
        // currentSelectedNode.UpdateOccupiedSlotsState();
        // neighborNode.UpdateOccupiedSlotsState();
    }

    private Tween currentTweener = null;

    private void SetNodeManager()
    {
        nodeManager = nodeManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<NodeManager>() : nodeManager;
    }
    
    private void SetGoodsPlacementManager()
    {
        goodsPlacementManager = goodsPlacementManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsPlacementManager>() : goodsPlacementManager;
    }
}
