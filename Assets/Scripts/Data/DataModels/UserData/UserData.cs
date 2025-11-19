
using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    public LevelConfigData levelConfigData;
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
public class LevelsInfo
{
    public int goodType;
    public int goodsToLoad;
    public bool unlocked;
}
