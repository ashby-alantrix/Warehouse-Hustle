using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LevelScreen : UIBase
{
    [SerializeField] private List<string> levelTransforms = new List<string>();
    [SerializeField] private Level[] levelObjects;
    [SerializeField] private Level currentFinishedLvl, newUnlockedLvl;

    [SerializeField] private Transform[] levelObjectPoints;
    [SerializeField] private Transform currentFinishedLvlTransform;
    [SerializeField] private Transform newUnlockedLvlTransform;

    [SerializeField] private Vector3 startAdditionalSlideOffset = new Vector3(0f, 50f, 0);
    [SerializeField] private Vector3 finalAdditionalSlideOffset = new Vector3(0f, 25f, 0);

    [SerializeField] private float startTweenDelay = 0.25f;
    [SerializeField] private float finalTweenDelay = 0.25f;
    [SerializeField] private float levelObjectSlideDelay = 1f;
    
    [Tooltip("Difference between center node/active level node and last node visible on top of screen")]
    [SerializeField] private int diff;

    private LevelManager levelManager;
    private int levelObjectPointStartIndex = 0;

    private Queue<Level> levelsQueue = new Queue<Level>();

    public void InitLevelManager(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    private int GetLevelPageStartVal() => levelManager.CurrentLevelNumber - 1 < 1 ? levelManager.CurrentLevelNumber : levelManager.CurrentLevelNumber - 1;

    public void InitLevelObjects()
    {
        int levelStartVal = GetLevelPageStartVal();
        for (int indexI = 0; indexI < levelObjects.Length; indexI++)
        {
            levelObjects[indexI].SetLevelText(levelStartVal++);
        }

        StartCoroutine(ShowLevelPageAnims());
    }

    private IEnumerator ShowLevelPageAnims()
    {
        yield return new WaitUntil(() => levelManager != null && levelManager.HasInitializedLevelsData);
        
        InitLevelsQueue();

        for (int indexI = 0; indexI < levelObjects.Length; indexI++)
        {
            if (levelObjects[indexI].HasBarricade) continue;

            levelObjects[indexI].ShowUnselectedLevelView();
        }

        if (levelManager.CurrentLevelNumber == 2)
            Debug.Break();

        if (levelManager.CurrentLevelNumber > 1)
        {
            AlignLevelObjectPositions();
        }
        else // for first level
        {
            levelObjects[0].ShowSelectedLevelView();
            levelObjects[0].TogglePlayBtnState(true);
        }

        if (!levelManager.CanPlayLevel)
        {
            Debug.Log($"OnLevelComplete");
            OnLevelComplete();    
        }

        StartCoroutine(StartLevelObjectAnims());
    }

    private void AlignLevelObjectPositions()
    {
        for (int indexI = 1; indexI < levelObjectPoints.Length; indexI++)
        {
            levelObjects[indexI - 1].transform.position = levelObjectPoints[indexI].position;
        }
    }

    private void InitLevelsQueue()
    {
        Debug.Log($"GetLevelStartVal(): {GetLevelPageStartVal()}");
        var showBarricade = levelManager.CurrentLevelNumber >= levelManager.TotalLevelsCount - diff;
        var endIndex = showBarricade ? levelManager.TotalLevelsCount - (GetLevelPageStartVal() - 1) + 1 : levelObjects.Length;

        for (int indexI = 0; indexI < endIndex; indexI++)
        {
            levelsQueue.Enqueue(levelObjects[indexI]);
        }
        
        if (showBarricade)
        {
            Debug.Log($"### levelManager.CurrentLevelNumber: {levelManager.CurrentLevelNumber}, {levelManager.TotalLevelsCount - diff}");
            Level lvlObj = levelsQueue.Last();
            lvlObj.gameObject.SetActive(true);
            lvlObj.SetLevelEndBarricade();
        }
    }

    private IEnumerator StartLevelObjectAnims()
    {
        foreach (var levelObject in levelsQueue)
        {
            Debug.Log($"levelObject: {levelObject.name}");
            levelObject.PlayScaleInAnims();
            yield return new WaitForSeconds(0.25f);
        }

        // if (levelManager.CurrentLevelNumber > 1)
        if (!levelManager.CanPlayLevel)
            TriggerScrollingAnim();
    }

    private void OnLevelComplete()
    {
        if (levelManager.CurrentLevelNumber == 1)
        {
            currentFinishedLvl.ShowUnselectedLevelView();
            newUnlockedLvl.ShowSelectedLevelView();

            Debug.Log($"currentFinishedLvl: {currentFinishedLvl.LevelNum}");
            Debug.Log($"currentFinishedLvl: {newUnlockedLvl.LevelNum}");
        }
        else if (levelManager.CurrentLevelNumber <= levelManager.TotalLevelsCount)
        {
            Debug.Log($"levelQueue first: {levelsQueue.First().LevelNum}");
            var testList = levelsQueue.ToList();
            currentFinishedLvl = testList[1];
            currentFinishedLvl.ShowUnselectedLevelView();
            Debug.Log($"currentFinishedLvl: {currentFinishedLvl.LevelNum}");

            newUnlockedLvl = testList[2];
            newUnlockedLvl.ShowSelectedLevelView();
            Debug.Log($"newUnlockedLvl: {newUnlockedLvl.LevelNum}");
        }
        
        newUnlockedLvl.SetLevelText(levelManager.CurrentLevelNumber);
    }

    public void TriggerScrollingAnim()
    {
        Tween tween = null, tween1 = null, tween2 = null;

        StartTween(out tween, additionalPos: startAdditionalSlideOffset, delay: startTweenDelay, useOffset: true);
        tween.OnComplete(() =>
        {
            // // tween.Kill();
            StartTween(out tween1, additionalPos: Vector3.zero, levelObjectSlideDelay, useOffset: false);
            tween1.OnComplete(() =>
            {
                StartTween(out tween2, additionalPos: finalAdditionalSlideOffset, finalTweenDelay, useOffset: false);
                tween2.OnComplete(() =>
                {
                    Level cachedLastLevel = levelsQueue.Last();
                    Level dequeuedLevel = null;

                    if (levelManager.CurrentLevelNumber > 1)
                    {
                        dequeuedLevel = levelsQueue.Dequeue();
                        dequeuedLevel.gameObject.SetActive(false);
                        dequeuedLevel.transform.position = levelObjectPoints[levelObjectPoints.Length - 1].position;
                        
                        if (cachedLastLevel.LevelNum != levelManager.TotalLevelsCount && !cachedLastLevel.HasBarricade)
                        {
                            levelsQueue.Enqueue(dequeuedLevel);
                            dequeuedLevel.gameObject.SetActive(true);
                            dequeuedLevel.SetLevelText(cachedLastLevel.LevelNum + 1);
                        }
                    }

                    // if (levelManager.CurrentLevelNumber < levelManager.TotalLevelsCount) 
                    //     levelManager.SetCurrentLevelNumber(levelManager.CurrentLevelNumber + 1); 

                    if (!newUnlockedLvl.HasBarricade)
                    {
                        newUnlockedLvl.TogglePlayBtnState(true);
                        newUnlockedLvl.ScaleLevelButton();
                    }
                    else 
                        newUnlockedLvl.ShowRestartButton();
                        
                    // Debug.Log($"NewUnlockedLevel: {newUnlockedLvl.LevelNum}");
                    // if (newUnlockedLvl.LevelNum == levelManager.TotalLevelsCount - diff)
                    // {
                    //     levelsQueue.Enqueue(dequeuedLevel);
                    //     dequeuedLevel.gameObject.SetActive(true);
                    //     dequeuedLevel.SetLevelEndBarricade();
                    //     Debug.Log($"Setting barricade");
                    // }

                    // foreach (var level in levelsQueue)
                    // {
                    //     if (levelsDict.ContainsKey($"{level.transform.position}"))
                    //         levelsDict[$"{level.transform.position}"] = level;
                    //     else 
                    //         levelsDict.Add($"{level.transform.position}", level);
                    // }
                });
            });
        });
    }

    private void StartTween(out Tween tween, Vector3 additionalPos, float delay, bool useOffset = false)
    {
        tween = null;
        Debug.Log($"StartTween CurrentLevelNumber: {levelManager.CurrentLevelNumber}");
        levelObjectPointStartIndex = levelManager.CurrentLevelNumber == 1 ? 1 : 0;
        Debug.Log($"levelObjectPointStartIndex: {levelObjectPointStartIndex}");
        var indexer = levelObjectPointStartIndex;

        foreach (var levelObject in levelsQueue)
        {
            tween = levelObject.transform.DOMove(useOffset ? levelObject.transform.position + additionalPos : levelObjectPoints[indexer++].position + additionalPos, delay);
        }
    }
}
