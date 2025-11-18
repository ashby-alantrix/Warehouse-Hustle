using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManagerTest : MonoBehaviour
{
    public static bool isLevelGeneratorTest = false;

    public GridData m_GridData;
    public GridManager gridManager;

    private void Awake()
    {
        isLevelGeneratorTest = true;
    }

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

    public void RemoveBlockedGridValue(int row, int col)
    {
        foreach (var nodeInfo in m_GridData.nodeInfos)
        {
            if (nodeInfo.gridValues.row != row)
                continue;

            if (nodeInfo.blockedGridValues.Count() < 1) break;

            foreach (var blockedGridValue in nodeInfo.blockedGridValues)
            {
                if (blockedGridValue.col == col)
                {
                    nodeInfo.blockedGridValues.Remove(blockedGridValue);
                    break;
                }
            }
        }
    }

    public void AddBlockedGridValue(int row, int col)
    {
        foreach (var nodeInfo in m_GridData.nodeInfos)
        {
            if (nodeInfo.gridValues.row != row)
                continue;

            nodeInfo.blockedGridValues.Add(new GridValues
            {
                row = row,
                col = col
            });
        }
    }
}
