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
    FreeRefillPopup,
    FeedbackPopup
}

public class PopupBase : UIBase, IUIBase
{
    [Header("Popup Scaling")]
    [SerializeField] protected Transform popupScaleContent;
    [SerializeField] protected bool shouldScale = true;
    [SerializeField] protected float scaleDelay = 0.5f;
    [SerializeField] protected PopupType popupType;

    private Action<PopupResultEvent> onComplete;
    public PopupType PopupType => popupType;

    protected PopupManager popupManager;

    public override void Show()
    {
        if (shouldScale)
        {
            popupScaleContent.localScale = UnityEngine.Vector3.zero;
            base.Show();

            popupScaleContent.DOScale(UnityEngine.Vector3.one, scaleDelay);
        }
        else 
            base.Show();
    }

    public override void Hide()
    {
        if (shouldScale)
            popupScaleContent.DOScale(UnityEngine.Vector3.zero, scaleDelay).OnComplete(() => base.Hide());
        else 
            base.Hide();
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
