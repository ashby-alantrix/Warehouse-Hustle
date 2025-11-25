using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour, IBootLoader,  IBase, IDataLoader
{
    private ScreenManager screenManager;
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
        screenManager = InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>();
        popupManager = InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>();

        inGameHUDScreen = screenManager.GetScreen<InGameHUDScreen>(ScreenType.InGameHUDScreen);
        levelCompletePopup = popupManager.GetPopup<LevelCompletePopup>(PopupType.LevelCompletePopup);
        gameOverPopup = popupManager.GetPopup<GameOverPopup>(PopupType.GameOverPopup);

        Debug.Log($"InGameUIManager: InGameHudScreen: {inGameHUDScreen}");
        Debug.Log($"InGameUIManager: levelCompletePopup: {levelCompletePopup}");

        inGameHUDScreen.Init();
    }
}
