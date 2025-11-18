using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManagerTest : MonoBehaviour
{
    public bool isLevelGeneratorTest = true;

    public GridData m_GridData;
    public GridManager gridManager;

    [ContextMenu("GenerateGridData")]
    public void GenerateNewGridData()
    {
        ClearAllNodesData();
        gridManager.ClearData();
        gridManager.InitGridInfo(m_GridData);
        gridManager.InitGridData();
    }

    private void ClearAllNodesData()
    {
        foreach (Transform child in gridManager.NodesParent)
            Destroy(child.gameObject);

        gridManager.NodeManager.ClearNodesData();
    }
}
