using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour, IBase, IBootLoader
{
    private Dictionary<ScreenType, ScreenBase> screensDict = new Dictionary<ScreenType, ScreenBase>();

    private ScreenBase activeScreen = null;
    public ScreenBase GetActiveScreen() => activeScreen;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<ScreenManager>(this);
        screensDict.Clear();
    }

    public void RegisterScreen(ScreenBase screenBase)
    {
        if (!screensDict.ContainsKey(screenBase.ScreenType))
            screensDict.Add(screenBase.ScreenType, screenBase);
        else
            screensDict[screenBase.ScreenType] = screenBase;
    }

    public T GetScreen<T>(ScreenType screenType) where T : ScreenBase
    {
        return screensDict.ContainsKey(screenType) ? (T)screensDict[screenType] : null;
    }

    public void ShowScreen(ScreenType screenType)
    {
        activeScreen = screensDict[screenType];
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

    public void HideScreen(ScreenType screenType)
    {
        if (screensDict[screenType] != null)
        {
            if (activeScreen.ScreenType == screenType) activeScreen = null;
            
            screensDict[screenType].Hide();
        }
    }

}
