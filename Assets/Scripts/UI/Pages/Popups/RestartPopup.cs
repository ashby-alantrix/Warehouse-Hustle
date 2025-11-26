using UnityEngine;
using UnityEngine.UI;

public class RestartPopup : PopupBase
{
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button closeBtn;

    private HealthSystem healthSystem;
    private LevelManager levelManager;

    public void OnClick_RestartBtn()
    {
        healthSystem = healthSystem == null ? InterfaceManager.Instance.GetInterfaceInstance<HealthSystem>() : healthSystem;
        levelManager = levelManager == null ? InterfaceManager.Instance.GetInterfaceInstance<LevelManager>() : levelManager;


        if (healthSystem.AvailableLifes > 1)
        {
            popupManager.HidePopup(popupType);
            healthSystem.RemoveHealth(1);
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
    }

    public void OnClick_CloseBtn()
    {
        popupManager.HidePopup(popupType);
    }

    private void OnEnable()
    {
        restartBtn.onClick.AddListener(OnClick_RestartBtn);
        closeBtn.onClick.AddListener(OnClick_CloseBtn);
    }

    private void OnDisable()
    {
        restartBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }
}
