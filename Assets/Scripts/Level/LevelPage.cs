using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LevelPage : MonoBehaviour
{
    [SerializeField] private Level[] levelObjects;
    [SerializeField] private Level currentFinishedLvl, newUnlockedLvl;

    [SerializeField] private Transform[] levelObjectPoints;
    [SerializeField] private Transform currentFinishedLvlTransform;
    [SerializeField] private Transform newUnlockedLvlTransform;

    [SerializeField] private Vector3 startAdditionalPos = new Vector3(0f, 50f, 0);
    [SerializeField] private Vector3 finalAdditionalPos = new Vector3(0f, 25f, 0);

    [SerializeField] private float startTweenDelay = 0.25f;
    [SerializeField] private float finalTweenDelay = 0.25f;
    [SerializeField] private float levelObjectSlideDelay = 1f;
    
    [Tooltip("Difference between center node/active level node and last node visible on top of screen")]
    [SerializeField] private int diff;

    private LevelManager levelManager;
    private int levelObjectPointStartIndex = 0;

    private Queue<Level> levelsQueue = new Queue<Level>();
    private Dictionary<string, Level> levelsDict = new Dictionary<string, Level>();

    void Awake()
    {
        foreach (var levelObject in levelObjects)
            levelsQueue.Enqueue(levelObject);
    }

    public void InitLevelManager(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public void InitLevelObjects()
    {
        int levelStartVal = levelManager.CurrentLevelNumber == 1 ? 1 : levelManager.CurrentLevelNumber;
        for (int indexI = levelStartVal; indexI <= levelObjects.Length; indexI++)
        {
            levelObjects[indexI - 1].SetLevelText(indexI);
        }
    }

    public void OnLevelComplete()
    {
        if (levelManager.CurrentLevelNumber == 1)
        {
            currentFinishedLvl.OnLevelCompleted();
            newUnlockedLvl.OnLevelUnlocked();
            TriggerAnim();
        }    
        else if (levelManager.CurrentLevelNumber <= levelManager.TotalLevelsCount)
        {
            string lvlPos = $"{currentFinishedLvlTransform.position + finalAdditionalPos}";
            if (levelsDict.ContainsKey(lvlPos))
            {
                currentFinishedLvl = levelsDict[lvlPos];
                currentFinishedLvl.OnLevelCompleted();
            }
            
            lvlPos = $"{newUnlockedLvlTransform.position + finalAdditionalPos}";
            if (levelsDict.ContainsKey(lvlPos))
            {
                newUnlockedLvl = levelsDict[lvlPos];
                newUnlockedLvl.OnLevelUnlocked();
            }

            TriggerAnim();
        }
    }

    public void TriggerAnim()
    {
        Tween tween = null, tween1 = null, tween2 = null;

        StartTween(out tween, additionalPos: startAdditionalPos, delay: startTweenDelay, useOffset: true);
        tween.OnComplete(() =>
        {
            // // tween.Kill();
            StartTween(out tween1, additionalPos: Vector3.zero, levelObjectSlideDelay, useOffset: false);
            tween1.OnComplete(() =>
            {
                StartTween(out tween2, additionalPos: finalAdditionalPos, finalTweenDelay, useOffset: false);
                tween2.OnComplete(() =>
                {
                    Level lastLevel = levelsQueue.Last();
                    Level dequeuedLevel = null;

                    if (levelManager.CurrentLevelNumber > 1)
                    {
                        dequeuedLevel = levelsQueue.Dequeue();
                        dequeuedLevel.gameObject.SetActive(false);
                        dequeuedLevel.transform.position = levelObjectPoints[levelObjectPoints.Length - 1].position;
                        
                        if (lastLevel.LevelNum != levelManager.TotalLevelsCount && !lastLevel.HasBarricade)
                        {
                            levelsQueue.Enqueue(dequeuedLevel);
                            dequeuedLevel.gameObject.SetActive(true);
                            dequeuedLevel.SetLevelText(lastLevel.LevelNum + 1);
                        }
                    }

                    if (levelManager.CurrentLevelNumber < levelManager.TotalLevelsCount) // TODO :: REMOVE AFTER TEST
                        levelManager.SetCurrentLevelNumber(levelManager.CurrentLevelNumber + 1); // TODO :: REMOVE AFTER TEST

                    if (!newUnlockedLvl.HasBarricade)
                        newUnlockedLvl.TogglePlayBtnState(true);
                    else 
                        newUnlockedLvl.ShowRestartButton();
                        
                    if (newUnlockedLvl.LevelNum == levelManager.TotalLevelsCount - diff)
                    {
                        levelsQueue.Enqueue(dequeuedLevel);
                        dequeuedLevel.gameObject.SetActive(true);
                        dequeuedLevel.SetLevelBarricade();
                    }

                    foreach (var level in levelsQueue)
                    {
                        if (levelsDict.ContainsKey($"{level.transform.position}"))
                            levelsDict[$"{level.transform.position}"] = level;
                        else 
                            levelsDict.Add($"{level.transform.position}", level);
                    }
                });
            });
        });
    }

    private void StartTween(out Tween tween, Vector3 additionalPos, float delay, bool useOffset = false)
    {
        tween = null;
        Debug.Log($"CurrentLevelNumber: {levelManager.CurrentLevelNumber}");
        levelObjectPointStartIndex = levelManager.CurrentLevelNumber == 1 ? 1 : 0;
        var indexer = levelObjectPointStartIndex;

        foreach (var levelObject in levelsQueue)
        {
            tween = levelObject.transform.DOMove(useOffset ? levelObject.transform.position + additionalPos : levelObjectPoints[indexer++].position + additionalPos, delay);
        }
    }
}
