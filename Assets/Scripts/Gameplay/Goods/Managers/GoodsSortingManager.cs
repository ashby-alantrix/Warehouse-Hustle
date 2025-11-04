using DG.Tweening;
using UnityEngine;

public class GoodsSortingManager : MonoBehaviour, IBase, IBootLoader
{
    private NodeManager nodeManager;
    private GoodsPlacementManager goodsPlacementManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsSortingManager>(this);
    }

    public void CheckNeibhours(Node selectedNode)
    {
        SetGoodsPlacementManager();
        var goodsSetDict = selectedNode.GetSetDict();

        foreach (var set in goodsSetDict) // 3 goods
        {
            ExploreNeighbors(selectedNode, set.Key);
        }
    }

    private void ExploreNeighbors(Node selectedNode, ItemType itemType)
    {
        int neighborsCount = selectedNode.GetNeighborsCount();
        int availSlots = 0;
        for (int index = 0; index < neighborsCount; index++)
        {
            if (nodeManager.IsNodeAvailable(selectedNode.GetNeighborHexOffset(index).ToString(), out Node neighborNode))
            {
                if (neighborNode.DoesNeighborHaveSimilarItem(itemType, out int itemsInNeighbor))
                {
                    // move the nodes to the neighbor: (use a recursion based approach)
                    //  -> case 1: if there are slots already available in the current node then take 
                    //             the matching nodes and move them to current node followed by 
                    //             sorting/refreshing the neighboring node and the current node
                    //  -> case 2: if there are no slots available, check the neighbor's neighbors to see if there are matching items (using recursion)

                    if (selectedNode.HasEmptySlots(out availSlots) && itemsInNeighbor <= availSlots)
                    {
                        // update datas:
                        //  -> update the goods data set in both nodes ||
                        //  -> update the goods items collection in both nodes 
                        // change the occupied props

                        neighborNode.RemoveItemsDataFromNode(itemType, itemsInNeighbor);
                        selectedNode.AddItemsDataToNode(itemType, itemsInNeighbor);

                        

                        goodsPlacementManager.RearrangeGoodsBetweenSelectedNodeAndNeighbor(itemType, selectedNode, neighborNode);
                    }
                }
            }
        }
    }
    
    private void SortAndRefreshNode()
    {
        
    }

    private void SetNodeManager()
    {
        nodeManager = nodeManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<NodeManager>() : nodeManager;
    }
    
    private void SetGoodsPlacementManager()
    {
        goodsPlacementManager = goodsPlacementManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsPlacementManager>() : goodsPlacementManager;
    }
}
