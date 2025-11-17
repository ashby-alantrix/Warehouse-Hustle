using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckGoodsLoader : MonoBehaviour
{
    [SerializeField] private Transform[] m_NodePlacements;
    [SerializeField] private int totalSlotsInNode = 12;

    public Vector3 GetPosDataBasedOnIndex(int idx)
    {
        return m_NodePlacements[idx].position;
    }
}
