using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private HexData[] m_HexDatas;
    [SerializeField] private GoodsPlacementManager goodsPlacementManager;

    private GoodsManager m_GoodsManager;
    private LevelManager levelManager;
    private TrucksLoaderManager trucksLoaderManager;
    private Dictionary<string, Node> nodesData = new Dictionary<string, Node>();

    private int totalNodesInGrid = 0;
    private int totalOccupiedNodes = 0;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<NodeManager>(this);
        SetLevelManager();
    }

    public void UpdateOccupiedNodesCount(int counter)
    {
        totalOccupiedNodes += counter;
        CheckIfNodesAreLeft();
        Debug.Log($"totalOccupiedNodes: {totalOccupiedNodes}");
    }

    private void SetLevelManager()
    {
        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
    }

    public void ClearNodesData()
    {
        nodesData.Clear();
    }

    public bool IsNodeAvailableInGrid(string pos, out Node node)
    {
        node = nodesData.ContainsKey(pos) ? nodesData[pos] : null;
        return nodesData.ContainsKey(pos) && nodesData[pos].gameObject.activeInHierarchy;
    }

    public void AddNodeInstance(GameObject instance, int row, int col)
    {
        var nodeInst = instance.GetComponent<Node>();

        nodeInst.InitNodeManager(this);
        nodesData.Add(instance.transform.position.ToString(), nodeInst);
        if (instance.gameObject.activeInHierarchy)
            totalNodesInGrid++;
    }

    public Vector3 IterateAndRetreiveNodeInstance(int startIndex, int endIndex)
    {
        var nodes = nodesData.Values.ToList();
        for (int indexI = startIndex; indexI < endIndex; indexI++)
        {
            Debug.Log($"GetGridCenterPoint: {indexI}");
            if (indexI == endIndex - 1)
            {
                Debug.Log($"nodes[indexI].transform.position: {nodes[indexI].transform.position}");
                return nodes[indexI].transform.position;
            }
        }

        return Vector3.zero;
    }

    public void InitNeighborsToNodes()
    {
        Vector3 tempHexOffset = Vector3.zero;
        Vector3 addedHexOffset = Vector3.zero;
        Node node = null;

        foreach (var nodeData in nodesData)
        {
            node = nodeData.Value;

            foreach (var hexData in m_HexDatas)
            {
                tempHexOffset.x = hexData.offset.x;
                tempHexOffset.z = hexData.offset.z;
                addedHexOffset = node.transform.position + tempHexOffset;

                // check if node is available at the addedHexOffset position
                if (nodesData.ContainsKey(addedHexOffset.ToString())) // to only the add the nodes that are present in the grid, blocked ones aren't added
                {
                    node.AddNeighborsData(addedHexOffset);
                }
            }
        }
    }

    public void OnNodeClickedOrFound(Node selectedNode)
    {
        SetGoodsPlacementManager();
        if (goodsPlacementManager && !goodsPlacementManager.CanPlaceGoods || !levelManager.CanPlayLevel) return;

        selectedNode.SetNodeOccupiedState(true);
        
        goodsPlacementManager.PlaceGoodsInsideNode(selectedNode);

        SetGoodsManager();
        m_GoodsManager.GoodsHandler.UpdateGoodsInputPlatform();
    }

    public void OnNodeFilled(Node filledNode, ItemType filledKey)
    {
        SetTrucksLoaderManager();

        trucksLoaderManager.LoadOrStoreNextGoods(filledNode.GetSpecificItems(filledKey), filledKey);
    }

    private void SetTrucksLoaderManager()
    {
        trucksLoaderManager = trucksLoaderManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<TrucksLoaderManager>() : trucksLoaderManager;
    }

    private void SetGoodsPlacementManager()
    {
        goodsPlacementManager = goodsPlacementManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsPlacementManager>() : goodsPlacementManager;
    }

    private void SetGoodsManager()
    {
        m_GoodsManager = m_GoodsManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsManager>() : m_GoodsManager;
    }

    private void CheckIfNodesAreLeft()
    {
        Debug.Log($"totalOccupiedNodes: totalNodesInGrid: {totalNodesInGrid}, totalOccupiedNodes: {totalOccupiedNodes}");
        if (totalNodesInGrid == totalOccupiedNodes)
        {
            Invoke(nameof(OnAllNodesOccupied), 2f);
        }
    }

    private void OnAllNodesOccupied()
    {
        levelManager.OnLevelStateChange(LevelState.Lost);
    }
}
