using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelPage : MonoBehaviour
{
    [SerializeField] private Level[] levelObjects;
    [SerializeField] private Transform[] levelObjectPoints;

    private LevelManager levelManager;
    private int levelObjectPointStartIndex = 0;

    private Queue<Level> levelsQueue = new Queue<Level>();

    void Awake()
    {
        foreach (var levelObject in levelObjects)
            levelsQueue.Enqueue(levelObject);
    }

    public void OnLevelComplete()
    {
        
    }

    public void TriggerAnim()
    {
        SetLevelManager();

        Tween tweener = null;
        Debug.Log($"CurrentLevelNumber: {levelManager.CurrentLevelNumber}");
        levelObjectPointStartIndex = levelManager.CurrentLevelNumber == 1 ? 1 : 0;
        var indexer = levelObjectPointStartIndex;
        
        foreach (var levelObject in levelsQueue)
        {
            tweener = levelObject.transform.DOMove(levelObject.transform.position + new Vector3(0f, 100f, 0), 0.5f);
        }

        tweener.OnComplete(() =>
        {
            Tween tweener1 = null;
            levelObjectPointStartIndex = levelManager.CurrentLevelNumber == 1 ? 1 : 0;
            var indexer = levelObjectPointStartIndex;

            foreach (var levelObject in levelsQueue)
            {
                Debug.Log($"Indexer: {indexer}");
                tweener1 = levelObject.transform.DOMove(levelObjectPoints[indexer++].position, 1f);
            }

            tweener1.OnComplete(() =>
            {
                Tween tweener2 = null;
                levelObjectPointStartIndex = levelManager.CurrentLevelNumber == 1 ? 1 : 0;
                var indexer = levelObjectPointStartIndex;

                foreach (var levelObject in levelsQueue)
                {
                    tweener2 = levelObject.transform.DOMove(levelObjectPoints[indexer++].position + new Vector3(0f, 100f, 0), 0.5f);
                }

                tweener2.OnComplete(() => 
                {
                    if (levelManager.CurrentLevelNumber > 1)
                    {
                        var dequeuedLevel = levelsQueue.Dequeue();
                        dequeuedLevel.gameObject.SetActive(false);
                        dequeuedLevel.transform.position = levelObjectPoints[levelObjectPoints.Length - 1].position;
                        levelsQueue.Enqueue(dequeuedLevel);
                        dequeuedLevel.gameObject.SetActive(true);
                    }
                    levelManager.SetCurrentLevelNumber(levelManager.CurrentLevelNumber + 1);
                });
            });
        });

        
    }

    public void SetLevelManager()
    {
        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
    }
}
