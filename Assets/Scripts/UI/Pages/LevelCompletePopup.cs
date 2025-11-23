using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletePopup : UIBase
{
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button nextButton;

    private int currencyReward;
    private LevelManager levelManager;

    void Awake()
    {
        levelManager = InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>();
        nextButton.onClick.AddListener(() => OnClick_NextButton());
    }

    private void OnClick_NextButton()
    {
        InterfaceManager.Instance?.GetInterfaceInstance<CurrencyManager>()?.UpdateCoinsData(currencyReward);
        MainSingleton.Instance.LoadLevelsScene();
    }

    public void SetCoinsReward(int rewardAmt)
    {
        currencyReward = rewardAmt;
        rewardText.text = $"{rewardAmt}";
    }

    void OnDestroy()
    {
        nextButton.onClick.RemoveAllListeners();
    }
}
