using UnityEngine;
using UnityEngine.UI;

public class MenuHUDScreen : ScreenBase
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
        popupManager.ShowPopup(PopupType.GetMoreLivesPopup);
    }

    #region HEALTH TEST LOGIC
    #if UNITY_EDITOR
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
#endif
    #endregion

    void Awake()
    {
        #if UNITY_EDITOR
            toggleButton1.onClick.AddListener(() => OnClick_ToggleTest1());    
            toggleButton2.onClick.AddListener(() => OnClick_ToggleTest2());    
        #else 
            toggleButton1.gameObject.SetActive(false);
            toggleButton2.gameObject.SetActive(false);
        #endif
    }

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

    void OnDestroy()
    {
        #if UNITY_EDITOR
            toggleButton1.onClick.RemoveAllListeners();    
            toggleButton2.onClick.RemoveAllListeners();    
        #endif 
    }
}
