using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupType
{
    RefreshInputGoods,
    ClearGoods,
    SwapGoods,
}

public class Powerup : MonoBehaviour
{
    [SerializeField] private GoodsHandler goodsHandler;

    [SerializeField] private int currencyVal = 50;
    [SerializeField] private PowerupType powerupType;

    private CurrencyManager currencyManager;
    private PopupManager popupManager;

    public void OnClick_Powerup()
    {
        Debug.Log($"OnMouseDown");
        currencyManager = currencyManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<CurrencyManager>() : currencyManager;   
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;   
        if (currencyManager.HasEnoughCurrency(currencyVal))
        {
            currencyManager.WithdrawCurrency(currencyVal);
            OnPowerupClicked();
        }
        else
        {
            // popupManager.GetPopup<FeedbackPopup>(PopupType.FeedbackPopup).SetFeedbackText();
            // popupManager.ShowPopup(PopupType.FeedbackPopup);
            popupManager.GetPopup<FeedbackPopup>(PopupType.FeedbackPopup).SetFeedbackText("NOT ENOUGH COINS AVAILABLE");
            popupManager.ShowPopup(PopupType.FeedbackPopup);
        }
    }

    private void OnPowerupClicked()
    {
        switch (powerupType)
        {
            case PowerupType.ClearGoods:
            Debug.Log($"Clear goods");
                goodsHandler.SetClearPowerupState(true);
            break;
            case PowerupType.RefreshInputGoods:
                goodsHandler.OnRefreshInputNodes();
            break;
            case PowerupType.SwapGoods:
            break;
        }
    }
}
