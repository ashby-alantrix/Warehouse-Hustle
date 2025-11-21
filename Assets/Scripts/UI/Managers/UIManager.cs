using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class UIManager : MonoBehaviour, IBootLoader, IBase
{
    private PopupManager popupManager;
    private InGameHUDScreen inGameHUDScreen;

    private LevelManager levelManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<UIManager>(this);
    }

    public void UpdateLoadedGoods(int loadedGoods)
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        inGameHUDScreen = inGameHUDScreen == null ? (InGameHUDScreen)popupManager.GetScreen<InGameHUDScreen>(UIType.InGameHUDScreen) : inGameHUDScreen;
        inGameHUDScreen.SetGoodsGoalText(loadedGoods);
    }

    public void OnLevelLost()
    {
        throw new NotImplementedException();
    }

    public void OnLevelWon()
    {
        
    }
}
