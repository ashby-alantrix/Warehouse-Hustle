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
            UpdateCoinsData(gameCurrencyData.initialCurrencyToProvide);
    }

    public void UpdateCoinsData(int coinsAmt)
    {
        userCurrencyData.attainedCurrency += coinsAmt;

        inGameUIManager.InGameHUDScreen.UpdateCurrencyText($"{userCurrencyData.attainedCurrency}");
        userDataBehaviour.SaveUserCurrencyData(userCurrencyData);
    }

    public bool HasEnoughCurrency(int availCurrency)
    {
        return availCurrency <= userCurrencyData.attainedCurrency;
            
    }
}
