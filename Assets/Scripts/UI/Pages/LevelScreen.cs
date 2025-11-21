using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LevelScreen : UIBase
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
    private Dictionary<string, Level> levelsDict = new Dictionary<string, Level>();

    void Awake()
    {
        foreach (var levelObject in levelObjects)
            levelsQueue.Enqueue(levelObject);

        var posVal = Vector3.zero;
        foreach (var level in levelsQueue)
        {
            posVal = level.transform.position + finalAdditionalSlideOffset;
            if (!levelsDict.ContainsKey($"{posVal}"))
                levelsDict[$"{posVal}"] = level;
            else 
                levelsDict.Add($"{posVal}", level);
        }

    }

    void OnEnable()
    {
        StartCoroutine(ShowLevelPageAnims());

        StartCoroutine(StartLevelObjectAnims());
    }

    private IEnumerator ShowLevelPageAnims()
    {
        yield return new WaitUntil(() => levelManager != null && levelManager.HasInitializedLevelsData);
        
        foreach (var lvlPair in levelsDict)
        {
            Debug.Log($"lvlPair.LevelNum: {lvlPair.Value.LevelNum}");
            if (lvlPair.Value.LevelNum == levelManager.CurrentLevelNumber)
            {
                lvlPair.Value.ShowSelectedLevelView();
                lvlPair.Value.SetLevelText(lvlPair.Value.LevelNum);
            }
            else 
            {    
                lvlPair.Value.ShowUnselectedLevelView();
                lvlPair.Value.SetLevelText(lvlPair.Value.LevelNum);
            }
        }

        if (!levelManager.CanPlayLevel)
        {
            OnLevelComplete();    
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

    private void OnLevelComplete()
    {
        if (levelManager.CurrentLevelNumber == 1)
        {
            currentFinishedLvl.ShowUnselectedLevelView();
            newUnlockedLvl.ShowSelectedLevelView();
        }
        else if (levelManager.CurrentLevelNumber <= levelManager.TotalLevelsCount)
        {
            string lvlPos = $"{currentFinishedLvlTransform.position + finalAdditionalSlideOffset}";
            if (levelsDict.ContainsKey(lvlPos))
            {
                currentFinishedLvl = levelsDict[lvlPos];
                currentFinishedLvl.ShowUnselectedLevelView();
            }

            lvlPos = $"{newUnlockedLvlTransform.position + finalAdditionalSlideOffset}";
            if (levelsDict.ContainsKey(lvlPos))
            {
                newUnlockedLvl = levelsDict[lvlPos];
                newUnlockedLvl.ShowSelectedLevelView();
            }
        }
        
        newUnlockedLvl.SetLevelText(levelManager.CurrentLevelNumber + 1);
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

                    if (levelManager.CurrentLevelNumber < levelManager.TotalLevelsCount) 
                        levelManager.SetCurrentLevelNumber(levelManager.CurrentLevelNumber + 1); 

                    if (!newUnlockedLvl.HasBarricade)
                        newUnlockedLvl.TogglePlayBtnState(true);
                    else 
                        newUnlockedLvl.ShowRestartButton();
                        
                    if (newUnlockedLvl.LevelNum == levelManager.TotalLevelsCount - diff)
                    {
                        levelsQueue.Enqueue(dequeuedLevel);
                        dequeuedLevel.gameObject.SetActive(true);
                        dequeuedLevel.SetLevelEndBarricade();
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
        Debug.Log($"levelObjectPointStartIndex: {levelObjectPointStartIndex}");
        var indexer = levelObjectPointStartIndex;

        foreach (var levelObject in levelsQueue)
        {
            tween = levelObject.transform.DOMove(useOffset ? levelObject.transform.position + additionalPos : levelObjectPoints[indexer++].position + additionalPos, delay);
        }
    }
}
