using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public enum LevelState
{
    Progress, 
    Won,
    Lost,
    Paused
}

public class LevelManager : MonoBehaviour, IBootLoader, IBase, IDataLoader
{
    private int prevLevelNumber = 1, currentLevelNumber = 1;

    private LevelScreen levelPage;
    private InGameUIManager inGameUIManager;
    private UserDataBehaviour userDataBehaviour;
    private InputManager inputManager;
    private TrucksLoaderManager trucksLoaderManager;
    private SoundManager soundManager;
    private PopupManager popupManager;
    private ScreenManager screenManager;
    private GoodsManager goodsManager;
    private NodeManager nodeManager;
    private HealthSystem healthSystem;

    private LevelConfigData levelConfigData;
    private Dictionary<int, LevelsInfo> levelDataDictionary = new Dictionary<int, LevelsInfo>();

    public int TotalLevelsCount => levelDataDictionary.Count;
    public int PrevLevelNumber => prevLevelNumber;
    public int CurrentLevelNumber => currentLevelNumber;
    public LevelState LevelState 
    {
        get;
        private set;
    }

    public bool HasInitializedLevelsData = false;
    public bool CanPlayLevel = true; // TODO :: Change the name of the variable according to logic

    public void SetLevelState(LevelState levelState)
    {
        LevelState = levelState;
    }

    public void ExecuteRestartLevelActions()
    {
        goodsManager = goodsManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsManager>() : goodsManager;
        goodsManager.ClearAllGoodsInNodes();
        goodsManager.ClearGoodsInputPlatforms();
        
        SetTrucksLoaderManager();
        trucksLoaderManager.ResetLoadedGoods();

        goodsManager.InitGoodsInfos();
        OnLevelStateChange(LevelState.Progress);
        SetInGameUIManager();
        inGameUIManager.ShowTargetGoalScreen();
    }

    private void SetInGameUIManager()
    {
        inGameUIManager = inGameUIManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<InGameUIManager>() : inGameUIManager;
    }

    public void OnLevelStateChange(LevelState state)
    {
        LevelState = state;
        SetInGameUIManager();
        inputManager = inputManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputManager>() : inputManager;
        nodeManager = nodeManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<NodeManager>() : nodeManager;

        SetTrucksLoaderManager();

        switch (LevelState)
        {
            case LevelState.Progress:
                CanPlayLevel = true;
            break;
            case LevelState.Won:
                CanPlayLevel = false;
                var levelsInfo = GetCurrentLevelsInfo();
                inGameUIManager?.LevelCompletePopup?.SetCoinsReward(levelsInfo.currencyToGive);
                soundManager.PlayPrimaryGameSoundClip(SoundType.Level_Win);

                prevLevelNumber = currentLevelNumber;
                currentLevelNumber++;
                userDataBehaviour.SaveLastUnlockedLevel(currentLevelNumber);
                popupManager.ShowPopup(PopupType.LevelCompletePopup);
            break;
            case LevelState.Lost:
                CanPlayLevel = false;
                soundManager.PlayPrimaryGameSoundClip(SoundType.Level_Lost);
                Debug.Log($"Stack check :: ");
                popupManager.ShowPopup(PopupType.GameOverPopup);
                inGameUIManager.GameOverPopup.InitData(GetCurrentLevelsInfo().targetGoodsToLoad - trucksLoaderManager.GetLoadedGoods());
            break;
            default: break;
        }
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
        screenManager = InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>();

        Debug.Log($"Initializing user data behaviour: {userDataBehaviour}");
    }

    public void InitializeData()
    {
        Debug.Log($"Initialized user data behaviour: {userDataBehaviour}");
        Debug.Log($"HasInitializedLevelsData: {HasInitializedLevelsData}");
        currentLevelNumber = userDataBehaviour.GetLastUnlockedLevel();
        if (!HasInitializedLevelsData) // is first time or new session
        {
            prevLevelNumber = currentLevelNumber;
        }

        Debug.Log($"### currentLevelNumber: {currentLevelNumber}");
        levelConfigData = userDataBehaviour.GetLevelsDatas();
        InitLevelsInfoToDict();

        soundManager = InterfaceManager.Instance?.GetInterfaceInstance<SoundManager>();
        levelPage = screenManager.GetScreen<LevelScreen>(ScreenType.LevelsScreen);      
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

        Debug.Log($"LevelsDataDict: {levelDataDictionary.Count}");
    }

    public void LoadLevelInGame()
    {
        healthSystem = healthSystem == null ? InterfaceManager.Instance?.GetInterfaceInstance<HealthSystem>() : healthSystem;
        if (healthSystem.HaveHealthLeft)
        {
            MainSingleton.Instance.LoadGameplayScene();
            OnLevelStateChange(LevelState.Progress);
        }
        else
        {
            popupManager.ShowPopup(PopupType.GetMoreLivesPopup);
        }
    }

    private void SetTrucksLoaderManager()
    {
        trucksLoaderManager = trucksLoaderManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<TrucksLoaderManager>() : trucksLoaderManager;
    }

    public void OnRestartWholeLevels()
    {
        userDataBehaviour.ClearUserData();
        MainSingleton.Instance.LoadMenuLoadingScene();
    }
}
