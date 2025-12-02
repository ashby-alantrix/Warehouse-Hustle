#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

public class NodeManagerEditor : MonoBehaviour
{
    [SerializeField] private GridEditorManager gridEditorManager;

    private Dictionary<string, NodeEditor> nodesData = new Dictionary<string, NodeEditor>();
    private Dictionary<string, NodeEditor> nodesData1 = new Dictionary<string, NodeEditor>();

    public void ClearNodesData()
    {
        nodesData.Clear();
    }

    public bool IsNodeAvailableInGrid(int row, int col, out NodeEditor node)
    {
        Debug.Log($"row: {row}, col: {col}");
        string pos = GetNodeKeyForm(row, col);
        Debug.Log($"pos: {pos}");
        node = nodesData.ContainsKey(pos) ? nodesData[pos] : null;
        return nodesData.ContainsKey(pos);
    }

    public void AddNodeInstance(NodeEditor nodeInstTest, int row, int col)
    {
        Debug.Log($"AddNodeInstance for editor specific code: row: {row}, col: {col}, nodeInstTest: {nodeInstTest.name}");
        nodeInstTest.InitGridManagerTest(gridEditorManager, row, col);

        nodesData.Add(GetNodeKeyForm(row, col), nodeInstTest);
    }

    private string GetNodeKeyForm(int row, int col)
    {
        return $"{row},{col}";
    }
}
#endif