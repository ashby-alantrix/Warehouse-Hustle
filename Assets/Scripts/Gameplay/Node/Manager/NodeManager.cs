using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class NodeManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private HexData[] m_HexDatas;
    [SerializeField] private GoodsPlacementManager goodsPlacementManager;

    private GoodsManager m_GoodsManager;
    private LevelManager levelManager;
    private InputManager inputManager;
    private PopupManager popupManager;
    private TrucksLoaderManager trucksLoaderManager;
    private SoundManager soundManager;

    public List<string> randomNodeKeys = new List<string>();
    private List<string> availableNodeKeys = new List<string>();
    private Dictionary<string, Node> nodesData = new Dictionary<string, Node>();

    private int totalNodesInGrid = 0;
    private HashSet<string> occupiedNodes = new HashSet<string>();

    public List<string> AvailableNodeKeys => availableNodeKeys;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<NodeManager>(this);
        SetLevelManager();

        totalNodesInGrid = nodesData.Count;
    }

    private IEnumerable<string> lastOccupiedNodes = new HashSet<string>();

    public void UpdateOccupiedNodes(bool toAdd, string nodePos = "")
    {
        // Debug.Log($"NodeData :: before occupiedNodes.Length: {occupiedNodes.Count}");
        // Debug.Log($"NodeData :: before Occupied nodes: {JsonConvert.SerializeObject(occupiedNodes)}");
        // Debug.Log($"NodeData :: before nodePos: {nodePos}");

        if (toAdd)
        {
            Debug.Log($"nodeData :: adding nodePos");
            occupiedNodes.Add(nodePos);
        }
        else if (occupiedNodes.Contains(nodePos))
        {
            Debug.Log($"nodeData :: Removing nodePos");
            occupiedNodes.Remove(nodePos);
        }

        Debug.Log($"Updating occupied nodes :: occupiedNodes.Count: {occupiedNodes.Count}");

        // Debug.Log($"NodeData :: after Occupied nodes: {JsonConvert.SerializeObject(occupiedNodes)}");
        // Debug.Log($"NodeData :: after nodePos: {nodePos}, {nodesData[nodePos].GetItemBaseCount()}, {nodesData[nodePos].name}");
        // Debug.Log($"NodeData :: after occupiedNodes.Length: {occupiedNodes.Count}");

        // Debug.Log($"NodeData :: after GetItemBaseCount: {nodesData[nodePos].GetItemBaseCount()}");
        // Debug.Log($"NodeData :: after GetTotalSlotsInNode: {nodesData[nodePos].GetTotalSlotsInNode()}");

        // Debug.Log($"NodeData :: after equality check: {nodesData[nodePos].GetItemBaseCount() == nodesData[nodePos].GetTotalSlotsInNode()}");
        // Debug.Log($"NodeData :: after GetSetKeysCount: {nodesData[nodePos].GetSetKeysCount()}");

        GoodsSortingManager goodsSortingManager = InterfaceManager.Instance.GetInterfaceInstance<GoodsSortingManager>();
        Debug.Log($"NodeDatas0 equality check: {occupiedNodes.Count == totalNodesInGrid}");
        Debug.Log($"NodeDatas0 occupiedCount: {occupiedNodes.Count}");
        Debug.Log($"NodeDatas0 occupiedCount: {lastOccupiedNodes.Count()}");
        Debug.Log($"NodeDatas0 totalNodesInGrid: {totalNodesInGrid}");

        if (occupiedNodes.Count == totalNodesInGrid)
        {
            lastOccupiedNodes.Append(nodePos);
            var addDelay = 1.5f;
            Debug.Log($"LastOccupiedNodes: occupiedCount: {occupiedNodes.Count}");
            Debug.Log($"LastOccupiedNodes: {JsonConvert.SerializeObject(lastOccupiedNodes)}");

            // Invoke(nameof(SearchLastOccupiedNodes), addDelay);
            // StartCoroutine(SearchLastOccupiedNodes());
        }
    }

