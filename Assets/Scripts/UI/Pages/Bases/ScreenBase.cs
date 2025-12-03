using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public enum ScreenType
{
    LoadingScreen,
    LevelsScreen,
    InGameHUDScreen,
    MenuHUDScreen,
    GlobalHUDScreen,
    TargetGoalScreen
}

public class ScreenBase : UIBase, IUIBase
{
    [SerializeField] protected ScreenType screenType;
    [SerializeField] protected bool shouldFade = false;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected float fadeDuration;

    public ScreenType ScreenType => screenType;

    private ScreenManager screenManager;

    public void Initialize()
    {
        screenManager = screenManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>() : screenManager;
        screenManager.RegisterScreen(this);
    }

    public override void Show()
    {
        if (shouldFade)
        {
            canvasGroup.alpha = 0;
            base.Show();
            canvasGroup.DOFade(1, fadeDuration);
        }
        else 
            base.Show();
    }

    public override void Hide()
    {
        if (shouldFade)
        {
            canvasGroup.DOFade(0, fadeDuration).OnComplete(() => base.Hide());
        }
        else 
            base.Hide();
    }
}
