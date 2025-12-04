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
    GlobalHUDScreen
}

public class ScreenBase : UIBase, IUIBase
{
    [SerializeField] protected ScreenType screenType;
    [SerializeField] protected bool shouldFade = false;
    

    public ScreenType ScreenType => screenType;

    private ScreenManager screenManager;

    public void Initialize()
    {
        screenManager = screenManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>() : screenManager;
        screenManager.RegisterScreen(this);
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
