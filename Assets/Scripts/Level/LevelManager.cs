using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LevelState
{
    Progress, 
    Won,
    Lost
}

public class LevelManager : MonoBehaviour, IBootLoader, IBase, IDataLoader
{

    private int currentLevelNumber = 1;
    private LevelState levelState = LevelState.Progress;

    private LevelScreen levelPage;
    private UIManager uiManager;
    private UserDataBehaviour userDataBehaviour;
    private PopupManager popupManager;

    private LevelConfigData levelConfigData;
    private Dictionary<int, LevelsInfo> levelDataDictionary = new Dictionary<int, LevelsInfo>();

    public int TotalLevelsCount => levelDataDictionary.Count;
    public int CurrentLevelNumber => currentLevelNumber;
    public LevelState LevelState => levelState;
    public bool HasInitializedLevelsData = false;
    public bool CanPlayLevel = true; // TODO :: Change the name of the variable according to logic

    public void OnLevelStateChange(LevelState state)
    {
        SetUIManager();
        levelState = state;
        switch (levelState)
        {
            case LevelState.Progress:
                CanPlayLevel = true;
            break;
            case LevelState.Won:
                CanPlayLevel = false;
                uiManager.OnLevelWon(GetCurrentLevelsInfo().coinsRewardToGive);
            break;
            case LevelState.Lost:
                CanPlayLevel = false;
                uiManager.OnLevelLost();
            break;
            default: break;
        }
    }

    private void SetUIManager()
    {
        uiManager = uiManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<UIManager>() : uiManager;
    }

    public void SetCurrentLevelNumber(int currentLevelNumber)
    {
        this.currentLevelNumber = currentLevelNumber;
    }

    public LevelsInfo GetCurrentLevelsInfo()
    {
        return levelDataDictionary.ContainsKey(currentLevelNumber) ? levelDataDictionary[currentLevelNumber] : null;
    }

    public LevelsInfo GetLevelsInfo(int levelNum)
    {
        return levelDataDictionary.ContainsKey(levelNum) ? levelDataDictionary[levelNum] : null;
    }

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<LevelManager>(this);

        userDataBehaviour = InterfaceManager.Instance?.GetInterfaceInstance<UserDataBehaviour>();
        popupManager = InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>();
    }

    public void InitializeData()
    {
        levelConfigData = userDataBehaviour.GetLevelsDatas();
        InitLevelsInfoToDict();

        levelPage = popupManager.GetScreen<LevelScreen>(UIType.LevelsScreen);
        levelPage.InitLevelManager(this);
        levelPage.InitLevelObjects();
        HasInitializedLevelsData = true;
    }

    public void InitLevelsInfoToDict()
    {
        foreach (var levelData in levelConfigData.levelDatas)
        {
            if (!levelDataDictionary.ContainsKey(levelData.level))
            {
                levelDataDictionary.Add(levelData.level, levelData.levelsInfo);
            }
        }
    }

    public void LoadLevelInGame()
    {
        MainSingleton.Instance.LoadGameplayScene();
    }
}
