using System.Numerics;
using System;
using DG.Tweening;
using UnityEngine;

public enum PopupType
{
    FTUE_Popup, //
    SettingsPopup, 
    RestartPopup, //
    LevelCompletePopup, 
    GameOverPopup, 
    LevelFailPopup,
    GetMoreLivesPopup, //
    FreeRefillPopup
}

public class PopupBase : UIBase, IUIBase
{
    [Header("Popup Scaling")]
    [SerializeField] protected Transform popupScaleContent;
    [SerializeField] protected float scaleDelay = 0.5f;
    [SerializeField] protected PopupType popupType;

    private Action<PopupResultEvent> onComplete;
    public PopupType PopupType => popupType;

    protected PopupManager popupManager;

    public override void Show()
    {
        popupScaleContent.localScale = UnityEngine.Vector3.zero;
        base.Show();

        popupScaleContent.DOScale(UnityEngine.Vector3.one, scaleDelay);
    }

    public override void Hide()
    {
        popupScaleContent.DOScale(UnityEngine.Vector3.zero, scaleDelay).OnComplete(() => base.Hide());
    }

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
