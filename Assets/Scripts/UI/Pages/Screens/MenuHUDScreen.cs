using UnityEngine;
using UnityEngine.UI;

public class MenuHUDScreen : ScreenBase
{
    [SerializeField] private Button coinsButton;
    [SerializeField] private Button livesButton;
    [SerializeField] private Button settingsButton;

    private PopupManager popupManager;

    public void OnClick_LifesButton()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        popupManager.ShowPopup(PopupType.GetMoreLivesPopup);
    }

    #region HEALTH TEST LOGIC
    #if UNITY_EDITOR
    public void OnClick_ToggleTest1()
    {
        HealthSystem healthSystem = InterfaceManager.Instance.GetInterfaceInstance<HealthSystem>();
        healthSystem.RemoveHealth(1);
    }

    public void OnClick_ToggleTest2()
    {
        HealthSystem healthSystem = InterfaceManager.Instance.GetInterfaceInstance<HealthSystem>();
        healthSystem.AddHealth(1);
    }
#endif
    #endregion

    void OnEnable()
    {
        livesButton.onClick.AddListener(() => OnClick_LifesButton());    

        
    }

    void OnDisable()
    {
        coinsButton.onClick.RemoveAllListeners();
        livesButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
    }
}
