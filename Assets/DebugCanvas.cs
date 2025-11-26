using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugCanvas : MonoBehaviour
{
    [SerializeField] private Button toggleButton1;
    [SerializeField] private Button toggleButton2;

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

    void OnDestroy()
    {
        #if UNITY_EDITOR
            toggleButton1.onClick.RemoveAllListeners();    
            toggleButton2.onClick.RemoveAllListeners();    
        #endif 
    }
}
