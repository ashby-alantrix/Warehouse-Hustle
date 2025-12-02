#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridEditorManager : MonoBehaviour
{
    public static bool isLevelGeneratorTest = false;

    public GridData m_GridData;

    public GridManagerEditor gridManagerEditor;
    private void Awake()
    {
        isLevelGeneratorTest = true;
    }

    public void GenerateNewGridData()
    {
        ClearAllNodesData();
        gridManagerEditor.ClearData();
        gridManagerEditor.InitGridInfo(m_GridData);
        gridManagerEditor.InitGridData();
    }

    private void ClearAllNodesData()
    {
        foreach (Transform child in gridManagerEditor.NodesParent)
            Destroy(child.gameObject);

        gridManagerEditor.NodeManagerEditor.ClearNodesData();
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
        Debug.Log($"Add blocked grid value: {row}, {col}");
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
#endif