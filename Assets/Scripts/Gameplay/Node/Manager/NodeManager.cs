using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private HexData[] m_HexDatas;
    [SerializeField] private GoodsPlacementManager goodsPlacementManager;

    private GoodsManager m_GoodsManager;
    private Dictionary<string, Node> nodesData = new Dictionary<string, Node>();

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<NodeManager>(this);
    }

    public bool IsNeighborNodeAvailableInGrid(string pos, out Node node)
    {
        Debug.Log($"### test4 node: IsNeighborsNodeAvailable");
        node = nodesData.ContainsKey(pos) ? nodesData[pos] : null;
        Debug.Log($"### test4 node: {node}");
        Debug.Log($"### test4 nodesData.ContainsKey(pos): {nodesData.ContainsKey(pos)}");
        return nodesData.ContainsKey(pos);
    }

    public void AddNodeInstance(GameObject instance)
    {
        var nodeInst = instance.GetComponent<Node>();
        nodeInst.InitNodeManager(this);
        nodesData.Add(instance.transform.position.ToString(), nodeInst);
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

    public void OnNodeClicked(Node selectedNode)
    {
        SetGoodsPlacementManager();
        if (!goodsPlacementManager.CanPlaceGoods) return;

        selectedNode.SetNodeOccupiedState(true);
        
        goodsPlacementManager.PlaceGoodsInsideNode(selectedNode);

        SetGoodsManager();
        m_GoodsManager.GoodsHandler.UpdateGoodsInputPlatform();
    }

    private void SetGoodsPlacementManager()
    {
        goodsPlacementManager = goodsPlacementManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsPlacementManager>() : goodsPlacementManager;
    }

    private void SetGoodsManager()
    {
        m_GoodsManager = m_GoodsManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsManager>() : m_GoodsManager;
    }
}
