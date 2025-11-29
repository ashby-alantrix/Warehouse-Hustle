using System.Dynamic;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Unity.VisualScripting;
using System;
using System.Linq;
using System.Collections;

public class GoodsSortingManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private float sortingDelay = 0.75f;
    [SerializeField] private float cooldownPeriod = 2f;

    private NodeManager nodeManager;
    private LevelManager levelManager;
    private GoodsPlacementManager goodsPlacementManager;
    private TrucksLoaderManager trucksLoaderManager;

    private Node currentSelectedNode = null;
    private Dictionary<ItemType, List<string>> connectedNodesDict = new Dictionary<ItemType, List<string>>();

    private bool foundDifferentKey = false;
    public bool isInitialized = false;

    private Node firstNode = null, secondNode = null;
    private int currentAvailSlots = 0, itemsToMove = 0, cacheCount = 0;

    public bool isSortingInProgress = false;
    public bool generalSortingState = false;
    public bool hasCheckedCachedData = false;
    public bool noNeighborsToCheck = false;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsSortingManager>(this);
        levelManager = InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>();
    }

    public Node GetCurrentSelectedNode() => currentSelectedNode;

    public void CheckNeighbors(Node selectedNode)
    {
        SetNodeManager();
        SetGoodsPlacementManager();
        SetTrucksLoaderManager();
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

        // isSortingInProgress = false;

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

        // connectedNodesDict.Clear();
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
                foundNeighbor.SetNodeOccupiedState(true);
                UpdateNodeWithCachedData(cachedKey, source: source, target: foundNeighbor);
                CheckNeighbors(foundNeighbor);
            }
            else 
            {
                if (FindNewNeighborWithEmptySlots(source,  neighbor: out foundNeighbor, out availSlots))
                {
                    Debug.LogError($"Found new neighbor: {foundNeighbor.name}");
                    UpdateNodeWithCachedData(cachedKey, source: source, target: foundNeighbor);
                    foundNeighbor.SetNodeOccupiedState(true);
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

    public void CompleteSorting()
    {
        if (generalSortingState)
        {
            levelManager.OnLevelStateChange(LevelState.Lost);
            isSortingInProgress = true;
        }
    }

    public IEnumerator CheckSortProgress()
    {
        yield return new WaitUntil(() => !isSortingInProgress);

        // CheckGameOverCondition("GoodsSortingManager");
    }

    private void CheckConnectedNodes(ItemType currentSetItemKey)
    {
        if (!levelManager.CanPlayLevel) return;
        
        ItemType otherSetItemKey;
        Debug.Log($"GameOverCheck :: GotMatchingNeighborWithAvailSlots");
        var hasMatchingNeighbor = GetMatchingNeighborWithAvailSlots(currentSetItemKey, currentSelectedNode, out Node neighbor, out int slots);
        bool isNotLastKey = false;
        if (currentSelectedNode != null && currentSelectedNode.GetSetKeysCount() > 0)
            isNotLastKey = currentSetItemKey != currentSelectedNode.GetSetKeys().Last(); 

        Debug.Log($"GameOverCheck :: hasMatchingNeighbor: {hasMatchingNeighbor} in GoodsSortingManager");
        // Debug.Log($"GameOverCheck :: isNotLastKey: {isNotLastKey} in GoodsSortingManager");
        Debug.Log($"GameOverCheck :: isSortingInProgress: {isSortingInProgress} in GoodsSortingManager");

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
            isSortingInProgress = (firstNode && firstNode.HasCachedData()) || (secondNode && secondNode.HasCachedData()) || 
                                    connectedNodesDict.Any(connectedNodePair => connectedNodePair.Value.Count > 1);
            Debug.Log($"IsSortingInProgress State: {isSortingInProgress}");

            if (firstNode && firstNode.HasCachedData())
                foreach (var key in firstNode.GetCachedKeys())
                    CheckIfCachedDataIsLeft(firstNode, key);

            if (secondNode && secondNode.HasCachedData())
                foreach (var key in secondNode.GetCachedKeys())
                    CheckIfCachedDataIsLeft(secondNode, key);

            connectedNodesDict[currentSetItemKey].Clear();
            Debug.Log($"Clearing connected nodes for {currentSetItemKey}");
            
            hasCheckedCachedData = true;

            Debug.Log($"GameOverCheck ::: Check sort progress");
            // StartCoroutine(CheckSortProgress());
            
            // CheckGameOverCondition("GoodsSortingManager");

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
        else if (firstNode.GetSetKeys().Count > 1 && firstNode.GetNextKeyAfterCurrent(currentSetItemKey, out otherSetItemKey) && secondNode.HasGoodsSet(currentSetItemKey)) // swapping scenario when multiple keys are involved
        {
            Debug.Log($"3rd condition :: otherSetItemKey: {otherSetItemKey}, firstNode: {firstNode.name}");
            Debug.Log($"3rd condition :: setcount: {firstNode.GetSetKeys().Count}");

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
        else if (secondNode.GetNextKeyAfterCurrent(currentSetItemKey, out otherSetItemKey) && firstNode.HasGoodsSet(currentSetItemKey)) // TODO :: Double check this case, could be useful if both first and second node is full
        {
            Debug.LogError($"5th condition :: CheckConnectedNodes :: double check this logic...");

            Debug.Log($"5th condition :: otherSetItemKey: {otherSetItemKey}, secondNode: {secondNode.name}");
            Debug.Log($"5th condition :: setcount: {secondNode.GetSetKeys().Count}");

            cacheCount = Mathf.Min(secondNode.GetGoodsSetCountForSpecificItem(otherSetItemKey), firstNode.GetGoodsSetCountForSpecificItem(currentSetItemKey));

            secondNode.StoreCachedData(otherSetItemKey, cacheCount);
            secondNode.FreeUpGoodsSet(otherSetItemKey, cacheCount);
            secondNode.SortItemsData();

            secondNode.CacheAndStoreItemBases(otherSetItemKey, cacheCount);
            secondNode.SortItemBases();

            goodsPlacementManager.RearrangeBasedOnSorting(secondNode);
            secondNode.UpdateOccupiedSlotsState();
        }
        else
        {
            Debug.LogError($"6th condition :: CheckConnectedNodes :: no op...");
        }

        UpdateConnectedNodeStates(currentSetItemKey);
        CheckConnectedNodes(currentSetItemKey);
    }

    public void CheckGameOverCondition(string name)
    {
        // Debug.Log($"{name} ::: nodeManager.AreAllNodesOccupied(): {nodeManager.AreAllNodesOccupied()} && isSortingInProgress: {isSortingInProgress}");
        Debug.Log($"GameOverCheck :: CheckGameOverCondition :: isSortingInProgress: {isSortingInProgress}, nodeManager.AreAllNodeOccupied: {nodeManager.AreAllNodesOccupied()}");
        // if (nodeManager.AreAllNodesOccupied() && !isSortingInProgress && levelManager.LevelState != LevelState.Lost)
        if (nodeManager.AreAllNodesOccupied() && !isSortingInProgress && levelManager.LevelState != LevelState.Lost)
        {
            Debug.Log($"GameOverCheck :: CheckGameOverCondition LevelState.Lost");
            Invoke(nameof(Delay), 1f);
        }
    }

    private void Delay()
    {
        levelManager.OnLevelStateChange(LevelState.Lost);
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
    
    private void SetTrucksLoaderManager()
    {
        trucksLoaderManager = trucksLoaderManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<TrucksLoaderManager>() : trucksLoaderManager;
    }

    internal void ClearConnectedNodes()
    {
        connectedNodesDict.Clear();
        isSortingInProgress = false;
        generalSortingState = false;
        hasCheckedCachedData = false;
        noNeighborsToCheck = false;
    }
}
