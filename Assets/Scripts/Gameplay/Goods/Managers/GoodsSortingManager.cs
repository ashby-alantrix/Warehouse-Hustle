using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class GoodsSortingManager : MonoBehaviour, IBase, IBootLoader
{
    private NodeManager nodeManager;
    private GoodsPlacementManager goodsPlacementManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsSortingManager>(this);
    }

    public void CheckNeighbors(Node selectedNode)
    {
        Debug.Log($"### Test4 CheckNeighbors");
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
        Debug.Log($"### Test4 ExploreNeighbors: " + neighborsCount);

        int availSlots = 0;
        for (int index = 0; index < neighborsCount; index++)
        {
            var isNeighborsNodeAvailable = nodeManager.IsNeighborsNodeAvailable(selectedNode.GetNeighborHexOffset(index).ToString(), out Node neighborNode);
            if (isNeighborsNodeAvailable)
            {
                Debug.Log($"### Test4 IsNeighborNodeAvailable: {neighborNode}");
                if (neighborNode.DoesNeighborHaveSimilarItem(itemType, out int itemsCountInNeighbor))
                {
                    Debug.Log($"### Test4 DoesNeighborHaveSimilarItem: {itemType}");
                    // move the nodes to the neighbor: (use a recursion based approach)
                    //  -> case 1: if there are slots already available in the current node then take 
                    //             the matching nodes and move them to current node followed by 
                    //             sorting/refreshing the neighboring node and the current node
                    //  -> case 2: if there are no slots available, check the neighbor's neighbors to see if there are matching items (using recursion)

                    if (selectedNode.HasEmptySlots(out availSlots) && itemsCountInNeighbor <= availSlots)
                    {
                        Debug.Log("### Test4 HasEmptySlots");
                        // update datas:
                        //  -> update the goods data set in both nodes ||
                        //  -> update the goods items collection in both nodes 
                        // change the occupied props

                        neighborNode.RemoveItemsDataFromNode(itemType, itemsCountInNeighbor);
                        selectedNode.AddItemsDataToNode(itemType, itemsCountInNeighbor);

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
