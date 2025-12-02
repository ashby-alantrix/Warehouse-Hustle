using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GoodsPlacementManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private GoodsSortingManager goodsSortingManager;
    [SerializeField] private float tweenDelay = 1f;

    private bool canPlaceGoods = true;

    private NodeManager nodeManager;
    private SoundManager soundManager;
    public bool CanPlaceGoods => canPlaceGoods;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsPlacementManager>(this);
    }

    public void InitializeData()
    {
        soundManager = InterfaceManager.Instance?.GetInterfaceInstance<SoundManager>();
    }

    public void PlaceGoodsInsideNode(Node selectedNode)
    {
        selectedNode.InitItemsData();
        var totalItems = selectedNode.GetItemBaseCount();

        canPlaceGoods = false;
        IterateAndMoveNodesUsingDictionary(selectedNode, totalItems, onComplete: () =>
        {
            canPlaceGoods = true;
            goodsSortingManager.CheckNeighbors(selectedNode);
        });
    }

    private void IterateAndMoveNodesUsingDictionary(Node selectedNode, int totalItems, Action onComplete = null)
    {
        Tween nodesMoverTween = null;

        var keys = selectedNode.GetKeysForItems();
        NodePlacementData nodePlacementData = null;
        int counter = 0, customIndexer = -1;
        ItemBase itemBase = null;

        for (int indexI = 0; indexI < totalItems; indexI++) // 0 1 2 3 // 4 5 6 7 // 8 9 10 11 //
        {
            customIndexer++;
            nodePlacementData = selectedNode.RetrieveNodePlacementData(indexI);
            nodePlacementData.isOccupied = true;

            itemBase = selectedNode.GetItemBase(customIndexer, keys[counter]);
            itemBase.nodePlacementIndex = indexI;
            nodesMoverTween = itemBase.transform.DOMove(nodePlacementData.transform.position, tweenDelay);

            if (customIndexer == selectedNode.GetSetsCountForItemType(keys[counter]) - 1) // 3 == 4 - 1
            {
                customIndexer = -1;
                counter++;
            }
        }

        nodesMoverTween.OnComplete(() =>
        {
            Debug.Log($"NodesMoverTween complete");
            onComplete?.Invoke();
            selectedNode.UpdateOccupiedSlotsState();
            selectedNode.UpdateOccupiedNodes();
            KillTween();
        });

        void KillTween()
        {
            nodesMoverTween.Kill();
        }
    }

    public void RearrangeGoodsBetweenSelectedNodeAndNeighbor(ItemType itemType, Node target, Node source, bool hasCachedKey = false)
    {
        // sorting is required if using goods set count instead of item base count as truth
        // or cache the item bases as well

        goodsSortingManager.isSortingInProgress = true;
        soundManager.PlayPrimaryGameSoundClip(SoundType.Swap);

        Tweener tweener = null;
        ItemBase itemBase = null;

        if (source.HasCachedItemType(itemType) && source.GetCachedItemBase(itemType) != null)
            Debug.Log($"Got cached itemss");

        var currentItemBases = hasCachedKey ? source.GetCachedItemBase(itemType) : source.GetSpecificItems(itemType);
        var targetItemBaseCount = target.GetItemBaseCount();

        Debug.Log($"Test 11: itemBases: {currentItemBases.Count}");
        Debug.Log($"Test 11: itemType: {itemType}");

        var additionalCount = targetItemBaseCount + currentItemBases.Count;
        var totalSlots = target.GetTotalSlotsInNode();
        additionalCount = additionalCount > totalSlots ? totalSlots : additionalCount; 

        Debug.Log($"Test 11: targetItemBaseCount: {targetItemBaseCount}");
        Debug.Log($"Test 11: additionalCount: {additionalCount}");

        for (int indexJ = targetItemBaseCount; indexJ < additionalCount; indexJ++) // TODO :: logic needs to be updated
        {
            NodePlacementData nodePlacementData = target.RetrieveNodePlacementData(indexJ);
            Debug.Log($"nodePlacementData.isOccupied: {nodePlacementData.isOccupied}");
            if (!nodePlacementData.isOccupied) // change the state periodically
            {
                itemBase = currentItemBases[indexJ - targetItemBaseCount];
                tweener = itemBase.transform.DOMove(nodePlacementData.transform.position, 1f);
            }
        }

        tweener.OnComplete(() =>
        {
            Debug.Log($"MoveMatchedSetToTarget OnComplete");
            Debug.Log($"MoveMatchedSetToTarget OnComplete: source :: name: {source.transform.name}, pos: {source.transform.position}");
            Debug.Log($"MoveMatchedSetToTarget OnComplete: target :: name: {target.transform.name}, pos: {target.transform.position}");

            source.UpdateOccupiedNodes();
            target.UpdateOccupiedNodes();

            source.CheckIfNodeIsFullOrCleared();
            target.CheckIfNodeIsFullOrCleared();

            if (!source.IsNodeFullOrCleared() && !target.IsNodeFullOrCleared())
                CheckGameOver();

            KillTweener();
        });
        
        void KillTweener()
        {
            tweener.Kill();
        }
    }

    private void CheckGameOver()
    {
        goodsSortingManager = goodsSortingManager == null ? InterfaceManager.Instance.GetInterfaceInstance<GoodsSortingManager>() : goodsSortingManager;
        nodeManager = nodeManager == null ? InterfaceManager.Instance.GetInterfaceInstance<NodeManager>() : nodeManager;

        Debug.Log($"goodsSortingManager.generalSortingState: {goodsSortingManager.generalSortingState}");
        nodeManager.LogNodeValue();

        if (nodeManager.AreAllNodesOccupied())
        {
            goodsSortingManager.isSortingInProgress = false;
            Debug.Log($"GameOverCheck :: isSortingInProgress: {goodsSortingManager.isSortingInProgress} in GoodsPlacementManager");
            Debug.Log($"GameOverCheck :: CheckGameOverCondition GoodsPlacementManager");
            goodsSortingManager.CheckGameOverCondition($"Node: {name}");
        }
    }

    public void RearrangeBasedOnSorting(Node currentNode)
    {
        var itemBaseCount = currentNode.GetItemBaseCount();
        Debug.Log($"Sorting :: itemBaseCount: {itemBaseCount}");

        IterateAndMoveNodesUsingDictionary(currentNode, itemBaseCount, null);
    }
}
