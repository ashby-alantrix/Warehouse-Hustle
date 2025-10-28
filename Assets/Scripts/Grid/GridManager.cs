using System;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject hexNode;

    private float m_Rows; // z
    private float m_RowPosition; // z
    private float m_Cols; // x
    private float m_PreCols;
    private float m_BaseColCount;

    private GridData m_GridData;

    void Start()
    {
        GridData gridData = new GridData();
        gridData.nodeInfos = new NodeInfo[5];
        gridData.nodeInfos[0] = new NodeInfo
        {
            row = 1,
            col = 4,
            placeNode = true
        };
        gridData.nodeInfos[1] = new NodeInfo
        {
            row = 2,
            col = 5,
            placeNode = true
        };
        gridData.nodeInfos[2] = new NodeInfo
        {
            row = 3,
            col = 6,
            placeNode = true
        };
        gridData.nodeInfos[3] = new NodeInfo
        {
            row = 4,
            col = 5,
            placeNode = true
        };
        gridData.nodeInfos[4] = new NodeInfo
        {
            row = 5,
            col = 4,
            placeNode = true
        };

        InitGridData(gridData);
    }

    void InitGridData(GridData gridData)
    {
        m_GridData = gridData;
        var nodeOffset = (hexNode.transform.localScale.z / 2) + (hexNode.transform.localScale.z / 4); // 0.75

        foreach (var nodeInfo in gridData.nodeInfos)
        {
            m_PreCols = m_Cols;

            m_Rows = nodeInfo.row;
            m_Cols = nodeInfo.col;

            GenerateGrid();

            m_RowPosition += nodeOffset;
        }

        // foreach (var data in m_GridData.nodeInfos)
        // {
        //     if (!data.placeNode) continue;

        //     Instantiate(hexNode, new Vector3(data.col, 0, data.row), Quaternion.identity);
        // }
    }
    
    void GenerateGrid()
    {
        float startPointVal = 0;
        float extraNodeCount = 0;

        if (m_PreCols != 0)
        {
            if (m_PreCols > m_Cols) // row1: 4 elements, row2: 3 elements
            {
                startPointVal = 0.5f;
            }
            else if (m_PreCols < m_Cols) // row1: 4 elements, row2: 5 elements
            {
                startPointVal = -0.5f;
            }

            if (m_Rows % 2 == 0) // even row
            {
                extraNodeCount = Mathf.Abs(m_Cols - m_BaseColCount) - 1; // odd cols, making it even
            }
            else // odd row
            {
                extraNodeCount = Mathf.Abs(m_Cols - m_BaseColCount); // even cols
            }

            // float absVal = Mathf.Abs(m_PreCols - m_Cols); // 1 // 3 // 5
            // float diff = absVal > 1 ? (extraNodeCount/2) : absVal - 1; // 0 // 1 // 3
            float diff = extraNodeCount / 2; // 0 // 1 // 3

            float times = Mathf.Sign(startPointVal) * diff; // 0 // -1
            startPointVal = startPointVal + times; // -0.5 // -1.5 // -3.5
        }
        else
        {
            m_BaseColCount = m_Cols;
        }

        for (int j = 0; j < m_Cols; j++)
        {
            Instantiate(hexNode, new Vector3(j + startPointVal, 0, m_RowPosition), Quaternion.identity);
        }
    }
}
