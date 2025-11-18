using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemLoadData
{
    public ItemType itemType;
    public List<ItemBase> itemBases;
    public GoodsQueuerData goodsQueuerData;
}

public class TrucksLoaderManager : MonoBehaviour, IBootLoader, IBase
{
    [SerializeField] private GoodsWaitingQueuer goodsWaitingQueuer;
    [SerializeField] private GameObject truckPrefab;
    [SerializeField] private Transform spawnStartPoint;
    [SerializeField] private Transform newTruckSpawnPoint;
    [SerializeField] private Transform truckDestPoint;
    [SerializeField] private Vector3 spawnOffset;

    [SerializeField] private int spawnCount;
    [SerializeField] private float trucksMoverInterval = 1f;
    [SerializeField] private float goodsMoverInterval = 1f;
    
    private Vector3 currentSpawnPos = Vector3.zero;
    private ItemType currentGoodsTypeToFill;
    private TruckBase currentActiveTruck;
    private ObjectPoolManager objectPoolManager;

    private List<Vector3> spawnPoints = new List<Vector3>(); // static 
    private List<TruckBase> truckBases = new List<TruckBase>(); // dynamic

    private List<ItemLoadData> itemLoadDatas = new List<ItemLoadData>();

    private List<ItemBase> currentLoadingItemBases = new List<ItemBase>();

    private int goodsInQueueCounter = 0;
    private bool isLoadingInProcess = false;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<TrucksLoaderManager>(this);

        currentSpawnPos = spawnStartPoint.position;
        SetObjectPoolManager();
        SpawnTrucks();
    }

    public void LoadOrStoreNextGoods(List<ItemBase> itemBases, ItemType goodsTypeToFill)
    {
        if (isLoadingInProcess)
        {
            GoodsQueuerData goodsQueuerData = null;
            goodsWaitingQueuer.InitGoods(itemBases, out goodsQueuerData);

            itemLoadDatas.Add(new ItemLoadData
            {
                itemType = goodsTypeToFill,
                itemBases = itemBases,
                goodsQueuerData = goodsQueuerData
            });
        }
        else
        {
            currentLoadingItemBases = itemBases;
            currentGoodsTypeToFill = goodsTypeToFill;

            LoadGoodsOntoTruck();
        }
    }

    public void LoadGoodsOntoTruck()
    {
        isLoadingInProcess = true;
        Tween truckLoadingTween = null;
        currentActiveTruck = truckBases[0];
        truckBases.RemoveAt(0); // 5

        for (int indexI = 0; indexI < currentLoadingItemBases.Count; indexI++)
        {
            truckLoadingTween = currentLoadingItemBases[indexI].transform.DOMove(currentActiveTruck.TruckGoodsLoader.SlotsPlacer.GetPosDataBasedOnIndex(indexI), goodsMoverInterval);
            currentLoadingItemBases[indexI].transform.parent = currentActiveTruck.transform;
        }

        truckLoadingTween.OnComplete(() => OnCurrentTruckLoadingComplete());
    }

    private void OnCurrentTruckLoadingComplete()
    {
        currentActiveTruck.transform.DOMove(truckDestPoint.position, 1f).OnComplete(() =>
        {
            foreach (var itemBase in currentLoadingItemBases)
            {
                objectPoolManager.PassObjectToPool($"{currentGoodsTypeToFill}", PoolType.Item, itemBase);
                itemBase.gameObject.SetActive(false);
            }

            objectPoolManager.PassObjectToPool($"{currentActiveTruck.TruckType}", PoolType.Truck, currentActiveTruck);
        });

        Tweener trucksMoverTween = null;
        for (int indexI = 0; indexI < truckBases.Count; indexI++)
        {
            trucksMoverTween = truckBases[indexI].transform.DOMove(spawnPoints[indexI], 1f);
        }

        trucksMoverTween.OnComplete(() =>
        {
            isLoadingInProcess = false;
            if (itemLoadDatas.Count > 0)
            {
                currentLoadingItemBases = itemLoadDatas[0].itemBases;
                currentGoodsTypeToFill = itemLoadDatas[0].itemType;
                itemLoadDatas[0].goodsQueuerData.isOccupied = false;
                itemLoadDatas.RemoveAt(0);

                LoadGoodsOntoTruck();
            }
        });

        TruckBase newTruckBase = objectPoolManager.GetObjectFromPool<TruckBase>($"{currentActiveTruck.TruckType}", PoolType.Truck);
        newTruckBase.transform.position = newTruckSpawnPoint.position;
        newTruckBase.gameObject.SetActive(true);

        newTruckBase.transform.DOMove(spawnPoints[spawnPoints.Count - 1], 1f);        
        truckBases.Add(newTruckBase);
    }

    private void SetObjectPoolManager()
    {
        objectPoolManager = objectPoolManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>() : objectPoolManager;
    }

    private void SpawnTrucks()
    {
        for (int indexI = 0; indexI < spawnCount; indexI++)
        {
            truckBases.Add(objectPoolManager.GetObjectFromPool<TruckBase>($"{TruckType.Truck1}", PoolType.Truck));

            truckBases[indexI].transform.position = currentSpawnPos;
            truckBases[indexI].gameObject.SetActive(true);

            spawnPoints.Add(currentSpawnPos);
            currentSpawnPos += spawnOffset;
        }

        newTruckSpawnPoint.position = currentSpawnPos;
    }

    private void SpawnNextTruck()
    {
        TruckBase truckBase = objectPoolManager.GetObjectFromPool<TruckBase>($"{TruckType.Truck1}", PoolType.Truck);
        truckBase.transform.position = newTruckSpawnPoint.position;
        truckBase.gameObject.SetActive(true);

        Tween newTween = truckBase.transform.DOMove(spawnPoints[spawnPoints.Count - 1], trucksMoverInterval);
        newTween.OnComplete(() => newTween.Kill());
    }
}
