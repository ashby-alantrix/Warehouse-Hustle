using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class GridManagerEditor : MonoBehaviour
{
    [SerializeField] private TextAsset customJsonTextAsset;
    [SerializeField] private bool useCustomJson;
    [SerializeField] private bool overriteJson = false;

    [SerializeField] private NodeManagerEditor nodeManagerEditor;
    [SerializeField] private GridEditorManager gridEditorManager;

    [SerializeField] private GameObject hexNodeTestPrefab;
    [SerializeField] private GameObject nodesParent;

    private int m_Rows; // z
    private int m_Cols; // x
    private float m_RowPosition; // z
    private float m_PreCols;
    private float m_BaseColCount;

    private int tempCounter = 0;

    private GridData m_GridData;
    public Transform NodesParent => nodesParent.transform;
    public NodeManagerEditor NodeManagerEditor => nodeManagerEditor;

    private LevelEditorButtonType blockedMeshState = LevelEditorButtonType.NONE;

    private Dictionary<int, List<int>> blockedGridValDict = new Dictionary<int, List<int>>();
    
    public void InitGridInfo(GridData updatedGridData)
    {
        if (useCustomJson)
        {
            m_GridData = JsonConvert.DeserializeObject<GridData>(customJsonTextAsset.text);
            useCustomJson = false;
            overriteJson = true;
            gridEditorManager.m_GridData = m_GridData;
        }
        else
        {
            m_GridData = updatedGridData;
        }
    }

    public void InitGridData()
    {
        var nodeOffset = (hexNodeTestPrefab.transform.localScale.z / 2) + (hexNodeTestPrefab.transform.localScale.z / 4); // 0.75

        foreach (var nodeInfo in m_GridData.nodeInfos)
        {
            m_PreCols = m_Cols;

            m_Rows = nodeInfo.gridValues.row;
            m_Cols = nodeInfo.gridValues.col;

            var blockedGridValLength = nodeInfo.blockedGridValues.Count;
            if (!blockedGridValDict.ContainsKey(m_Rows))
                blockedGridValDict.Add(m_Rows, new List<int>(blockedGridValLength));

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

        for (int j = 0; j < m_Cols; j++)
        {
            var instance = Instantiate(hexNodeTestPrefab, new Vector3(j + startPointVal, 0, m_RowPosition), Quaternion.identity);
            instance.transform.SetParent(nodesParent.transform);
            instance.transform.name = $"{instance.transform.name} {tempCounter}";
            tempCounter++;

            var nodeInstTest = instance.GetComponent<NodeEditor>();
            nodeManagerEditor.AddNodeInstance(nodeInstTest, m_Rows, j + 1);

            if (blockedGridValDict.ContainsKey(m_Rows) && blockedGridValDict[m_Rows].Contains(j + 1))
            {
                if (nodeManagerEditor.IsNodeAvailableInGrid(m_Rows, j + 1, out NodeEditor nodeEditor))
                    nodeEditor.ChildTransform.gameObject.SetActive(false);
                continue;
            }
            
            nodeInstTest.ChildTransform.gameObject.SetActive(true);
            Debug.Log($"blockedMeshState: {blockedMeshState}");
            if (blockedMeshState == LevelEditorButtonType.SHOW_BLOCKED_MESH)
                ToggleAllBlockedSets(true);
            else if (blockedMeshState == LevelEditorButtonType.HIDE_BLOCKED_MESH)
                ToggleAllBlockedSets(false);
        }
    }

    public void SetBlockedMeshState(bool state)
    {
        blockedMeshState = state ? LevelEditorButtonType.SHOW_BLOCKED_MESH : LevelEditorButtonType.HIDE_BLOCKED_MESH;
    }

    public void ToggleAllBlockedSets(bool state)
    {
        foreach (var blockedData in blockedGridValDict)
        {
            Debug.Log($"RowValue: {blockedData.Key}");
            if (blockedGridValDict[blockedData.Key].Count < 1) continue;

            foreach (var colValue in blockedGridValDict[blockedData.Key])
            {
                Debug.Log($"ColValue: {colValue}");
                if (nodeManagerEditor.IsNodeAvailableInGrid(blockedData.Key, colValue, out NodeEditor nodeEditor))
                {
                    Debug.Log($"NodeEditor: {nodeEditor.name}, row: {nodeEditor.Row}, col: {nodeEditor.Col}");
                    nodeEditor.ToggleMeshState(state);
                }
            }
        }
    }

    public void ClearData()
    {
        m_Rows = 0;
        m_RowPosition = 0;
        m_Cols = 0;
        m_PreCols = 0;
        m_BaseColCount = 0;
        blockedGridValDict.Clear();
    }

    public void GenerateJsonUsingGridData()
    {
        string path = "";
        string dir = "Assets/Data/Json/LevelsJson";

        if (overriteJson)
        {
            path = $"{dir}/{customJsonTextAsset.name}.json";
            File.WriteAllText(path, JsonConvert.SerializeObject(m_GridData));
            overriteJson = false;
            return;
        }

        int fileCount = Directory.GetFiles(dir, "*.json").Length;
        int counter = fileCount < 1 ? 1 : fileCount + 1;

        path = $"{dir}/Level{counter}.json";
        File.WriteAllText(path, JsonConvert.SerializeObject(m_GridData));
    }
}
