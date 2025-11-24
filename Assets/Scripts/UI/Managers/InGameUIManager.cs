using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour, IBootLoader,  IBase, IDataLoader
{
    private PopupManager popupManager;
    private InGameHUDScreen inGameHUDScreen;
    private GameOverPopup gameOverPopup;
    private LevelCompletePopup levelCompletePopup;

    public InGameHUDScreen InGameHUDScreen => inGameHUDScreen;
    public GameOverPopup GameOverPopup => gameOverPopup;
    public LevelCompletePopup LevelCompletePopup => levelCompletePopup;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<InGameUIManager>(this);
    }

    public void InitializeData()
    {
        popupManager = InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>();
        inGameHUDScreen = popupManager.GetScreen<InGameHUDScreen>(UIType.InGameHUDScreen);
        levelCompletePopup = popupManager.GetScreen<LevelCompletePopup>(UIType.LevelCompletePopup);
        gameOverPopup = popupManager.GetScreen<GameOverPopup>(UIType.GameOverPopup);

        Debug.Log($"InGameUIManager: InGameHudScreen: {inGameHUDScreen}");
        Debug.Log($"InGameUIManager: levelCompletePopup: {levelCompletePopup}");

        inGameHUDScreen.Init();
    }
}
