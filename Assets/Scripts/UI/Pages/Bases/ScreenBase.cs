using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScreenType
{
    LoadingScreen,
    LevelsScreen,
    InGameHUDScreen,
    MenuHUDScreen,
    GlobalHUDScreen,
}

public class ScreenBase : UIBase, IUIBase
{
    [SerializeField] protected ScreenType screenType;

    public ScreenType ScreenType => screenType;

    private ScreenManager screenManager;

    public void Initialize()
    {
        screenManager = screenManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>() : screenManager;
        screenManager.RegisterScreen(this);
    }
}
