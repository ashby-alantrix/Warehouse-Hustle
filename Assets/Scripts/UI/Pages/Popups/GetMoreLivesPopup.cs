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
    private CurrencyManager currencyManager;
    private InputManager inputManager;

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
        SetLivesContentTransform(isFull);

        lifeFullContent.SetActive(isFull);
        lifeToFillContent.SetActive(!isFull);
    }

    public void ResetContentScale()
    {
        popupScaleContent.localScale = Vector3.one;
    }

    private void SetLivesContentTransform(bool isFull)
    {
        popupScaleContent = isFull ? lifeFullContent.transform : lifeToFillContent.transform;
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
        else
        {
            popupManager.GetPopup<FeedbackPopup>(PopupType.FeedbackPopup).SetFeedbackText($"NOT ENOUGH COINS");
            popupManager.ShowPopup(PopupType.FeedbackPopup);
        }
    }

    private void OnEnable()
    {
        InitManagers();

        lifeFullCloseBtn.onClick.AddListener(() => OnClosePopup());
        lifeToFillCloseBtn.onClick.AddListener(() => OnClosePopup());
        purchaseCurrencyBtn.onClick.AddListener(() => OnClick_PurchaseCurrency());

        SetHealthContent(healthSystem.IsFull);
        purchaseCurrencyValue = healthSystem.GameHealthData.singleHealthCurrencyValue;
        purchaseCurrencyText.text = $"{purchaseCurrencyValue}";

        currentEnabledLifeIcons = healthSystem.AvailableLifes;

        for (int indexI = 0; indexI < lifeImages.Length; indexI++)
        {
            lifeImages[indexI].enabled = indexI <= currentEnabledLifeIcons - 1;
        }

        inputManager = inputManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputManager>() : inputManager;
        inputManager?.SetInputState(false);
    }

    private void InitManagers()
    {
        currencyManager = currencyManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<CurrencyManager>() : currencyManager;
        healthSystem = healthSystem == null ? InterfaceManager.Instance?.GetInterfaceInstance<HealthSystem>() : healthSystem;
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
    }

    private void OnDisable()
    {
        lifeFullCloseBtn.onClick.RemoveAllListeners();
        lifeToFillCloseBtn.onClick.RemoveAllListeners();
        purchaseCurrencyBtn.onClick.RemoveAllListeners();

        Invoke(nameof(OnPopupClosed), 0.5f);
    }

    private void OnPopupClosed()
    {
        inputManager = inputManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputManager>() : inputManager;
        inputManager?.SetInputState(true);
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
