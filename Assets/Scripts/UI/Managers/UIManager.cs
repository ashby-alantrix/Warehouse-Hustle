using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour, IBootLoader, IBase
{
    private PopupManager popupManager;

    private LevelManager levelManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<UIManager>(this);
        popupManager = InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>();
    }

    public void UpdateLoadedGoods(int loadedGoods)
    {
        // inGameHUDScreen = inGameHUDScreen == null ? (InGameHUDScreen)popupManager.GetScreen<InGameHUDScreen>(UIType.InGameHUDScreen) : inGameHUDScreen;
        InGameHUDScreen inGameHUDScreen = popupManager.GetScreen<InGameHUDScreen>(UIType.InGameHUDScreen);
        inGameHUDScreen.SetGoodsGoalText(loadedGoods);
    }

    public void OnLevelLost()
    {
        
    }

    public void OnLevelWon(int coinsReward)
    {
        popupManager.ShowScreen(UIType.LevelCompletePopup);
        LevelCompletePopup levelCompletePopup = popupManager.GetScreen<LevelCompletePopup>(UIType.LevelCompletePopup);   
        Debug.Log($"LevelCompletePopup: {levelCompletePopup}");
        levelCompletePopup.SetCoinsReward(coinsReward);
    }
}
