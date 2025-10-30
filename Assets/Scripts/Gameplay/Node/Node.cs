using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private Transform[] m_NodePlacements;

    private NodePlacementData[] m_NodePlacementDatas;
    private List<Vector3> m_NeighboursHexOffsets;

    private int m_NodePlacementLength = 0;
    public bool isNodeOccupied = false;
    private NodeManager m_NodeManager;

    public void InitNodeManager(NodeManager nodeManager)
    {
        m_NodeManager = nodeManager;
    }

    public void AddNeighboursData(Vector3 hexOffset)
    {
        m_NeighboursHexOffsets.Add(hexOffset);
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

    public void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
        if (!isNodeOccupied) // game's not over
        {
            isNodeOccupied = true;
            m_NodeManager.OnNodeClicked();
        }
    }
}
