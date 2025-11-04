using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private Transform[] m_NodePlacements;

    private int m_NodePlacementLength = 0;
    public bool isNodeOccupied = false;

    private NodePlacementData[] m_NodePlacementDatas;
    private List<ItemBase> itemBases = new List<ItemBase>();
    private List<Vector3> neighborsHexOffsets = new List<Vector3>();

    private Dictionary<ItemType, int> setsDict = new Dictionary<ItemType, int>();

    private NodeManager m_NodeManager;
    private GoodsManager m_GoodsManager;

    public void InitNodeManager(NodeManager nodeManager)
    {
        m_NodeManager = nodeManager;
    }

    public void InitItemsData()
    {
        m_GoodsManager = m_GoodsManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsManager>() : m_GoodsManager;
        itemBases = m_GoodsManager.GoodsHandler.CurrentGoodsPlacer.GetBaseObjects();
        Debug.Log($"### CurrentGoodsPlacer.GetBaseObjects().Count: {itemBases.Count}");

        var goodsDataSet = m_GoodsManager.GoodsHandler.CurrentGoodsPlacer.GetGoodsDataSet();
        foreach (var data in goodsDataSet)
        {
            AddItemsDataToNode(data.type, data.setCount);
            Debug.Log($"### data.type: {data.type}, data.setCount: {data.setCount}");
        }

        // Debug.Log($"### NodePos: {this.transform.position}");
    }

    #region NODE_DATA_UPDATION
    public void AddItemsDataToNode(ItemType itemType, int itemsToAddCount)
    {
        if (!setsDict.ContainsKey(itemType))
            setsDict.Add(itemType, itemsToAddCount);
        else
            setsDict[itemType] += itemsToAddCount;
    }

    // Remove the respective 
    public void RemoveItemsDataFromNode(ItemType itemType, int itemsToRemoveCount)
    {
        if (setsDict.ContainsKey(itemType))
        {
            int availCount = setsDict[itemType];
            if (availCount > itemsToRemoveCount)
            {
                availCount -= itemsToRemoveCount;
                setsDict[itemType] = availCount;
            }
            else //if (availCount == itemsToRemoveCount)
            {
                // TODO :: double check condition
                Debug.Log($"### Removing item type :: availCount: {availCount}, itemsToRemoveCount: {itemsToRemoveCount}");
                setsDict.Remove(itemType);
            }
        }
    }
    #endregion

    public int GetItemBaseCount() => itemBases.Count;

    public ItemBase GetItemBase(int index) => itemBases[index];

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
        Debug.Log("OnMouseDown");
        if (!isNodeOccupied) // game's not over
        {
            isNodeOccupied = true;
            m_NodeManager.OnNodeClicked(this);
        }
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
