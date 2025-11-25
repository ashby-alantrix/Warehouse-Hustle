using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : PopupBase
{
    [SerializeField] private Button playUsingCurrencyBtn;
    [SerializeField] private TextMeshProUGUI remGoodsToSort;
    [SerializeField] private Button reviveBtn;
    [SerializeField] private Button closeBtn;

    private NodeManager nodeManager;
    private PopupManager popupManager;
    private LevelManager levelManager;
    private CurrencyManager currencyManager;
    private GoodsSortingManager goodsSortingManager;

    private int nodesToClear = 5;
    private int clearCurrency = 200;

    new void OnEnable()
    {
        // base.OnEnable();
        playUsingCurrencyBtn.onClick.AddListener(() => OnClick_PlayUsingCurrency());
        closeBtn.onClick.AddListener(() => OnClick_CloseBtn());
    }

    new void OnDisable()
    {
        // base.OnDisable();
        playUsingCurrencyBtn.onClick.RemoveAllListeners();
        reviveBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    public void InitData(int remGoods)
    {
        remGoodsToSort.text = $"{remGoods}";
    }

    private void OnClick_PlayUsingCurrency()
    {
        currencyManager = currencyManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<CurrencyManager>() : currencyManager;
        SetPopupManager();

        if (!currencyManager.HasEnoughCurrency(clearCurrency))
        {
            // show feedback message -> not enough coins
            return;
        }

        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
        currencyManager.WithdrawCurrency(clearCurrency);

        popupManager.HideActivePopup();
        nodeManager = nodeManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<NodeManager>() : nodeManager;
        var nodeKeys = nodeManager.GetRandomNodeKeys(count: nodesToClear, startIndex: 0);
        Node foundNode = null;

        Debug.Log($"NodeKeysCount: {nodeKeys.Count}");

        foreach (var nodekey in nodeKeys)
        {
            if (nodeManager.IsNodeAvailableInGrid(nodekey, out foundNode))
            {
                foundNode.ClearOrResetGoodsDataAndView();
            }
        }
        goodsSortingManager = goodsSortingManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GoodsSortingManager>() : goodsSortingManager;
        goodsSortingManager.ClearConnectedNodes();

        levelManager.OnLevelStateChange(LevelState.Progress);
    }

    protected void OnClick_CloseBtn()
    {
        SetPopupManager();
        popupManager.HidePopup(popupType);

        // cut out one health
        popupManager.ShowPopup(PopupType.RestartPopup);
    }

    private void SetPopupManager()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
    }
}
