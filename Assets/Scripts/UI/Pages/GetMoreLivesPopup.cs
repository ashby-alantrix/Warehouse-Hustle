using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetMoreLivesPopup : UIBase
{
    [SerializeField] private GameObject lifeFullContent;
    [SerializeField] private GameObject lifeToFillContent;

    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Image[] lifeImages;

    [SerializeField] private Button purchaseCurrencyBtn;
    [SerializeField] private TextMeshProUGUI purchaseCurrencyText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button lifeFullCloseBtn;
    [SerializeField] private Button lifeToFillCloseBtn;

    private int currentEnabledLifeIcons;
    private int purchaseCurrencyValue = 50;
    private HealthSystem healthSystem;
    private PopupManager popupManager;

    private const string LIFES_FULL_TEXT = "LIVES";
    private const string LIFES_TO_FULL_TEXT = "GET MORE LIVES";

    public bool IsLifeToFillContentActive => lifeToFillContent.activeInHierarchy;

    public void UpdateAvailableLifes(int availLifes)
    {
        currentEnabledLifeIcons = availLifes;
        Debug.Log($"life: {availLifes}");
        Debug.Log($"currentEnabledLifeIcons: {currentEnabledLifeIcons}");

        if (Mathf.Sign(availLifes) > 0)
            lifeImages[currentEnabledLifeIcons - 1].enabled = true;
        else 
            lifeImages[currentEnabledLifeIcons].enabled = false;
    }

    public void SetHealthContent(bool isFull)
    {
        lifeFullContent.SetActive(isFull);
        lifeToFillContent.SetActive(!isFull);
    }

    private void OnEnable() 
    {
        lifeFullCloseBtn.onClick.AddListener(() => OnClosePopup());
        lifeToFillCloseBtn.onClick.AddListener(() => OnClosePopup());

        healthSystem = healthSystem == null ? InterfaceManager.Instance?.GetInterfaceInstance<HealthSystem>() : healthSystem;
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;

        purchaseCurrencyValue = healthSystem.GameHealthData.singleHealthCurrencyValue;
        purchaseCurrencyText.text = $"{purchaseCurrencyValue}";

        currentEnabledLifeIcons = healthSystem.AvailableLifes;

        for (int indexI = 0; indexI < lifeImages.Length; indexI++)
        {
            lifeImages[indexI].enabled = indexI <= currentEnabledLifeIcons - 1;
        }
    }


    private void OnDisable()
    {
        lifeFullCloseBtn.onClick.RemoveAllListeners();
        lifeToFillCloseBtn.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (lifeFullContent.activeInHierarchy) return;

        timerText.text = healthSystem.GetFormattedTime();
    }

    private void OnClosePopup()
    {
        popupManager.HideScreen(UIType.GetMoreLivesPopup);
    }
}
