using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeRefillPopup : PopupBase
{
    [SerializeField] private Button refillBtn;
    [SerializeField] private Button closeBtn;

    private HealthSystem healthSystem;

    private void OnEnable()
    {
        refillBtn.onClick.AddListener(() => OnClick_RefillBtn());
        closeBtn.onClick.AddListener(() => OnClick_CloseBtn());
    }

    private void OnDisable()
    {
        refillBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    private void OnClick_RefillBtn()
    {
        popupManager.HidePopup(popupType);
        healthSystem = healthSystem == null ? InterfaceManager.Instance.GetInterfaceInstance<HealthSystem>() : healthSystem;
        healthSystem.RefillHealth();
        healthSystem.SetFreeRefillState(true);

        OnComplete(PopupResultEvent.LivesFull);
    }

    private void OnClick_CloseBtn()
    {
        popupManager.HidePopup(popupType);
        OnComplete(PopupResultEvent.OnCancelRefillHealth);
    }
}
