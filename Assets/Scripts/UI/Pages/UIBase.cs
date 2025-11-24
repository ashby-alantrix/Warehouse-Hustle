using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    LoadingScreen,
    LevelsScreen,
    InGameHUDScreen,
    FTUE_Popup,
    MenuSettingsPopup,
    RestartPopup,
    LevelCompletePopup,
    GameOverPopup,
    LevelFailPopup,
    GetMoreLivesPopup,
}

public class UIBase : MonoBehaviour, IUIBase
{
    [SerializeField] protected UIType uiType;

    public UIType UIType => uiType;

    private PopupManager popupManager;

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        popupManager.RegisterScreen(this);
    }
}
