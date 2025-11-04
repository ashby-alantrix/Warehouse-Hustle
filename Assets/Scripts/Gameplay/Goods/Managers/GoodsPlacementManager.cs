using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GoodsPlacementManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private GoodsSortingManager goodsSortingManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsPlacementManager>(this);
    }

    public void PlaceGoodsInsideNode(Node selectedNode)
    {
        selectedNode.InitItemsData();
        NodePlacementData nodePlacementData = null;
        Tween nodesMoverTween = null;
        var itemBaseCount = selectedNode.GetItemBaseCount();
        int counter = 0, indexer = 0;

        var keys = selectedNode.GetKeysForItems().ToList(); // Weapon, Armour, Boots

        for (int indexI = 0; indexI < itemBaseCount; indexI++) // 0 1 2 3 (4) // 4 5 6 // 7 8 9 //
        {
            nodePlacementData = selectedNode.RetrieveNodePlacementData(indexI);
            nodePlacementData.isOccupied = true;

            nodesMoverTween = selectedNode.GetItemBase(indexer, keys[counter]).transform.DOMove(nodePlacementData.transform.position, 1f);

            indexer++;
            if (indexer == selectedNode.GetSetsCountForItemType(keys[counter]) - 1) // 3 == 4 - 1
            {
                indexer = 0;
                counter++;
            }
        }

        // nodesMoverTween.OnComplete(() => goodsSortingManager.CheckNeibhours(selectedNode));
    }

    public void RearrangeGoodsBetweenSelectedNodeAndNeighbor(ItemType itemType, Node selectedNode, Node neighborNode)
    {
        var itemBases = neighborNode.GetSpecificItems(itemType);
        var itemBaseCount = selectedNode.GetItemBaseCount();
        var totalSlots = selectedNode.GetTotalSlotsInNode;

        for (int indexJ = itemBaseCount; indexJ < totalSlots; indexJ++)
        {
            NodePlacementData nodePlacementData = selectedNode.RetrieveNodePlacementData(indexJ);
            if (!nodePlacementData.isOccupied)
            {
                var itemBase = itemBases[indexJ - itemBaseCount];
                itemBase.transform.DOMove(nodePlacementData.transform.position, 1f);
            }
        }
    }
}
