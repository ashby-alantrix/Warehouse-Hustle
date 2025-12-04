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
    FeedbackPopup,
    TargetGoalPopup
}

public enum PopupScalerType
{
    None,
    Zoom,
    Fade
}

public class PopupBase : UIBase, IUIBase
{
    [Header("Popup Scaling")]
    [SerializeField] protected Transform popupScaleContent;
    // [SerializeField] protected bool shouldScale = true;
    [SerializeField] protected float zoomDuration = 0.5f;
    [SerializeField] protected PopupType popupType;
    [SerializeField] protected PopupScalerType popupScalerType = PopupScalerType.Zoom;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected float fadeDuration;

    private Action<PopupResultEvent> onComplete;
    public PopupType PopupType => popupType;

    protected PopupManager popupManager;

    public override void Show()
    {
        ApplyEffectOnShow();
    }

    public override void Hide()
    {
        ApplyEffectOnHide();
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

    private void ApplyEffectOnShow()
    {
        switch (popupScalerType)
        {
            case PopupScalerType.None:
                base.Show();
            break;
            case PopupScalerType.Zoom:
                popupScaleContent.localScale = UnityEngine.Vector3.zero;
                base.Show();
                popupScaleContent.DOScale(UnityEngine.Vector3.one, zoomDuration);
            break;
            case PopupScalerType.Fade:
                canvasGroup.alpha = 0;
                base.Show();
                canvasGroup.DOFade(1, fadeDuration);
            break;
        }
    }

    private void ApplyEffectOnHide()
    {
        switch (popupScalerType)
        {
            case PopupScalerType.None:
                base.Hide();
            break;
            case PopupScalerType.Zoom:
                popupScaleContent.DOScale(UnityEngine.Vector3.zero, zoomDuration).OnComplete(() => base.Hide());
            break;
            case PopupScalerType.Fade:
                canvasGroup.DOFade(0, fadeDuration).OnComplete(() => base.Hide());
            break;
        }
    }

    private void OnDestroy()
    {
        onComplete = null;
    }
}
