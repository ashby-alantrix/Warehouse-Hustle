using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Pool;

public class ItemLoadData
{
    public ItemType itemType;
    public List<ItemBase> itemBases;
    public GoodsQueuerData goodsQueuerData;
}

public class TrucksLoaderManager : MonoBehaviour, IBootLoader, IBase, IDataLoader
{
    [SerializeField] private float truckDestDelay = 6f;
    [SerializeField] private float truckNextPointDelay = 4f;

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
    private SoundManager soundManager;
    private LevelManager levelManager;
    private InGameUIManager inGameUIManager;

    private List<Vector3> spawnPoints = new List<Vector3>(); // static 
    private List<TruckBase> truckBases = new List<TruckBase>(); // dynamic

    private List<ItemLoadData> itemLoadDatas = new List<ItemLoadData>();

    private List<ItemBase> currentLoadingItemBases = new List<ItemBase>();

    private int goodsInQueueCounter = 0;
    private int targetGoodsToLoad = 0;
    private int loadedGoods = 0;
    private bool isLoadingInProcess = false;

    public int GetLoadedGoods() => loadedGoods;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<TrucksLoaderManager>(this);
    }

    public void InitializeData()
    {
        objectPoolManager = InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>();
        soundManager = InterfaceManager.Instance?.GetInterfaceInstance<SoundManager>();

        levelManager = InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>();
        targetGoodsToLoad = levelManager.GetCurrentLevelsInfo().targetGoodsToLoad;

        inGameUIManager = InterfaceManager.Instance?.GetInterfaceInstance<InGameUIManager>();

        currentSpawnPos = spawnStartPoint.position;

        Debug.Log($"GridCenterPoint currentSpawnPos: {currentSpawnPos}");

        SpawnTrucks();
    }

    public void LoadOrStoreNextGoods(List<ItemBase> itemBases, ItemType goodsTypeToFill)
    {
        if (loadedGoods >= targetGoodsToLoad) 
        {
            return;
        }

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
        // currentActiveTruck = truckBases[0];
        // truckBases.RemoveAt(0); // 5

        soundManager.RegisterAudioSource(SoundType.Truck_Dest_Point, truckBases[0].TruckMover.TruckAudioSource); // each time the current active truck's audio source would be registered

        for (int indexI = 0; indexI < currentLoadingItemBases.Count; indexI++)
        {
            truckLoadingTween = currentLoadingItemBases[indexI].transform.DOMove(truckBases[0].TruckGoodsLoader.SlotsPlacer.GetPosDataBasedOnIndex(indexI), goodsMoverInterval);
            currentLoadingItemBases[indexI].transform.parent = truckBases[0].transform;
        }

        truckLoadingTween.OnComplete(() => OnCurrentTruckLoadingComplete());
    }

    private void OnCurrentTruckLoadingComplete()
    {
        var currentActiveTruck = truckBases[0];
        truckBases.RemoveAt(0);
        
        loadedGoods += currentLoadingItemBases.Count;
        inGameUIManager.InGameHUDScreen.SetGoodsGoalText(loadedGoods);

        soundManager.PlaySecondaryGameSoundClip(SoundType.Truck_Dest_Point);
        var cachedLoadingItemBases = new List<ItemBase>(currentLoadingItemBases);

        currentActiveTruck.transform.DOMove(truckDestPoint.position, truckDestDelay).OnComplete(() =>
        {
            foreach (var itemBase in cachedLoadingItemBases)
            {
                // objectPoolManager.PassObjectToPool($"{currentGoodsTypeToFill}", PoolType.Item, itemBase);
                itemBase.gameObject.SetActive(false);
            }

            Debug.Log($"Reached dest point");

            currentActiveTruck.gameObject.SetActive(false);
            objectPoolManager.PassObjectToPool($"{currentActiveTruck.TruckType}", PoolType.Truck, currentActiveTruck);

            TruckBase newTruckBase = objectPoolManager.GetObjectFromPool<TruckBase>($"{currentActiveTruck.TruckType}", PoolType.Truck);
            newTruckBase.transform.position = newTruckSpawnPoint.position;
            newTruckBase.gameObject.SetActive(true);

            newTruckBase.transform.DOMove(spawnPoints[spawnPoints.Count - 1], truckNextPointDelay);        
            truckBases.Add(newTruckBase);
        });

        Tweener trucksMoverTween = null;
        soundManager.PlayPrimaryGameSoundClip(SoundType.Truck_Next_Point);

        for (int indexI = 0; indexI < truckBases.Count; indexI++)
        {
            trucksMoverTween = truckBases[indexI].transform.DOMove(spawnPoints[indexI], truckNextPointDelay);
        }

        trucksMoverTween.OnComplete(() =>
        {
            isLoadingInProcess = false;


            if (loadedGoods >= targetGoodsToLoad) 
            {
                levelManager.OnLevelStateChange(LevelState.Won);
                return;
            }
            
            if (itemLoadDatas.Count > 0)
            {
                currentLoadingItemBases = itemLoadDatas[0].itemBases;
                currentGoodsTypeToFill = itemLoadDatas[0].itemType;
                itemLoadDatas[0].goodsQueuerData.isOccupied = false;
                itemLoadDatas.RemoveAt(0);

                LoadGoodsOntoTruck();
            }
        });

        // TruckBase newTruckBase = objectPoolManager.GetObjectFromPool<TruckBase>($"{currentActiveTruck.TruckType}", PoolType.Truck);
        // newTruckBase.transform.position = newTruckSpawnPoint.position;
        // newTruckBase.gameObject.SetActive(true);

        // newTruckBase.transform.DOMove(spawnPoints[spawnPoints.Count - 1], truckNextPointDelay);        
        // truckBases.Add(newTruckBase);
    }

    private void SpawnTrucks()
    {
        for (int indexI = 0; indexI < spawnCount; indexI++)
        {
            var pooledObject = objectPoolManager.GetObjectFromPool<TruckBase>($"{TruckType.Truck1}", PoolType.Truck);
            Debug.Log($"Pooled object: {pooledObject}");
            truckBases.Add(pooledObject);

            truckBases[indexI].transform.position = currentSpawnPos;
            truckBases[indexI].gameObject.SetActive(true);

            spawnPoints.Add(currentSpawnPos);
            currentSpawnPos += spawnOffset;
        }

        newTruckSpawnPoint.position = currentSpawnPos;
    }

    public void ResetLoadedGoods()
    {
        loadedGoods = 0;
        inGameUIManager.InGameHUDScreen.SetGoodsGoalText(loadedGoods);
    }
}
