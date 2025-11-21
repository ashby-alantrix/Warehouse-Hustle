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

    private LevelManager levelManager;

    void Awake()
    {
        levelManager = InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>();
        nextButton.onClick.AddListener(() => OnClick_NextButton());
    }

    private void OnClick_NextButton()
    {
        // levelManager.SetCurrentLevelNumber(levelManager.CurrentLevelNumber + 1);
        MainSingleton.Instance.LoadLevelsScene();
    }

    public void SetCoinsReward(int coinsReward)
    {
        rewardText.text = $"{coinsReward}";
    }

    void OnDestroy()
    {
        nextButton.onClick.RemoveAllListeners();
    }
}
