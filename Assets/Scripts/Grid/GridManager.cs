using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class GridManager : MonoBehaviour, IDataLoader, IBase, IBootLoader
{
    [SerializeField] private NodeManager m_NodeManager;

    [SerializeField] private GameObject m_HexNode;
    [SerializeField] private GameObject m_NodesParent;

    [SerializeField] private LevelsSO levelsSO;

    [SerializeField] private bool useCustom = false;
    [SerializeField] private TextAsset customLevelJson;

    private int m_Rows; // z
    private int m_Cols; // x

    private float savedStartPointValue = 0;
    private float baseNodesThreshold = 4;
    private float m_RowPosition; // z
    private float m_PreCols;
    private float m_BaseColCount;

    private int tempCounter = 0;

    private GridData m_GridData;
    private LevelManager levelManager;

    public Transform NodesParent => m_NodesParent.transform;
    public NodeManager NodeManager => m_NodeManager;

    private Dictionary<float, List<float>> blockedGridValDict = new Dictionary<float, List<float>>();

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GridManager>(this);
    }

    public void InitializeData()
    {
        if (useCustom)
        {
            m_GridData = JsonConvert.DeserializeObject<GridData>(customLevelJson.text);
        }
        else 
        {
            levelManager = InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>();
            m_GridData = JsonConvert.DeserializeObject<GridData>(levelsSO.GetLevelJson(levelManager.CurrentLevelNumber));

            Debug.Log($"GridData: {JsonConvert.SerializeObject(m_GridData)}");
        }
        
        Init();
    }

    public void Init()
    {
        InitGridData();
        m_NodeManager.InitNeighborsToNodes();
    }

    public void InitGridInfo(GridData gridData)
    {
        m_GridData = gridData;

        Debug.Log($"GridData: {JsonConvert.SerializeObject(m_GridData)}");
    }

    public void InitGridData()
    {
        Debug.Log($"Init grid data");
        var nodeOffset = (m_HexNode.transform.localScale.z / 2) + (m_HexNode.transform.localScale.z / 4); // 0.75
        // savedStartPointValue = GetStartPointVal();
        Debug.Log($"SavedStartPointVal: {savedStartPointValue}");
        bool hasSet = false;

        foreach (var nodeInfo in m_GridData.nodeInfos)
        {
            // savedStartPointValue = 0;
            // if (!hasSet)
            // {
            //     hasSet = true;
            //     savedStartPointValue = GetStartPointVal(m_Cols);
            // }

            m_PreCols = m_Cols;

            m_Rows = nodeInfo.gridValues.row;
            m_Cols = nodeInfo.gridValues.col;

            var blockedGridValLength = nodeInfo.blockedGridValues.Count;
            if (!blockedGridValDict.ContainsKey(m_Rows))
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
        float startPointVal = savedStartPointValue;
        Debug.Log($"{m_Rows} SavedStartPointVal1: {savedStartPointValue}");
        float extraNodeCount = 0;
        float diff = 0;
        float times = 0;

        if (m_PreCols != 0)
        {
            if (/*m_PreCols > m_Cols && */ m_Cols <= m_BaseColCount) // row1: 4 elements, row2: 3 elements
            {
                startPointVal += 0.5f;
            }
            else if (m_Cols > m_PreCols || m_Cols > m_BaseColCount) // row1: 4 elements, row2: 5 elements
            {
                startPointVal += -0.5f;
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

                startPointVal = savedStartPointValue;
            }

            // float absVal = Mathf.Abs(m_PreCols - m_Cols); // 1 // 3 // 5
            // float diff = absVal > 1 ? (extraNodeCount/2) : absVal - 1; // 0 // 1 // 3
            //diff = extraNodeCount / 2; // 0 // 1 // 3

            //times = Mathf.Sign(startPointVal) * diff; // 0 // -1

            Debug.Log($"SavedStartPointVal2 times: {times}");
            startPointVal = startPointVal + times; // -0.5 // -1.5 // -3.5
        }
        else
        {
            m_BaseColCount = m_Cols;
        }

        for (int j = 0; j < m_Cols; j++)
        {
            var instance = Instantiate(m_HexNode, new Vector3(j + startPointVal, 0, m_RowPosition), Quaternion.identity);
            tempCounter++;
            instance.transform.name = $"{instance.transform.name} {tempCounter}";
            instance.transform.SetParent(m_NodesParent.transform);

            if (blockedGridValDict.ContainsKey(m_Rows) && blockedGridValDict[m_Rows].Contains(j + 1))
            {
                instance.SetActive(false);
                m_NodeManager.AddNodeInstance(instance, m_Rows, j + 1);
                continue;
            }
            
            m_NodeManager.AddNodeInstance(instance, m_Rows, j + 1);
        }
    }

    public Vector3 GetGridCenterPoint()
    {
        NodeInfo nodeInfo = m_GridData.nodeInfos.FirstOrDefault();

        int median = nodeInfo.gridValues.col / 2;

        Debug.Log($"GetGridCenterPoint: median {median}");
        Vector3 offsetPos = Vector3.zero;
        if (nodeInfo.gridValues.col % 2 == 0)
        {
            offsetPos.x = 0.5f;
        }
        else
        {
            median += 1;
        }
        Debug.Log($"GetGridCenterPoint: offsetPos {offsetPos}");
        Debug.Log($"GetGridCenterPoint: median {median}");
 
        Vector3 gridCenterPoint = m_NodeManager.IterateAndRetreiveNodeInstance(startIndex: 0, endIndex: median) + offsetPos;
        return gridCenterPoint;
    }
}
