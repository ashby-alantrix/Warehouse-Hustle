using UnityEngine;

public enum PopupType
{
    FTUE_Popup,
    MenuSettingsPopup,
    RestartPopup,
    LevelCompletePopup,
    GameOverPopup,
    LevelFailPopup,
    GetMoreLivesPopup,
}

public class PopupBase : UIBase, IUIBase
{
    [SerializeField] protected PopupType popupType;

    public PopupType PopupType => popupType;

    private PopupManager popupManager;

    public void Initialize()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        popupManager.RegisterPopup(this);
    }
}
