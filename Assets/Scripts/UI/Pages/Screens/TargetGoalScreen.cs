using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetGoalScreen : ScreenBase
{
    [SerializeField] private TextMeshProUGUI targetGoalText;
    [SerializeField] private float screenShowDelay = 1.5f;

    private ScreenManager screenManager;

    public void SetTargetGoalText(string targetGoalText)
    {
        this.targetGoalText.text = targetGoalText;
        Invoke(nameof(CloseScreen), screenShowDelay);
    }

    private void CloseScreen()
    {
        screenManager = screenManager == null ? InterfaceManager.Instance.GetInterfaceInstance<ScreenManager>() : screenManager;
        screenManager.HideScreen(screenType);
    }
}
