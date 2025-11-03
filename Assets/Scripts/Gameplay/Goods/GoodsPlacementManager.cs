using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GoodsPlacementManager : MonoBehaviour, IBase, IBootLoader
{
    private GoodsManager m_GoodsManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsPlacementManager>(this);
    }

    public void PlaceGoodsOnNode(Node selectedNode)
    {
        selectedNode.InitItemBases();
        NodePlacementData nodePlacementData = null;

        for (int index = 0; index < selectedNode.GetItemBaseCount(); index++)
        {
            nodePlacementData = selectedNode.RetrieveNodePlacementData(index);
            nodePlacementData.isOccupied = true;

            selectedNode.GetItemBase(index).transform.DOMove(nodePlacementData.transform.position, 1f);
        }
    }
}
