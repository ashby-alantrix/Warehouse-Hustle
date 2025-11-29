using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsManager : MonoBehaviour, IBootLoader, IBase 
{
    [SerializeField] private GoodsHandler m_GoodsHandler;

    public GoodsHandler GoodsHandler => m_GoodsHandler;

    private LevelManager levelManager;
    private NodeManager nodeManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsManager>(this);

        levelManager = InterfaceManager.Instance.GetInterfaceInstance<LevelManager>();
        InitGoodsInfos();
    }

    public void InitGoodsInfos()
    {
        m_GoodsHandler.InitGoodsInfo(levelManager.GetCurrentLevelsInfo().availGoodTypes);
    }

    public void ClearGoodsInNodes(int nodesToClear)
    {
        nodeManager = nodeManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<NodeManager>() : nodeManager;
        Debug.Log($"ClearRandomNodeKeys: {nodeManager.randomNodeKeys.Count}");
        var nodeKeys = nodeManager.GetRandomNodeKeys(count: nodesToClear, startIndex: 0);
        Node foundNode = null;

        Debug.Log($"NodeKeysCount: {nodeKeys.Count}");

        foreach (var nodekey in nodeKeys)
        {
            if (nodeManager.IsNodeAvailableInGrid(nodekey, out foundNode))
            {
                foundNode.ClearOrResetGoodsDataAndView();
                nodeManager.UpdateOccupiedNodes(toAdd: false, nodekey);
            }
        }
    }

    public void ClearAllGoodsInNodes()
    {
        nodeManager = nodeManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<NodeManager>() : nodeManager;
        var nodeKeys = nodeManager.AvailableNodeKeys;
        Node foundNode = null;

        foreach (var nodekey in nodeKeys)
        {
            if (nodeManager.IsNodeAvailableInGrid(nodekey, out foundNode))
            {
                foundNode.ClearOrResetGoodsDataAndView();
            }
        }

        nodeManager.ResetOccupiedNodesList();
    }

    public void ClearGoodsInputPlatforms()
    {
        m_GoodsHandler.ClearGoodsInputPlatforms();
    }
}
