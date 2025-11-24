using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : UIBase
{
    [SerializeField] private Button playUsingCurrencyBtn;
    [SerializeField] private Button reviveBtn;
    [SerializeField] private Button closeBtn;

    private NodeManager nodeManager;
    private PopupManager popupManager;
    private LevelManager levelManager;
    private CurrencyManager currencyManager;

    private int nodesToClear = 5;
    private int clearCurrency = 200;

    new void OnEnable()
    {
        // base.OnEnable();
        playUsingCurrencyBtn.onClick.AddListener(() => OnClick_PlayUsingCurrency());
        reviveBtn.onClick.AddListener(() => OnClick_PlayByReviving());
        closeBtn.onClick.AddListener(() => OnClick_CloseBtn());
    }

    new void OnDisable()
    {
        // base.OnDisable();
        playUsingCurrencyBtn.onClick.RemoveAllListeners();
        reviveBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    public void InitData()
    {
        
    }

    private void OnClick_PlayUsingCurrency()
    {
        SetCurrencyManager();
        SetPopupManager();

        if (!currencyManager.HasEnoughCurrency(clearCurrency))
        {
            // show feedback message -> not enough coins
            return;
        }

        SetLevelManager();

        popupManager.HideActiveScreen();
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

        levelManager.OnLevelStateChange(LevelState.Progress);
    }

    private void OnClick_PlayByReviving()
    {
        
    }

    protected void OnClick_CloseBtn()
    {
        SetPopupManager();
        popupManager.HideScreen(uiType);

        // cut out one health
        popupManager.ShowScreen(UIType.RestartPopup);
    }

    private void SetPopupManager()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
    }

    private void SetLevelManager()
    {
        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
    }

    private void SetCurrencyManager()
    {
        currencyManager = currencyManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<CurrencyManager>() : currencyManager;
    }
}
