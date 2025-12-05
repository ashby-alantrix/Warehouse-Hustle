using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetGoalPopup : PopupBase
{
    [SerializeField] private TextMeshProUGUI targetGoalText;
    [SerializeField] private float showDelay = 1.5f;

    public void SetTargetGoalText(string targetGoalText)
    {
        this.targetGoalText.text = targetGoalText;
        Invoke(nameof(CloseScreen), showDelay);
    }

    private void CloseScreen()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance.GetInterfaceInstance<PopupManager>() : popupManager;
        popupManager.HidePopup(popupType);
    }
}
