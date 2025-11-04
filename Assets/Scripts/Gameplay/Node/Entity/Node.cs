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
    private Dictionary<ItemType, List<ItemBase>>  itemBasesCollection = new Dictionary<ItemType, List<ItemBase>>();

    private NodeManager nodeManager;
    private GoodsManager goodsManager;

    public int GetItemTypeCount() => goodsSetDict.Count;

    public int GetTotalSlotsInNode => totalSlotsInNode;

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

    public void InitNodeManager(NodeManager nodeManager)
    {
        this.nodeManager = nodeManager;
    }

    public bool DoesNeighborHaveSimilarItem(ItemType itemType, out int goodsCount)
    {
        goodsCount = goodsSetDict.ContainsKey(itemType) ? goodsSetDict[itemType] : 0;
        return goodsSetDict.ContainsKey(itemType);
    }

    public void InitItemsData()
    {
        goodsManager = goodsManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsManager>() : goodsManager;

        var itemBaseObjects = goodsManager.GoodsHandler.CurrentGoodsPlacer.GetBaseObjects();

        Debug.Log($"### Test1: {itemBaseObjects.Count}");

        foreach (var baseObj in itemBaseObjects)
        {
            if (!itemBasesCollection.ContainsKey(baseObj.ItemType))
                itemBasesCollection.Add(baseObj.ItemType, new List<ItemBase>() { baseObj });
            else
                itemBasesCollection[baseObj.ItemType].Add(baseObj);
        } 

        foreach (var itemBase in itemBasesCollection)
        {
            Debug.Log($"### Test2: {itemBase.Key}, {itemBase.Value.Count}");
        }

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

    public int GetItemBaseCount()
    {
        int itemBaseCount = 0;
        foreach (var data in itemBasesCollection)
        {
            Debug.Log($"data.Key: {data.Key}, data.Value.Count: {data.Value.Count}");
            itemBaseCount += data.Value.Count;
        }

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
        return itemBasesCollection[itemType];
        // return itemBasesCollection.Select(item => item).Where(item => item.ItemType == itemType).ToList();
    }

    public int GetNeighborsCount() => neighborsHexOffsets.Count;

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
        Debug.Log($"Test3: {transform.position} :: isNodeOccupied: {isNodeOccupied}");
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
