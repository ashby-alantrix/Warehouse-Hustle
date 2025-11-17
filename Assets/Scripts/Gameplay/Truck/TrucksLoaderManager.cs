using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrucksLoaderManager : MonoBehaviour, IBootLoader, IBase
{
    [SerializeField] private GameObject truckPrefab;
    [SerializeField] private Transform spawnStartPoint;
    [SerializeField] private Transform newTruckSpawnPoint;
    [SerializeField] private int spawnCount;
    [SerializeField] private float trucksMoverInterval = 1f;
    [SerializeField] private float goodsMoverInterval = 1f;
    [SerializeField] private Vector3 spawnOffset;
    
    private TruckBase currentActiveTruck = null;
    private ObjectPoolManager objectPoolManager;
    private Vector3 currentSpawnPos = Vector3.zero;

    private List<Vector3> spawnPoints = new List<Vector3>(); // static 
    private List<TruckBase> truckBases = new List<TruckBase>(); // dynamic

    private Action onTruckReachedDestination;
    // private List<ItemBase> itemBases = new List<ItemBase>();

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<TrucksLoaderManager>(this);

        currentSpawnPos = spawnStartPoint.position;
        SetObjectPoolManager();
        SpawnTrucks();
    }

    public void LoadGoodsOntoTruck(List<ItemBase> itemBases, ItemType filledGoodType)
    {
        for (int indexI = 0; indexI < itemBases.Count; indexI++)
        {
            itemBases[indexI].transform.DOMove(currentActiveTruck.TruckGoodsLoader.GetPosDataBasedOnIndex(indexI), goodsMoverInterval);
        }

        onTruckReachedDestination += () => 
        {
            foreach (var itemBase in itemBases)
            {
                objectPoolManager.PassObjectToPool($"{filledGoodType}", PoolType.Item, itemBase);
                itemBase.gameObject.SetActive(false);
            }     
        };
    }

    // TODO :: Call when truck moves to end of the screen
    private void OnTruckReachedDestination()
    {
        onTruckReachedDestination?.Invoke();
        Invoke(nameof(RemovedInvokedEvent), 0.5f);
    }

    private void RemovedInvokedEvent()
    {
        onTruckReachedDestination = null;
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

        currentActiveTruck = truckBases[0];
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
