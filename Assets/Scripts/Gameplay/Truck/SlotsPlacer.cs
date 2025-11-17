using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotsPlacer
{
    [SerializeField] private Transform[] m_NodePlacements;
    [SerializeField] private int totalSlotsInNode = 12;

    public int TotalSlotsInNode => totalSlotsInNode;

    public Vector3 GetPosDataBasedOnIndex(int idx)
    {
        return m_NodePlacements[idx].position;
    }
}
