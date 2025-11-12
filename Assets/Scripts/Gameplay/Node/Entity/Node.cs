using System.Diagnostics.SymbolStore;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class Node : MonoBehaviour
{
    [SerializeField] private Material occupiedMat;
    [SerializeField] private Material unOccupiedMat;

    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private int totalSlotsInNode = 12;
    [SerializeField] private Transform[] m_NodePlacements;

    private int m_NodePlacementLength = 0;
    public bool isNodeOccupied = false;

    private NodePlacementData[] m_NodePlacementDatas;
    private List<Vector3> neighborsHexOffsets = new List<Vector3>();

    private Dictionary<ItemType, int> goodsSetDict = new Dictionary<ItemType, int>();
    private Dictionary<ItemType, List<ItemBase>> itemBasesCollection = new Dictionary<ItemType, List<ItemBase>>();
    private Dictionary<ItemType, int> cachedGoodsSet = new Dictionary<ItemType, int>();

    public Dictionary<ItemType, List<ItemBase>> ItemBasesCollection => itemBasesCollection;

    private NodeManager nodeManager;
    private GoodsManager goodsManager;
    private ObjectPoolManager objectPoolManager;

    public int GetItemTypeCount() => goodsSetDict.Count;

    public int GetTotalSlotsInNode() => totalSlotsInNode;

    public bool HasEmptySlots(out int availSlots)
    {
        int totalSlotsOccupied = 0;
        foreach (var set in goodsSetDict)
        {
            totalSlotsOccupied += set.Value;
        }

        availSlots = totalSlotsInNode - totalSlotsOccupied;
        return availSlots != 0;
    }

    public bool HasCachedData()
    {
        return false;
    }

    public int GetCachedData() // TODO :: change return type and data to send back here
    {
        return 0;
    }

    public void StoreCachedData(int cacheCount, ItemType otherSetItemKey)
    {
        // store the goods set 
        // store the item bases
        if (!cachedGoodsSet.ContainsKey(otherSetItemKey))
            cachedGoodsSet.Add(otherSetItemKey, cacheCount);
    }

    public void FreeUpGoodsSet(int cacheCount, ItemType otherSetItemKey)
    {
        if (goodsSetDict.ContainsKey(otherSetItemKey))
        {
            goodsSetDict[otherSetItemKey] -= cacheCount;
        }
    }

    public bool GetNextKeyAfterCurrent(ItemType currentType, out ItemType itemType)
    {
        foreach (var goodSet in goodsSetDict)
        {
            if (goodSet.Key == currentType)
            {
                continue;
            }
            else
            {
                itemType = goodSet.Key;
                return true;
            }
        }

        itemType = ItemType.MAX;
        return false;
    }

    public Dictionary<ItemType, int> GetSetDict() => goodsSetDict;

    public List<ItemType> GetSetKeys()
    {
        return goodsSetDict.Keys.ToList();
    }

    public int GetSetKeysCount()
    {
        return goodsSetDict.Keys.Count;
    }
    
    public void InitNodeManager(NodeManager nodeManager)
    {
        this.nodeManager = nodeManager;
    }

    public bool CheckIfSetItemMatches(ItemType itemType, out int goodsCount)
    {
        Debug.Log($"JsonData: {JsonConvert.SerializeObject(goodsSetDict)}");
        goodsCount = goodsSetDict.ContainsKey(itemType) ? goodsSetDict[itemType] : 0;
        // Debug.Log($"Test 5: DoesNeighborHaveSimilarItem: itemType: " + itemType + ", goodsCount: " + goodsCount);

        return goodsSetDict.ContainsKey(itemType);
    }

    public int GetGoodsSetCount(ItemType itemType)
    {
        return goodsSetDict.ContainsKey(itemType) ? goodsSetDict[itemType] : 0;
    }

    public bool HasGoodsSet(ItemType itemType)
    {
        return goodsSetDict.ContainsKey(itemType);
    }

    public bool IsThereDifferentKey(ItemType itemType)
    {
        Debug.Log($"IsLastKey:: {itemType}");
        if (goodsSetDict.Count > 0)
        {
            Debug.Log($"IsLastKey:: goodsSetDict.Keys: " + goodsSetDict.Keys.Count);
            Debug.Log($"IsLastKey:: {itemType} == {goodsSetDict.Keys.Last()}");
            return itemType != goodsSetDict.Keys.Last();
            return goodsSetDict.Keys.Contains(itemType);
        }
        else
        {
            // Debug.Log($"IsLastKey:: not last key");
            return false;
        }

        return goodsSetDict.Count > 0 ? itemType == goodsSetDict.Keys.Last() : false;
    }

    public void InitItemsData()
    {
        goodsManager = goodsManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsManager>() : goodsManager;

        var itemBaseObjects = goodsManager.GoodsHandler.CurrentGoodsPlacer.GetBaseObjects();
        foreach (var baseObj in itemBaseObjects)
            AddToItemBasesCollection(baseObj);

        // foreach (var item in itemBaseObjects)
        // {
        //     Debug.Log($"### Test 9: item.Type: {item.ItemType}, item.Value: {item}");
        // }

        // foreach (var item in itemBasesCollection)
        // {
        //     Debug.Log($"### Test 10: item.Type: {item.Key}, item.Value: {item.Value.Count}");
        // }

        var goodsDataSet = goodsManager.GoodsHandler.CurrentGoodsPlacer.GetGoodsDataSet();
        foreach (var data in goodsDataSet)
            AddItemsDataToNode(data.type, data.setCount);

        Debug.Log($"goodsDataSetJson for {this.name}: {JsonConvert.SerializeObject(goodsDataSet)}");
    }

    #region NODE_DATA_UPDATION
    public void AddItemsDataToNode(ItemType itemType, int itemsToAddCount)
    {
        // Debug.Log($"AddItemsDataToNode");
        // foreach (var data in goodsSetDict)
        // {
        //     Debug.Log($"GoodsKey: {data.Key}, GoodsCount: {data.Value}");
        // }

        // Debug.Log($"Before updating goods set");
        // Debug.Log($"number of goods sets: {goodsSetDict.Count}");
        // int itemsCount = goodsSetDict.ContainsKey(itemType) ? goodsSetDict[itemType] : 0;
        // Debug.Log($"number of goods for set({itemType}): {itemsCount}");

        if (!goodsSetDict.ContainsKey(itemType))
            goodsSetDict.Add(itemType, itemsToAddCount);
        else
            goodsSetDict[itemType] += itemsToAddCount;

        // Debug.Log($"After updating goods set");
        // Debug.Log($"number of goods sets: {goodsSetDict.Count}");
        // itemsCount = goodsSetDict.ContainsKey(itemType) ? goodsSetDict[itemType] : 0;
        // Debug.Log($"number of goods for set({itemType}): {itemsCount}");
    }

    public void RemoveItemsDataFromNode(ItemType itemType, int itemsToRemoveCount)
    {
        if (goodsSetDict.ContainsKey(itemType))
        {
            int availCount = goodsSetDict[itemType];
            if (availCount > itemsToRemoveCount)
            {
                availCount -= itemsToRemoveCount;
                goodsSetDict[itemType] = availCount;

                if (goodsSetDict[itemType] == 0)
                    goodsSetDict.Remove(itemType);
            }
            else //if (availCount == itemsToRemoveCount)
            {
                // TODO :: double check condition
                Debug.Log($"### Removing item type :: availCount: {availCount}, itemsToRemoveCount: {itemsToRemoveCount}");
                goodsSetDict.Remove(itemType);
                Debug.Log($"goodsSetDict.ContainsKey: {goodsSetDict.ContainsKey(itemType)}");
            }
        }
    }

    public void SortItemBases()
    {
        var sortedDict = itemBasesCollection.OrderByDescending(pair => pair.Value.Count).ToList();
        itemBasesCollection.Clear();
        goodsSetDict.Clear();

        Debug.Log($"Iteration: sortedDict: {sortedDict.Count()}");

        foreach (var sortedItem in sortedDict)
        {
            itemBasesCollection.Add(sortedItem.Key, sortedItem.Value);
            goodsSetDict.Add(sortedItem.Key, sortedItem.Value.Count);

            Debug.Log($"Iteration: sortedItem: {sortedItem.Key}, sortedItem.Value: {sortedItem.Value.Count}");
        }
    }

    public void SortItemsData()
    {
        var sortedDict = goodsSetDict.OrderByDescending(pair => pair.Value);
        goodsSetDict.Clear();

        foreach (var data in sortedDict)
            goodsSetDict.Add(data.Key, data.Value);
    }

    #endregion

    #region ITEMS_BASE_UPDATION

    public void AddToItemBasesCollection(ItemBase baseObj)
    {
        if (!itemBasesCollection.ContainsKey(baseObj.ItemType))
            itemBasesCollection.Add(baseObj.ItemType, new List<ItemBase>() { baseObj });
        else
            itemBasesCollection[baseObj.ItemType].Add(baseObj);
    }

    public ItemBase RemoveFromItemBasesCollection(ItemType itemType)
    {
        // if (!itemBasesCollection.ContainsKey(itemType) || itemBasesCollection[itemType].Count < 1) return null;

        Debug.Log($"#### itemBasesCollection: {itemBasesCollection != null}");
        Debug.Log($"ItemToRemove: {itemBasesCollection.Count}");
        ItemBase itemToRemove = itemBasesCollection[itemType][0];
        itemBasesCollection[itemType].RemoveAt(0);

        if (itemBasesCollection[itemType].Count == 0)
        {
            Debug.Log($"Removing item");
            itemBasesCollection.Remove(itemType);
            Debug.Log($"after removal itemBasesCollection: {itemBasesCollection.Count}");
        }

        return itemToRemove;
    }

    #endregion

    public int GetItemBaseCount()
    {
        int itemBaseCount = 0;
        foreach (var data in itemBasesCollection)
            itemBaseCount += data.Value.Count;

        return itemBaseCount;
    }

    public int GetGoodsSetsCount()
    {
        int setsCount = 0;
        foreach (var data in goodsSetDict)
            setsCount += data.Value;

        return setsCount;
    }

    public List<ItemType> GetKeysForItems()
    {
        return itemBasesCollection.Keys.ToList();
    }

    public int GetSetsCountForItemType(ItemType itemType)
    {
        return itemBasesCollection[itemType].Count;
    }

    public ItemBase GetItemBase(int index, ItemType itemType)
    {
        return itemBasesCollection[itemType][index];
    }

    public List<ItemBase> GetSpecificItems(ItemType itemType)
    {
        try
        {
            return itemBasesCollection[itemType];
            // return itemBasesCollection.ContainsKey(itemType) ? itemBasesCollection[itemType] : new List<ItemBase>();
            // return itemBasesCollection.Select(item => item).Where(item => item.ItemType == itemType).ToList();
        }
        catch (Exception ex)
        {
            Debug.LogError("Caught exception: " + ex.Message);
        }

        return new List<ItemBase>();
    }

    public int GetNeighborsCount() => neighborsHexOffsets.Count;

    public int GetNeighborHexOffsetLength() => neighborsHexOffsets.Count;

    public Vector3 GetNeighborHexOffset(int index)
    {
        return neighborsHexOffsets[index];
    }

    public string GetNodePos() => $"{transform.position}";

    public void AddNeighborsData(Vector3 hexOffset)
    {
        neighborsHexOffsets.Add(hexOffset);
    }

    public NodePlacementData RetrieveNodePlacementData(int index)
    {
        return m_NodePlacementDatas[index];
    }

    public void OnMouseDown()
    {
        Debug.Log($"OnNodeClicked: OnMouseDown() :: name: {transform.name}, isNodeOccupied: {isNodeOccupied}");

        if (!isNodeOccupied) // game's not over
        {
            Debug.Log($"OnNodeClicked: nodeManager.OnNodeClicked :: name: {transform.name}, isNodeOccupied: {isNodeOccupied}");
            nodeManager.OnNodeClicked(this);
        }
    }

    public void SetNodeOccupiedState(bool state)
    {
        isNodeOccupied = state;
        meshRenderer.material = isNodeOccupied ? occupiedMat : unOccupiedMat;
    }
    
    private void Awake()
    {
        m_NodePlacementLength = m_NodePlacements.Length;
        m_NodePlacementDatas = new NodePlacementData[m_NodePlacementLength];

        for (int i = 0; i < m_NodePlacementLength; i++)
        {
            m_NodePlacementDatas[i] = new NodePlacementData();
            m_NodePlacementDatas[i].isOccupied = false;
            m_NodePlacementDatas[i].transform = m_NodePlacements[i];
        }
    }

    public void CheckIfNodeIsFullOrCleared()
    {
        var itemBaseCount = GetItemBaseCount();

        Debug.Log($"itemBasesCollection.Count: {itemBasesCollection.Count}");

        if (itemBasesCollection.Count == 1 && totalSlotsInNode == itemBaseCount)
        {
            Debug.Log($"Pushing nodes back to pool");
            SetObjectPoolManager();
            OnNodeFull(); // TODO :: temporary logic, update with loading onto to truck

            goodsSetDict.Clear();
            itemBasesCollection.Clear();
            SetNodeOccupiedState(false);
        }
        else if (itemBaseCount == 0)
        {
            // goodsSetDict.Clear();
            // itemBasesCollection.Clear();
            Debug.Log($"MoveMatchedSetToTarget OnComplete items are empty");
            SetNodeOccupiedState(false);
        }
    }

    private void OnNodeFull()
    {
        foreach (var itemBase in itemBasesCollection)
        {
            foreach (var item in itemBase.Value)
            {
                item.gameObject.SetActive(false);
                objectPoolManager.PassObjectToPool(itemBase.Key, item);
            }
        }       
            
    }

    private void SetObjectPoolManager()
    {
        objectPoolManager = objectPoolManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>() : objectPoolManager;
    }

    public void UpdateOccupiedSlotsState()
    {
        var itemsCount = GetItemBaseCount();

        for (int index = 0; index < totalSlotsInNode; index++)
            m_NodePlacementDatas[index].isOccupied = index < itemsCount;
    }
}
