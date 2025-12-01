using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUDScreen : ScreenBase
{
    [SerializeField] private Animator animator;
    
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI goodsGoalCountText;
    [SerializeField] private Button hudButtonsCont;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button soundBtn;
    [SerializeField] private Image soundSprite;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private GameObject settingsDropdown;

    private int goodsGoalCount = 0;
    private LevelManager levelManager;
    private PopupManager popupManager;
    private SoundManager soundManager;
    private bool showSettingDropDown = false;

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

    void OnEnable()
    {
        restartBtn.onClick.AddListener(() => OnClick_RestartButton());
        hudButtonsCont.onClick.AddListener(() =>
        {
            ShowSettingDropdown();
        });

        homeBtn.onClick.AddListener(() => OnClick_HomeButton());
        soundBtn.onClick.AddListener(() => OnClick_SoundButton());
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

    private void OnClick_SoundButton()
    {
        SetSoundManager();
        soundManager.SetGameSound(!soundManager.IsGameSoundOn);
        SetSoundSprite();
    }

    private void SetSoundManager()
    {
        soundManager = soundManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<SoundManager>() : soundManager;
    }

    private void SetSoundSprite()
    {
        SetSoundManager();
        soundSprite.sprite = soundManager.IsGameSoundOn ? soundOnSprite : soundOffSprite;
    }

    public void ShowSettingDropdown()
    {
        SetSoundSprite();

        showSettingDropDown = !showSettingDropDown;
        if (showSettingDropDown)
        {
            if (settingsDropdown.activeInHierarchy)
            {
                animator.Play("Open");
                return;
            }

            settingsDropdown.SetActive(true);
        }
        else 
        {
            animator.Play("Close");
            // Invoke(nameof(DisableSettingDropdown), 1f);
        }
    }

    public void DisableSettingDropdown()
    {
        Debug.Log($"Disable Setting Dropdown");
        settingsDropdown.SetActive(false);
    }

    void OnDisable()
    {
        hudButtonsCont.onClick.RemoveAllListeners();
    }
}
