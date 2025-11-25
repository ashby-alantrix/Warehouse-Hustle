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
    [SerializeField] private Button settingsButton;

    private int goodsGoalCount = 0;
    private LevelManager levelManager;

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
}
