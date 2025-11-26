using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUDScreen : ScreenBase
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI goodsGoalCountText;
    [SerializeField] private Button hudButtonsCont;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private GameObject settingsDropdown;

    private int goodsGoalCount = 0;
    private LevelManager levelManager;
    private PopupManager popupManager;

    public void Init()
    {
        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
        var levelsInfo = levelManager.GetCurrentLevelsInfo();
        InitLevelInfo(levelManager.CurrentLevelNumber, levelsInfo.targetGoodsToLoad);
    }

    private void InitLevelInfo(int levelNum, int goodsGoalCount)
    {
        levelText.text = $"LEVEL {levelNum}";
        this.goodsGoalCount = goodsGoalCount;
        goodsGoalCountText.text = $"{0}/{goodsGoalCount}";
    }

    public void SetGoodsGoalText(int loadedGoods)
    {
        goodsGoalCountText.text = $"{loadedGoods}/{goodsGoalCount}";
    }

    public void UpdateCurrencyText(string coins)
    {
        Debug.Log($"coinsText: {coins}");
        coinsText.text = coins;
    }

    void OnEnable()
    {
        restartBtn.onClick.AddListener(() => OnClick_RestartButton());
        hudButtonsCont.onClick.AddListener(() =>
        {
            ShowSettingDropdown(true);
        });
        homeBtn.onClick.AddListener(() => OnClick_HomeButton());
    }

    private void OnClick_RestartButton()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        popupManager.ShowPopup(PopupType.RestartPopup);
    }

    private void OnClick_HomeButton()
    {
        MainSingleton.Instance.LoadLevelsScene();
    }

    private void OnClick_SettingsButton()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        popupManager.ShowPopup(PopupType.SettingsPopup);
    }

    private void ShowSettingDropdown(bool state)
    {
        settingsDropdown.SetActive(state);
    }

    void OnDisable()
    {
        hudButtonsCont.onClick.RemoveAllListeners();
    }
}
