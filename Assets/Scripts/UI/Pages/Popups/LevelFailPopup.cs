using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelFailPopup : PopupBase
{
    [SerializeField] private Button tryAgainBtn;
    [SerializeField] private Button onCloseBtn;

    private HealthSystem healthSystem;
    private LevelManager levelManager;

    private void OnEnable()
    {
        tryAgainBtn.onClick.AddListener(() => OnClick_TryAgain());
        onCloseBtn.onClick.AddListener(() => OnClick_CloseBtn());
    }

    private void OnDisable()
    {
        tryAgainBtn.onClick.RemoveAllListeners();
        onCloseBtn.onClick.RemoveAllListeners();
    }

    public void OnClick_TryAgain()
    {
        healthSystem = healthSystem == null ? InterfaceManager.Instance?.GetInterfaceInstance<HealthSystem>() : healthSystem;
        SetLevelManager();
        
        Debug.Log($"healthSystem.HasEnoughHealth: {healthSystem.HaveHealthLeft}, healthSystem.AvailableLifes: {healthSystem.AvailableLifes}");
        Debug.Log($"healthSystem.HasEnoughHealth: {healthSystem.UserHealthData.haveUsedFreeRefill}");

        if (healthSystem.HaveHealthLeft)
        {
            popupManager.HidePopup(popupType);
            levelManager.ExecuteRestartLevelActions();
        }
        else if (!healthSystem.UserHealthData.haveUsedFreeRefill)
        {
            OnComplete(PopupResultEvent.OnFreeRefillHealth);
        }
        else
        {
            OnComplete(PopupResultEvent.FreeRefillUsed);
        }
        // else 
    }

    public void OnClick_CloseBtn()
    {
        InterfaceManager.Instance?.GetInterfaceInstance<HealthSystem>().SetLastProgressTime();
        MainSingleton.Instance.LoadMenuLoadingScene();
    }

    public void SetLevelManager()
    {
        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
    }
}