/*
    private IEnumerator SearchLastOccupiedNodes()
    {
        bool hasBreak = false;
        while (occupiedNodes.Count() == totalNodesInGrid)
        {
            foreach (var pos in lastOccupiedNodes)
            {
                if (nodesData[pos].GetItemBaseCount() == 0 || nodesData[pos].GetItemBaseCount() == nodesData[pos].GetTotalSlotsInNode() && nodesData[pos].GetSetKeysCount() == 1)
                {
                    hasBreak = true;
                    break;
                }
            }
        }

        yield return new WaitForEndOfFrame();
        lastOccupiedNodes.ToList().Clear();

        Debug.Log($"NodeDatas1 :: lastOccupiedNodes.Count: {lastOccupiedNodes.Count}");
        foreach (var pos in lastOccupiedNodes)
        {
            Debug.Log($"NodeDatas1 :: Node name: {nodesData[pos].name}");
            Debug.Log($"NodeDatas1 :: nodesData[pos].GetItemBaseCount(): {nodesData[pos].GetItemBaseCount()}");
            Debug.Log($"NodeDatas1 :: nodesData[pos].GetTotalSlotsInNode(): {nodesData[pos].GetTotalSlotsInNode()}");
            Debug.Log($"NodeDatas1 :: nodesData[pos].GetSetKeysCount(): {nodesData[pos].GetSetKeysCount()}");
        }

        if (lastOccupiedNodes.Any(pos => nodesData[pos].GetItemBaseCount() == 0 || 
                                        nodesData[pos].GetItemBaseCount() == nodesData[pos].GetTotalSlotsInNode() && nodesData[pos].GetSetKeysCount() == 1))
        {
            lastOccupiedNodes.Clear();
            return;
        }

        Debug.Log($"LastOccupiedNodes: {lastOccupiedNodes.Count}");

        foreach (var pos in lastOccupiedNodes)
        {
            Debug.Log($"NodeDatas2 :: Node name: {nodesData[pos].name}");
            Debug.Log($"NodeDatas2 :: nodesData[pos].GetItemBaseCount(): {nodesData[pos].GetItemBaseCount()}");
            Debug.Log($"NodeDatas2 :: nodesData[pos].GetTotalSlotsInNode(): {nodesData[pos].GetTotalSlotsInNode()}");
            Debug.Log($"NodeDatas2 :: nodesData[pos].GetSetKeysCount(): {nodesData[pos].GetSetKeysCount()}");
        }

        GoodsSortingManager goodsSortingManager = InterfaceManager.Instance.GetInterfaceInstance<GoodsSortingManager>();
        goodsSortingManager.CheckGameOverCondition("NodeManager");

        lastOccupiedNodes.Clear();
    }
    */

    public void ResetOccupiedNodesList()
    {
        occupiedNodes.Clear();
    }

    private void SetLevelManager()
    {
        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
    }

    public bool IsNodeAvailableInGrid(string pos, out Node node)
    {
        node = nodesData.ContainsKey(pos) ? nodesData[pos] : null;
        return nodesData.ContainsKey(pos) && nodesData[pos].gameObject.activeInHierarchy;
    }

    public List<string> GetRandomNodeKeys(int count, int startIndex = 0)
    {
        if (startIndex == 0 && randomNodeKeys.Count > 0) randomNodeKeys.Clear();

        for (int i = availableNodeKeys.Count - 1; i >= 0; i--)
        {
            var randIndex = UnityEngine.Random.Range(0, i + 1);
            
            var temp = availableNodeKeys[i];
            availableNodeKeys[i] = availableNodeKeys[randIndex];
            availableNodeKeys[randIndex] = temp;
        }

        return availableNodeKeys.GetRange(0, count);
    }

    public void AddNodeInstance(GameObject instance, int row, int col)
    {
        var nodeInst = instance.GetComponent<Node>();

        nodeInst.InitNodeManager(this);
        nodesData.Add($"{instance.transform.position}", nodeInst);
        if (instance.activeInHierarchy)
            availableNodeKeys.Add($"{instance.transform.position}");

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
        inputManager = inputManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputManager>() : inputManager;
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;

        if (goodsPlacementManager && !goodsPlacementManager.CanPlaceGoods || !levelManager.CanPlayLevel || popupManager.GetActivePU()) return;

        selectedNode.SetNodeOccupiedState(true);
        soundManager = soundManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<SoundManager>() : soundManager;
        soundManager.PlayPrimaryGameSoundClip(SoundType.Node_Click);

        goodsPlacementManager.PlaceGoodsInsideNode(selectedNode);

        SetGoodsManager();
        m_GoodsManager.GoodsHandler.UpdateGoodsInputPlatform();
    }

    public void OnNodeFilled(Node filledNode, ItemType filledKey)
    {
        SetTrucksLoaderManager();

        soundManager.PlayPrimaryGameSoundClip(SoundType.Node_Filled);
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

    public void LogNodeValue()
    {
        Debug.Log($"totalNodesInGrid {totalNodesInGrid} == totalOccupiedNodes {occupiedNodes.Count}");
    }

    public bool AreAllNodesOccupied() => totalNodesInGrid == occupiedNodes.Count;

    public void ShowGameOverEmojis()
    {
        foreach (var node in nodesData.Values)
        {
            node.SetGameOverEmoji(true);
        }
    }
}
