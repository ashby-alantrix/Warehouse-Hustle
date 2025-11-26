using System;
using UnityEngine;

public enum PopupType
{
    FTUE_Popup,
    SettingsPopup,
    RestartPopup,
    LevelCompletePopup,
    GameOverPopup,
    LevelFailPopup,
    GetMoreLivesPopup,
    FreeRefillPopup
}

public class PopupBase : UIBase, IUIBase
{
    [SerializeField] protected PopupType popupType;

    private Action<PopupResultEvent> onComplete;
    public PopupType PopupType => popupType;

    protected PopupManager popupManager;

    public void Initialize()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        popupManager.RegisterPopup(this);
    }

    public void InitNextActionEvent(Action<PopupResultEvent> onComplete)
    {
        this.onComplete = onComplete;
    }

    protected void OnComplete(PopupResultEvent popupResultEvent)
    {
        onComplete?.Invoke(popupResultEvent);
    }

    void OnDestroy()
    {
        onComplete = null;
    }
}
