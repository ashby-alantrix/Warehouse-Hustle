using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class UserDataBehaviour : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private StoreDataBase dataStorer;

    private UserData userData;

    public UserData GetUserData() => userData;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<UserDataBehaviour>(this);

        userData = new UserData();
        userData.levelConfigData = JsonConvert.DeserializeObject<LevelConfigData>(dataStorer.GetLevelsJson());
    }

    public LevelConfigData GetLevelsDatas()
    {
        return userData.levelConfigData;
    }

    public bool IsFirstUserSession()
    {
        return PlayerPrefs.GetInt(WarehouseHustle_Constants.IsFirstUserSession) == 1;
    }

    public void SetFirstUserSessionState(bool state)
    {
        PlayerPrefs.SetInt(WarehouseHustle_Constants.IsFirstUserSession, state ? 1 : 0);
    }
}
