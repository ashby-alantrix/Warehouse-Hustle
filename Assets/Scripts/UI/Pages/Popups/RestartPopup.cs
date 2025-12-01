using UnityEngine;
using UnityEngine.UI;

public class RestartPopup : PopupBase
{
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button closeBtn;

    private HealthSystem healthSystem;
    private LevelManager levelManager;
    private InputManager inputManager;
    private ScreenManager screenManager;

    public void OnClick_RestartBtn()
    {
        healthSystem = healthSystem == null ? InterfaceManager.Instance.GetInterfaceInstance<HealthSystem>() : healthSystem;
        levelManager = levelManager == null ? InterfaceManager.Instance.GetInterfaceInstance<LevelManager>() : levelManager;
        screenManager = screenManager == null ? InterfaceManager.Instance.GetInterfaceInstance<ScreenManager>() : screenManager;

        if (healthSystem.AvailableLifes > 1)
        {
            screenManager.GetScreen<InGameHUDScreen>(ScreenType.InGameHUDScreen).ShowSettingDropdown();
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
        inputManager = inputManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputManager>() : inputManager;

        inputManager?.SetInputState(true);
        popupManager.HidePopup(popupType);
    }

    private void OnEnable()
    {
        restartBtn.onClick.AddListener(OnClick_RestartBtn);
        closeBtn.onClick.AddListener(OnClick_CloseBtn);

        inputManager = inputManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputManager>() : inputManager;
        inputManager?.SetInputState(false);
    }

    private void OnDisable()
    {
        restartBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();

        Invoke(nameof(OnPopupClosed), 0.5f);
    }

    private void OnPopupClosed()
    {
        inputManager = inputManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputManager>() : inputManager;
        inputManager?.SetInputState(true);
    }
}
