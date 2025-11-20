using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour, IBootLoader, IBase
{
    [SerializeField] private LevelScreen levelPage;

    private int currentLevelNumber = 1;

    private UserDataBehaviour userDataBehaviour;
    private LevelConfigData levelConfigData;
    private Dictionary<int, LevelsInfo> levelDataDictionary = new Dictionary<int, LevelsInfo>();

    public int TotalLevelsCount => levelDataDictionary.Count;
    public int CurrentLevelNumber => currentLevelNumber;

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
        levelConfigData = userDataBehaviour.GetLevelsDatas();

        InitLevelsInfoToDict();
        levelPage.InitLevelManager(this);
        levelPage.InitLevelObjects();
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

    public void OnLevelCompleted()
    {
        levelPage.OnLevelComplete();
    }

    public void LoadLevelInGame()
    {
        MainSingleton.Instance.LoadGameplayScene();
    }
}
