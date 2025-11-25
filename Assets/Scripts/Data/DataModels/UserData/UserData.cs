
/************************* GAME DATA *************************/
[System.Serializable]
public class GameData
{
    public GameCurrencyData gameCurrency;
    public HealthData healthData;
    public LevelConfigData levelConfigData;
}

[System.Serializable]
public class GameCurrencyData
{
    public int initialCurrencyToProvide;
}

[System.Serializable]
public class LevelConfigData
{
    public LevelDatas[] levelDatas;
}

[System.Serializable]
public class LevelDatas
{
    public int level;
    public LevelsInfo levelsInfo;
}

[System.Serializable]
public class HealthData
{
    public int totalLifes;
    public int singleHealthCurrencyValue;
    public float timeInSecondsForOneLife;
}

[System.Serializable]
public class LevelsInfo
{
    public int availGoodTypes;
    public int targetGoodsToLoad;
    public int currencyToGive;
}

/************************* USER DATA *************************/

[System.Serializable]
public class UserData
{
    public UserCurrencyData userCurrencyData;
    public UserLevelData userLevelData;
    public UserHealthData userHealthData;
    public TimeData timeData;
}

[System.Serializable]
public class TimeData
{
    public string lastPlayedProgressTime;
    public string lastElapsedSeconds;
    
}

[System.Serializable]
public class UserCurrencyData
{
    public int attainedCurrency;
}

[System.Serializable]
public class UserLevelData
{
    public int lastUnlockedLevel = 1;
    public UserLevelDataInfo[] userLevelDataInfo;
}

[System.Serializable]
public class UserHealthData
{
    public int attainedLifes;
}

[System.Serializable]
public class UserLevelDataInfo
{
    public int level;
    public UserLevelInfo userLevelsInfo;
}

[System.Serializable]
public class UserLevelInfo
{
    public bool unlocked;
}