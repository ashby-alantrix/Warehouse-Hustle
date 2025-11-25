using System;
using System.Collections.Generic;
using UnityEngine;

// public class BaseUIManager<T> : MonoBehaviour, IBase, IBootLoader where T : UIBase
// {
//     public void Initialize()
//     {
//         throw new NotImplementedException();
//     }
// }

public class PopupManager : MonoBehaviour, IBase, IBootLoader
{
    private Dictionary<PopupType, PopupBase> popupsDict = new Dictionary<PopupType, PopupBase>();

    private PopupBase activePopup = null;
    public PopupBase GetActiveScreen() => activePopup;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<PopupManager>(this);
        popupsDict.Clear();
    }

    public void RegisterPopup(PopupBase popupBase)
    {
        if (!popupsDict.ContainsKey(popupBase.PopupType))
            popupsDict.Add(popupBase.PopupType, popupBase);
        else
            popupsDict[popupBase.PopupType] = popupBase;
    }

    public T GetPopup<T>(PopupType uiType) where T : PopupBase
    {
        return popupsDict.ContainsKey(uiType) ? (T)popupsDict[uiType] : null;
    }

    public void ShowPopup(PopupType uiType)
    {
        activePopup = popupsDict[uiType];
        if (activePopup != null)
        {
            activePopup.Show();
        }
    }

    public void HideActivePopup()
    {
        if (activePopup != null)
            activePopup.Hide();
    }

    public void HidePopup(PopupType popupType)
    {
        if (popupsDict[popupType] != null)
        {
            if (activePopup.PopupType == popupType) activePopup = null;
            
            popupsDict[popupType].Hide();
        }
    }

}
