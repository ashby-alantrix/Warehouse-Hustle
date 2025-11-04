using DG.Tweening;
using UnityEngine;

public class GoodsPlacementManager : MonoBehaviour, IBase, IBootLoader
{
    private GoodsManager m_GoodsManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsPlacementManager>(this);
    }

    public void PlaceGoodsInsideNode(Node selectedNode)
    {
        selectedNode.InitItemsData();
        NodePlacementData nodePlacementData = null;
        Tween nodesMoverTween = null;

        for (int index = 0; index < selectedNode.GetItemBaseCount(); index++)
        {
            nodePlacementData = selectedNode.RetrieveNodePlacementData(index);
            nodePlacementData.isOccupied = true;

            nodesMoverTween = selectedNode.GetItemBase(index).transform.DOMove(nodePlacementData.transform.position, 1f);
        }

        nodesMoverTween.OnComplete(() => );
    }
}
