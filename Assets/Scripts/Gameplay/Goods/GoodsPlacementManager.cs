using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GoodsPlacementManager : MonoBehaviour, IBase, IBootLoader
{
    private List<ItemBase> currentItemBases = new List<ItemBase>();

    private GoodsManager m_GoodsManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsPlacementManager>(this);
    }

    public void PlaceGoodsOnNode(Node selectedNode)
    {
        m_GoodsManager = m_GoodsManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsManager>() : m_GoodsManager;
        currentItemBases = m_GoodsManager.GoodsHandler.CurrentGoodsPlacer.GetBaseObjects();

        Debug.Log($"### {name}.PlaceGoodsOnNode: currentItemBases.Count: {currentItemBases.Count}");

        NodePlacementData nodePlacementData = null;

        for (int index = 0; index < currentItemBases.Count; index++)
        {
            nodePlacementData = selectedNode.RetrieveNodePlacementData(index);
            nodePlacementData.isOccupied = true;

            currentItemBases[index].transform.DOMove(nodePlacementData.transform.position, 1f);
        }
    }
}
