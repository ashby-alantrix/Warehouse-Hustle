using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject hexNode;
    [SerializeField] private TextAsset gridJson;

    private float m_Rows; // z
    private float m_RowPosition; // z
    private float m_Cols; // x
    private float m_PreCols;
    private float m_BaseColCount;

    private GridData m_GridData;
    private Dictionary<float, List<float>> blockedGridValDict = new Dictionary<float, List<float>>();

    void Start()
    {
        Debug.Log("data:" + gridJson.text);

        var obj = new { Name = "Ashby", Age = 25 };
        string json = JsonConvert.SerializeObject(obj);
        Debug.Log(json);

        m_GridData = JsonConvert.DeserializeObject<GridData>(gridJson.text);

        InitGridData();
    }

    void InitGridData()
    {
        var nodeOffset = (hexNode.transform.localScale.z / 2) + (hexNode.transform.localScale.z / 4); // 0.75

        foreach (var nodeInfo in m_GridData.nodeInfos)
        {
            m_PreCols = m_Cols;

            m_Rows = nodeInfo.gridValues.row;
            m_Cols = nodeInfo.gridValues.col;

            var blockedGridValLength = nodeInfo.blockedGridValues.Length;
            blockedGridValDict.Add(m_Rows, new List<float>(blockedGridValLength));
            for (int i = 0; i < blockedGridValLength; i++)
            {
                blockedGridValDict[m_Rows].Add(nodeInfo.blockedGridValues[i].col);
            }

            GenerateGrid();

            m_RowPosition += nodeOffset;
        }
    }
    
    void GenerateGrid()
    {
        float startPointVal = 0;
        float extraNodeCount = 0;
        float diff = 0;
        float times = 0;

        if (m_PreCols != 0)
        {
            if (/*m_PreCols > m_Cols && */ m_Cols <= m_BaseColCount) // row1: 4 elements, row2: 3 elements
            {
                startPointVal = 0.5f;
            }
            else if (m_Cols > m_PreCols || m_Cols > m_BaseColCount) // row1: 4 elements, row2: 5 elements
            {
                startPointVal = -0.5f;
            }

            if (m_Rows % 2 == 0) // even row
            {
                extraNodeCount = Mathf.Abs(m_Cols - m_BaseColCount) - 1; // odd cols, making it even

                diff = extraNodeCount / 2; // 0 // 1 // 3

                times = Mathf.Sign(startPointVal) * diff; // 0 // -1
            }
            else // odd row
            {
                extraNodeCount = Mathf.Abs(m_Cols - m_BaseColCount); // even cols
                diff = extraNodeCount / 2; // 0 // 1 // 3

                times = Mathf.Sign(startPointVal) * diff; // 0 // -1

                startPointVal = 0;
            }

            // float absVal = Mathf.Abs(m_PreCols - m_Cols); // 1 // 3 // 5
            // float diff = absVal > 1 ? (extraNodeCount/2) : absVal - 1; // 0 // 1 // 3
            //diff = extraNodeCount / 2; // 0 // 1 // 3

            //times = Mathf.Sign(startPointVal) * diff; // 0 // -1

            startPointVal = startPointVal + times; // -0.5 // -1.5 // -3.5
        }
        else
        {
            m_BaseColCount = m_Cols;
        }

        for (float j = 0; j < m_Cols; j++)
        {
            if (blockedGridValDict.ContainsKey(m_Rows) && blockedGridValDict[m_Rows].Contains(j + 1))
                continue;

            Instantiate(hexNode, new Vector3(j + startPointVal, 0, m_RowPosition), Quaternion.identity);
        }
    }
}
