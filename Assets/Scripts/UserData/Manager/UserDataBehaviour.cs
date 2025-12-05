using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class UserDataBehaviour : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private StoreDataBase dataStorer;

    private GameData gameData;
    private UserData userData;

    public GameData GetGameData() => gameData;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<UserDataBehaviour>(this);
    }

    public void InitializeData()
    {
        InitGameData();
        InitUserData();

        Debug.unityLogger.logEnabled = false;
    }

    private void InitGameData()
    {
        if (gameData == null)
        {
            gameData = new GameData();
            gameData = JsonConvert.DeserializeObject<GameData>(dataStorer.GetLevelsJson());
        }
    }

    private void InitUserData()
    {
        Debug.Log($"HasSavedUserData: {HasSavedUserData()}");
        if (HasSavedUserData())
        {
            userData = JsonConvert.DeserializeObject<UserData>(PlayerPrefs.GetString(WarehouseHustle_Constants.SaveUserData));
        }
        else
        {
            userData = new UserData();
            userData.userCurrencyData = new UserCurrencyData();
            userData.userHealthData = new UserHealthData();
            userData.timeData = new TimeData();

            userData.soundData = new InGameSoundData();
            userData.soundData.gameSoundToggle = true;

            userData.userLevelData = new UserLevelData();
            userData.userLevelData.lastUnlockedLevel = 1;
            userData.userLevelData.userLevelDataInfo = new UserLevelDataInfo[gameData.levelConfigData.levelDatas.Length];

            InitUserLevelDatas();
            SaveUserData();
        }
        
        SetFirstUserSessionState(!PlayerPrefs.HasKey(WarehouseHustle_Constants.IsFirstUserSession));
    }

    private void InitUserLevelDatas()
    {
        var length = gameData.levelConfigData.levelDatas.Length;
        for (int indexI = 0; indexI < length; indexI++)
        {
            userData.userLevelData.userLevelDataInfo[indexI] = new UserLevelDataInfo();
            userData.userLevelData.userLevelDataInfo[indexI].level = indexI + 1;
            userData.userLevelData.userLevelDataInfo[indexI].userLevelsInfo = new UserLevelInfo();
            userData.userLevelData.userLevelDataInfo[indexI].userLevelsInfo.unlocked = false;
        }
    }

    public GameCurrencyData GetGameCurrencyData()
    {
        return gameData.gameCurrency;
    }

    public HealthData GetHealthData()
    {
        return gameData.healthData;
    }

    public UserHealthData GetUserHealthData()
    {
        return userData.userHealthData;
    }

    public UserCurrencyData GetUserCurrencyData()
    {
        return userData.userCurrencyData;
    }

    public LevelConfigData GetLevelsDatas()
    {
        return gameData.levelConfigData;
    }

    public int GetLastUnlockedLevel()
    {
        Debug.Log($"userData: {userData}");
        Debug.Log($"userData.userLevelData: {userData.userLevelData}");
        Debug.Log($"userData.userLevelData.lastUnlockedLevel: {userData.userLevelData.lastUnlockedLevel}");

        return userData.userLevelData.lastUnlockedLevel;
    }

    public InGameSoundData GetSoundData()
    {
        return userData.soundData;
    }

    public void SaveLastUnlockedLevel(int level)
    {
        userData.userLevelData.lastUnlockedLevel = level;
        SaveUserData();
    }

    public void SaveUserCurrencyData(UserCurrencyData userCurrencyData)
    {
        userData.userCurrencyData = userCurrencyData;
        SaveUserData();
    }

    public void SaveSoundData(InGameSoundData soundData)
    {
        userData.soundData = soundData;
        SaveUserData();
    }

    #region PLAYER_PREFS_SAVING

    public bool IsFirstUserSession()
    {
        return PlayerPrefs.GetInt(WarehouseHustle_Constants.IsFirstUserSession) == WarehouseHustle_Constants.TRUE;
    }

    public void SetFirstUserSessionState(bool state)
    {
        PlayerPrefs.SetInt(WarehouseHustle_Constants.IsFirstUserSession, state ? WarehouseHustle_Constants.TRUE : WarehouseHustle_Constants.FALSE);
        PlayerPrefs.Save();
    }

    public TimeData GetTimeData()
    {
        return userData.timeData;
    }

    public void SetUserHealthData(UserHealthData userHealthData)
    {
        if (userData != null)
        {
            userData.userHealthData = userHealthData;
            SaveUserData();
        }
    }

    public void SetLastProgressTime(string timeString, string elapsedSeconds)
    {
        Debug.Log($"time :: userData.timeData.lastPlayedProgressTime: {timeString}");
        Debug.Log($"time :: userData.timeData.lastElapsedSeconds: {elapsedSeconds}");

        userData.timeData.lastPlayedProgressTime = timeString;
        userData.timeData.lastElapsedSeconds = elapsedSeconds;
        SaveUserData();
    }

    public bool HasSavedUserData()
    {
        return PlayerPrefs.HasKey(WarehouseHustle_Constants.SaveUserData);
    }

    public void SaveUserData()
    {
        PlayerPrefs.SetString(WarehouseHustle_Constants.SaveUserData, JsonConvert.SerializeObject(userData));
        PlayerPrefs.Save();
    }

    public void ClearUserData()
    {
        PlayerPrefs.SetString(WarehouseHustle_Constants.SaveUserData, "");
        PlayerPrefs.DeleteKey(WarehouseHustle_Constants.SaveUserData);
        PlayerPrefs.DeleteKey(WarehouseHustle_Constants.IsFirstUserSession);
        PlayerPrefs.Save();
    }

    #endregion
}
