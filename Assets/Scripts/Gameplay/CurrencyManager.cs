using Unity.VisualScripting;
using UnityEngine;

public class CurrencyManager : MonoBehaviour, IBootLoader, IBase, IDataLoader
{
    public GameCurrencyData gameCurrencyData;
    public UserCurrencyData userCurrencyData;
    private InGameUIManager inGameUIManager;
    private UserDataBehaviour userDataBehaviour;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<CurrencyManager>(this);

        userDataBehaviour  = InterfaceManager.Instance?.GetInterfaceInstance<UserDataBehaviour>();
        inGameUIManager = InterfaceManager.Instance?.GetInterfaceInstance<InGameUIManager>();
    }

    public void InitializeData()
    {
        Debug.Log($"USERDATA: Initialize Data for currency system");

        gameCurrencyData = userDataBehaviour.GetGameCurrencyData();
        userCurrencyData = userDataBehaviour.GetUserCurrencyData();

        inGameUIManager.InGameHUDScreen.UpdateCurrencyText($"{userCurrencyData.attainedCurrency}");

        if (userDataBehaviour.IsFirstUserSession())
            AddCurrency(gameCurrencyData.initialCurrencyToProvide);
    }

    public void AddCurrency(int addAmt)
    {
        userCurrencyData.attainedCurrency += addAmt;
        UpdateCurrencyData();
    }

    public void WithdrawCurrency(int withdrawAmt)
    {
        userCurrencyData.attainedCurrency -= withdrawAmt;
        UpdateCurrencyData();
    }

    public void UpdateCurrencyData()
    {
        Debug.Log($"Updated currency data userCurrencyData.attainedCurrency: {userCurrencyData.attainedCurrency}");
        inGameUIManager.InGameHUDScreen.UpdateCurrencyText($"{userCurrencyData.attainedCurrency}");
        userDataBehaviour.SaveUserCurrencyData(userCurrencyData);
    }

    public bool HasEnoughCurrency(int availCurrency)
    {
        Debug.Log($"HasEnoughCurrency: {availCurrency} <= {userCurrencyData.attainedCurrency}");
        return availCurrency <= userCurrencyData.attainedCurrency;
            
    }
}
