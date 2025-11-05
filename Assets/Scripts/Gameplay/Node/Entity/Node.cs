using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class Node : MonoBehaviour
{
    [SerializeField] private int totalSlotsInNode = 12;
    [SerializeField] private Transform[] m_NodePlacements;

    private int m_NodePlacementLength = 0;
    public bool isNodeOccupied = false;

    private NodePlacementData[] m_NodePlacementDatas;
    private List<Vector3> neighborsHexOffsets = new List<Vector3>();

    private Dictionary<ItemType, int> goodsSetDict = new Dictionary<ItemType, int>();
    private Dictionary<ItemType, List<ItemBase>> itemBasesCollection = new Dictionary<ItemType, List<ItemBase>>();

    public Dictionary<ItemType, List<ItemBase>> ItemBasesCollection => itemBasesCollection;

    private NodeManager nodeManager;
    private GoodsManager goodsManager;

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

    public Dictionary<ItemType, int> GetSetDict() => goodsSetDict;

    public List<ItemType> GetSetKeys()
    {
        return goodsSetDict.Keys.ToList();
    }
    
    public void InitNodeManager(NodeManager nodeManager)
    {
        this.nodeManager = nodeManager;
    }

    public bool DoesNeighborHaveSimilarItem(ItemType itemType, out int goodsCount)
    {
        goodsCount = goodsSetDict.ContainsKey(itemType) ? goodsSetDict[itemType] : 0;
        Debug.Log($"Test 5: DoesNeighborHaveSimilarItem: itemType: " + itemType + ", goodsCount: " + goodsCount);

        return goodsSetDict.ContainsKey(itemType);
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
    }

    #region NODE_DATA_UPDATION
    public void AddItemsDataToNode(ItemType itemType, int itemsToAddCount)
    {
        if (!goodsSetDict.ContainsKey(itemType))
            goodsSetDict.Add(itemType, itemsToAddCount);
        else
            goodsSetDict[itemType] += itemsToAddCount;
    }

    // Remove the respective 
    public void RemoveItemsDataFromNode(ItemType itemType, int itemsToRemoveCount)
    {
        if (goodsSetDict.ContainsKey(itemType))
        {
            int availCount = goodsSetDict[itemType];
            if (availCount > itemsToRemoveCount)
            {
                availCount -= itemsToRemoveCount;
                goodsSetDict[itemType] = availCount;
            }
            else //if (availCount == itemsToRemoveCount)
            {
                // TODO :: double check condition
                Debug.Log($"### Removing item type :: availCount: {availCount}, itemsToRemoveCount: {itemsToRemoveCount}");
                goodsSetDict.Remove(itemType);
            }
        }
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
        ItemBase itemToRemove = itemBasesCollection[itemType][0];
        itemBasesCollection[itemType].RemoveAt(0);

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
        if (!isNodeOccupied) // game's not over
        {
            nodeManager.OnNodeClicked(this);
        }
    }

    public void SetNodeOccupiedState(bool state)
    {
        isNodeOccupied = state;
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
}
