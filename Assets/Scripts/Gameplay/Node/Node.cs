using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private Transform[] m_NodePlacements;

    private NodePlacementData[] m_NodePlacementDatas;
    private List<Vector3> m_NeighborsHexOffsets = new List<Vector3>();
    private List<ItemBase> itemBases = new List<ItemBase>();

    private int m_NodePlacementLength = 0;
    public bool isNodeOccupied = false;

    private NodeManager m_NodeManager;
    private GoodsManager m_GoodsManager;

    public void InitNodeManager(NodeManager nodeManager)
    {
        m_NodeManager = nodeManager;
    }

    public void InitItemBases()
    {
        m_GoodsManager = m_GoodsManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsManager>() : m_GoodsManager;
        itemBases = m_GoodsManager.GoodsHandler.CurrentGoodsPlacer.GetBaseObjects();
    }

    public int GetItemBaseCount() => itemBases.Count;

    public ItemBase GetItemBase(int index) => itemBases[index];

    public void AddNeighborsData(Vector3 hexOffset)
    {
        m_NeighborsHexOffsets.Add(hexOffset);
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
