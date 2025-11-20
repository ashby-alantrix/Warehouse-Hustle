using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour, IBase, IBootLoader
{
    public List<UIBase> bases = new List<UIBase>();

    private Dictionary<UIType, UIBase> screensDict = new Dictionary<UIType, UIBase>();

    private UIBase activeScreen = null;
    public UIBase GetActiveScreen() => activeScreen;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<PopupManager>(this);
    }

    public void RegisterScreen(UIBase uiBase)
    {
        Debug.Log($"Register screen: {uiBase}, {uiBase.UIType}");

        if (!screensDict.ContainsKey(uiBase.UIType))
            screensDict.Add(uiBase.UIType, uiBase);
        else
            screensDict[uiBase.UIType] = uiBase;

        foreach (var screen in screensDict)
        {
            Debug.Log($"Screendict, key: {screen.Key}, value: {screen.Value}");
        }
    }

    public UIBase GetScreen<T>(UIType uiType) where T : UIBase
    {
        return screensDict.ContainsKey(uiType) ? (T)screensDict[uiType] : null;
    }

    public void ShowScreen(UIType uiType)
    {
        activeScreen = screensDict[uiType];
        if (activeScreen != null)
        {
            activeScreen.Show();
        }
    }

    public void HideActiveScreen()
    {
        if (activeScreen != null)
            activeScreen.Hide();
    }

    public void HideScreen(UIType uiType)
    {
        if (screensDict[uiType] != null)
        {
            screensDict[uiType].Hide();
        }
    }

    private void OnDestroy()
    {
        screensDict.Clear();
    }
}
