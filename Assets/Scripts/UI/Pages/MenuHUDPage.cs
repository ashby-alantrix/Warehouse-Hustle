using UnityEngine;
using UnityEngine.UI;

public class MenuHUDPage : UIBase
{
    [SerializeField] private Button coinsButton;
    [SerializeField] private Button livesButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button toggleButton1;
    [SerializeField] private Button toggleButton2;

    private PopupManager popupManager;

    public void OnClick_LifesButton()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        popupManager.ShowScreen(UIType.GetMoreLivesPopup);
    }

    public void OnClick_ToggleTest1()
    {
        HealthSystem healthSystem = InterfaceManager.Instance.GetInterfaceInstance<HealthSystem>();
        healthSystem.UpdateAvailableLives(-1);
    }

    public void OnClick_ToggleTest2()
    {
        HealthSystem healthSystem = InterfaceManager.Instance.GetInterfaceInstance<HealthSystem>();
        healthSystem.UpdateAvailableLives(1);
    }

    void OnEnable()
    {
        livesButton.onClick.AddListener(() => OnClick_LifesButton());    
        toggleButton1.onClick.AddListener(() => OnClick_ToggleTest1());    
        toggleButton2.onClick.AddListener(() => OnClick_ToggleTest2());    
    }

    void OnDisable()
    {
        coinsButton.onClick.RemoveAllListeners();
        livesButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
    }
}
