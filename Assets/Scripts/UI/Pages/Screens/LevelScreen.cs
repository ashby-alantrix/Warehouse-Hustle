using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LevelScreen : ScreenBase
{
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

    private int GetLevelPageStartVal() => levelManager.CurrentLevelNumber == 1 || levelManager.CurrentLevelNumber == 2 ? 
                                            1 : (!levelManager.CanPlayLevel ? levelManager.CurrentLevelNumber - 2 : levelManager.CurrentLevelNumber - 1);

    public void InitLevelObjects()
    {
        int levelStartVal = GetLevelPageStartVal();
        for (int indexI = 0; indexI < levelObjects.Length; indexI++)
        {
            levelObjects[indexI].SetLevelText(levelStartVal++);
        }

        StartCoroutine(ShowLevelPageAnims());
    }

    private void InitLevelsQueue()
    {
        Debug.Log($"GetLevelStartVal(): {GetLevelPageStartVal()}");
        var addBarricade = levelManager.CurrentLevelNumber > levelManager.TotalLevelsCount - diff;
        var endIndex = addBarricade ? levelManager.TotalLevelsCount - GetLevelPageStartVal() + 1 : levelObjects.Length;

        Debug.Log($"EndIndex: {endIndex}");
        for (int indexI = 0; indexI < endIndex; indexI++)
        {
            levelsQueue.Enqueue(levelObjects[indexI]);
        }

        if (addBarricade)
        {
            Debug.Log($"### levelManager.CurrentLevelNumber: {levelManager.CurrentLevelNumber}, {levelManager.TotalLevelsCount - diff}");
            Level lvlObj = levelObjects[endIndex];
            lvlObj.SetLevelEndBarricade();
            lvlObj.gameObject.SetActive(true);
            levelsQueue.Enqueue(lvlObj);
        }
        
        Debug.Log($"levelsQueue count: {levelsQueue.Count}");
    }

    private IEnumerator ShowLevelPageAnims()
    {
        yield return new WaitUntil(() => levelManager != null && levelManager.HasInitializedLevelsData);
        
        InitLevelsQueue();

        for (int indexI = 0; indexI < levelObjects.Length; indexI++)
        {
            // if (levelObjects[indexI].HasBarricade) continue;

            levelObjects[indexI].ShowUnselectedLevelView();
        }

        if ((levelManager.CurrentLevelNumber != 1 && (levelManager.CanPlayLevel || levelManager.LevelState == LevelState.Lost)) || levelManager.CurrentLevelNumber > 2)
        {
            AlignLevelObjectPositions();
        }

        Debug.Log($"OnLevelComplete");

        CanTriggerScrolling = !levelManager.CanPlayLevel && levelManager.PrevLevelNumber != levelManager.CurrentLevelNumber 
            && levelManager.LevelState != LevelState.Lost && levelManager.CurrentLevelNumber != 1;

        UpdateLevelPageInfo();    
        StartCoroutine(StartLevelObjectAnims());
    }

    private bool CanTriggerScrolling = false;

    private void AlignLevelObjectPositions()
    {
        int levelObjPointIdx = 1;
        int levelObjectsLen = levelObjects.Length;
        if (!levelManager.CanPlayLevel && levelManager.CurrentLevelNumber >= levelManager.TotalLevelsCount)
        {
            levelObjectsLen -= 1;
        }
        else if (levelManager.CurrentLevelNumber > (levelManager.TotalLevelsCount - diff))
        {
            var decreaseCounter = levelManager.CurrentLevelNumber - (levelManager.TotalLevelsCount - diff);
            levelObjectsLen -= decreaseCounter;
        }

        for (int indexI = 0; indexI < levelObjectsLen; indexI++)
        {
            levelObjects[indexI].transform.position = levelObjectPoints[levelObjPointIdx].position;
            levelObjPointIdx++;
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

        Debug.Log($"LEVELNUMBER: PrevLevelNumber: {levelManager.PrevLevelNumber}");
        Debug.Log($"LEVELNUMBER: CurrentLevelNumber: {levelManager.CurrentLevelNumber}");
        if (CanTriggerScrolling)
            TriggerScrollingAnim();
        else
        {
            foreach (var level in levelsQueue)
                level.InitListenersLevelObject();
        }
    }

    private void UpdateLevelPageInfo()
    {
        if (levelManager.CurrentLevelNumber == 1)
        {
            var testList = levelsQueue.ToList();
            testList[0].ShowSelectedLevelView();
            testList[0].TogglePlayBtnState(true);
        }
        else if (levelManager.CurrentLevelNumber == 2)
        {
            var testList = levelsQueue.ToList();
            newUnlockedLvl = testList[1]; // current centered object
            newUnlockedLvl.ShowSelectedLevelView();
            EnablePlayButton();

            newUnlockedLvl.SetLevelText(levelManager.CurrentLevelNumber);
        }
        else if (levelManager.CurrentLevelNumber <= levelManager.TotalLevelsCount)
        {
            Debug.Log($"levelQueue first: {levelsQueue.First().LevelNum}");
            var testList = levelsQueue.ToList();
            var middleIndex = !levelManager.CanPlayLevel ? 2 : 1; // TODO :: use len/2 if possible
            newUnlockedLvl = testList[middleIndex]; // current centered object
            newUnlockedLvl.ShowSelectedLevelView();
            EnablePlayButton();
            
            newUnlockedLvl.SetLevelText(levelManager.CurrentLevelNumber);

            Debug.Log($"currentFinishedLvl: {currentFinishedLvl.LevelNum}");

            //newUnlockedLvl = testList[2]; // next object/level to show
            //newUnlockedLvl.ShowSelectedLevelView();
            Debug.Log($"newUnlockedLvl: {newUnlockedLvl.LevelNum}");
        }
        else
        {
            newUnlockedLvl = levelsQueue.Last();
            Debug.Log($"test1 :: newUnlockedLvl.HasBarricade: {newUnlockedLvl.HasBarricade}, && CanTriggerScrolling: {CanTriggerScrolling}");
            newUnlockedLvl.ShowSelectedLevelView();
            if (newUnlockedLvl.HasBarricade && !CanTriggerScrolling)
            {
                newUnlockedLvl.ShowRestartButton();
            }
        }
    }

    public void EnablePlayButton()
    {
        if (levelManager.CanPlayLevel || levelManager.LevelState == LevelState.Lost)
            newUnlockedLvl.TogglePlayBtnState(true);
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

                    if (levelManager.CurrentLevelNumber > 2 && levelManager.CurrentLevelNumber <= levelManager.TotalLevelsCount)
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

                    if (!newUnlockedLvl.HasBarricade)
                    {
                        newUnlockedLvl.TogglePlayBtnState(true);
                        newUnlockedLvl.ScaleLevelButton();
                    }
                    else 
                    {
                        newUnlockedLvl.ShowRestartButton();
                    }

                    foreach (var level in levelsQueue)
                        level.InitListenersLevelObject();
                });
            });
        });
    }

    private void StartTween(out Tween tween, Vector3 additionalPos, float delay, bool useOffset = false)
    {
        tween = null;
        Debug.Log($"StartTween CurrentLevelNumber: {levelManager.CurrentLevelNumber}");

        levelObjectPointStartIndex = levelManager.CurrentLevelNumber <= 2 ? 1 : 0;

        Debug.Log($"levelObjectPointStartIndex: {levelObjectPointStartIndex}");
        var indexer = levelObjectPointStartIndex;
        Debug.Log($"Indexer: {indexer}");

        foreach (var levelObject in levelsQueue)
        {
            tween = levelObject.transform.DOMove(useOffset ? levelObject.transform.position + additionalPos : levelObjectPoints[indexer++].position + additionalPos, delay);
        }
    }
}
