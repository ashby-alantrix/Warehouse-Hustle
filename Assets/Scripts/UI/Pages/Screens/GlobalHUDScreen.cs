using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalHUDScreen : ScreenBase
{
    [SerializeField] private Button livesButton;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI remLivesText;
    [SerializeField] private GameObject livesButtonFilledCont;
    [SerializeField] private GameObject livesButtonNonFilledCont;

    private PopupManager popupManager;
    private SoundManager soundManager;
    private HealthSystem healthSystem;

    public void OnClick_Lives()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        popupManager.ShowPopup(PopupType.GetMoreLivesPopup);
    }

    // TODO :: Call when health is updated 
    public void UpdateLivesButtonContainer(bool isFull)
    {
        healthSystem = healthSystem == null ? InterfaceManager.Instance?.GetInterfaceInstance<HealthSystem>() : healthSystem;

        livesButtonFilledCont.SetActive(isFull);
        livesButtonNonFilledCont.SetActive(!isFull);

        if (!isFull)
        {
            Debug.Log($"healthSystem.AvailableLifes: {healthSystem.AvailableLifes}");
            remLivesText.text = $"{healthSystem.AvailableLifes}";
        }
    }

    public void UpdateCurrencyText(string coins)
    {
        Debug.Log($"coinsText: {coins}");
        coinsText.text = coins;
    }

    private void OnEnable()
    {
        Debug.Log($"OnEnable for GlobalHUDScreen is called");
        livesButton.onClick.AddListener(() => OnClick_Lives());
    }

    private void OnDisable()
    {
        livesButton.onClick.RemoveAllListeners();
    }
}
