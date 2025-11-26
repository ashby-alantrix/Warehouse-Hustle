using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetMoreLivesPopup : PopupBase
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
    private int purchaseCurrencyValue = 60;
    private HealthSystem healthSystem;
    private PopupManager popupManager;
    private CurrencyManager currencyManager;

    private const string LIFES_FULL_TEXT = "LIVES";
    private const string LIFES_TO_FULL_TEXT = "GET MORE LIVES";

    public bool IsLifeToFillContentActive => lifeToFillContent.activeInHierarchy;

    public void UpdateAvailableLifes(int availLifes)
    {
        currentEnabledLifeIcons = availLifes;
        Debug.Log($"GetMoreLivesPopup life: {availLifes}");
        Debug.Log($"GetMoreLivesPopup currentEnabledLifeIcons: {currentEnabledLifeIcons}");

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

    public void OnClick_PurchaseCurrency()
    {
        Debug.Log($"GetMoreLivesPopup: {currencyManager.HasEnoughCurrency(purchaseCurrencyValue)}");
        if (currencyManager.HasEnoughCurrency(purchaseCurrencyValue))
        {
            Debug.Log($"GetMoreLivesPopup: currencyManager.HasEnoughCurrency(purchaseCurrencyValue): {currencyManager.HasEnoughCurrency(purchaseCurrencyValue)}");
            currencyManager.WithdrawCurrency(purchaseCurrencyValue);
            healthSystem.AddHealth(1);
        }
    }

    private void OnEnable()
    {
        lifeFullCloseBtn.onClick.AddListener(() => OnClosePopup());
        lifeToFillCloseBtn.onClick.AddListener(() => OnClosePopup());
        purchaseCurrencyBtn.onClick.AddListener(() => OnClick_PurchaseCurrency());

        InitManagers();

        SetHealthContent(healthSystem.IsFull);
        purchaseCurrencyValue = healthSystem.GameHealthData.singleHealthCurrencyValue;
        purchaseCurrencyText.text = $"{purchaseCurrencyValue}";

        currentEnabledLifeIcons = healthSystem.AvailableLifes;

        for (int indexI = 0; indexI < lifeImages.Length; indexI++)
        {
            lifeImages[indexI].enabled = indexI <= currentEnabledLifeIcons - 1;
        }
    }

    private void InitManagers()
    {
        healthSystem = healthSystem == null ? InterfaceManager.Instance?.GetInterfaceInstance<HealthSystem>() : healthSystem;
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        currencyManager = currencyManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<CurrencyManager>() : currencyManager;
    }

    private void OnDisable()
    {
        lifeFullCloseBtn.onClick.RemoveAllListeners();
        lifeToFillCloseBtn.onClick.RemoveAllListeners();
        purchaseCurrencyBtn.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (lifeFullContent.activeInHierarchy) return;

        timerText.text = healthSystem.GetFormattedTime();
    }

    private void OnClosePopup()
    {
        popupManager.HidePopup(PopupType.GetMoreLivesPopup);
    }
}
