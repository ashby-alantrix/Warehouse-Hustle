using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    private GoodsManager m_GoodsManager;
    
    [SerializeField] private HexData[] m_HexDatas;

    private Dictionary<string, Node> nodesData = new Dictionary<string, Node>();

    public void AddNodeInstance(GameObject instance)
    {
        var nodeInst = instance.GetComponent<Node>();
        nodeInst.InitNodeManager(this);
        nodesData.Add(instance.transform.position.ToString(), nodeInst);
    }

    public void InitNeighboursToNodes()
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
                if (nodesData.ContainsKey(addedHexOffset.ToString()))
                {
                    node.AddNeighborsData(addedHexOffset);
                }
            }
        }
    }

    public void OnNodeClicked()
    {
        
    }
}
