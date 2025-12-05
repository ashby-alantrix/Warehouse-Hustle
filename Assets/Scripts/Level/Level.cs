using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [SerializeField] private Vector3 position;
    [SerializeField] private GameObject unselectedLevelObject;
    [SerializeField] private GameObject selectedLevelObject;

    [SerializeField] private float animScale = 1f;
    [SerializeField] private float btnScaleDelay = 0.5f;
    [SerializeField] private Transform levelContent;
    
    [SerializeField] private int levelNum;
    [SerializeField] private GameObject playBtn;
    [SerializeField] private GameObject restartBtn;
    [SerializeField] private GameObject barricade;
    [SerializeField] private TextMeshProUGUI selLevelText;
    [SerializeField] private TextMeshProUGUI unselLevelText;

    [Header("Buttons")]
    [SerializeField] private Button levelImageBtn;
    [SerializeField] private Button levelTextImageBtn;
    [SerializeField] private Button restartBtnRef;
    [SerializeField] private Button playBtnRef;

    private Transform btnContent;

    public int LevelNum => levelNum;

    public bool HasBarricade { get; internal set; }

    private LevelManager levelManager;
    private bool isSelected = false;

    void Awake()
    {
        position = transform.position;
    }

    void OnEnable()
    {
        restartBtnRef.onClick.AddListener(OnClick_RestartButton);
        playBtnRef.onClick.AddListener(OnClick_PlayButton);

        Debug.Log($"### HasBarricade: {HasBarricade}");
        levelImageBtn.enabled = levelTextImageBtn.enabled = false;
    }

    void OnDisable()
    {
        playBtnRef.onClick.RemoveAllListeners();
        levelImageBtn.onClick.RemoveAllListeners();
        levelTextImageBtn.onClick.RemoveAllListeners();
        restartBtnRef.onClick.RemoveAllListeners();
    }

    public void InitListenersLevelObject()
    {
        levelImageBtn.onClick.RemoveAllListeners();
        levelTextImageBtn.onClick.RemoveAllListeners();
        
        if (!HasBarricade)
        {
            levelImageBtn.onClick.AddListener(OnClick_PlayButton);
            levelTextImageBtn.onClick.AddListener(OnClick_PlayButton);    
        }
        else
        {
            levelImageBtn.onClick.AddListener(OnClick_RestartButton);
            levelTextImageBtn.onClick.AddListener(OnClick_RestartButton);
        }

        levelImageBtn.enabled = levelTextImageBtn.enabled = true;
    }

    public void ShowUnselectedLevelView()
    {
        isSelected = false;
        TogglePlayBtnState(false);
        // grey out the level object sprite or replace the sprite with a greyed out one
        ToggleLevelObjectStates(false);
    }

    public void ShowSelectedLevelView()
    {
        isSelected = true;
        // change to level selected sprite and scale the level object
        ToggleLevelObjectStates(true);
        // if (HasBarricade)
        // {
        //     return;
        // }

    }

    public void ToggleLevelObjectStates(bool state)
    {
        selectedLevelObject.SetActive(state);
        unselectedLevelObject.SetActive(!state);
    }

    public void PlayScaleInAnims()
    {
        ScaleLevelContent();
        if (playBtn.activeInHierarchy)
        {
            btnContent = playBtn.transform;
            Invoke(nameof(ScaleLevelButton), btnScaleDelay);
        }
        else if (restartBtn.activeInHierarchy)
        {
            btnContent = restartBtn.transform;
            Invoke(nameof(ScaleLevelButton), btnScaleDelay);
        }
    }

    private void ScaleLevelContent()
    {
        Debug.Log($"ScaleLevelContent");
        levelContent.localScale = Vector3.zero;
        Tween tween = levelContent.DOScale(Vector3.one, animScale);
        tween.OnComplete(() => 
        {
            tween.Kill();
        });
    }

    public void ScaleLevelButton()
    {
        btnContent.localScale = Vector3.zero;
        btnContent.DOScale(Vector3.one, animScale);
    }

    public void SetLevelText(int levelNum)
    {
        this.levelNum = levelNum;
        if (isSelected)
            selLevelText.text = $"LEVEL {levelNum}";
        else 
            unselLevelText.text = $"LEVEL {levelNum}";
    }

    public void TogglePlayBtnState(bool state)
    {
        Debug.Log($"LevelNum :: {levelNum}, {state}");
        playBtn.SetActive(state);
        if (playBtn.transform.localScale == Vector3.zero)
        {
            btnContent = playBtn.transform;
            Invoke(nameof(ScaleLevelButton), btnScaleDelay);
        }
    }

    public void SetLevelEndBarricade()
    {
        HasBarricade = true;
        Debug.Log($"### HasBarricade :: SetLevelEndBarricade");
        // barricade.SetActive(true);
        selLevelText.text = $"Market closed";
        unselLevelText.text = $"Market closed";
    }

    public void ShowRestartButton()
    {
        restartBtn.SetActive(true);

        levelImageBtn.onClick.RemoveAllListeners();
        levelTextImageBtn.onClick.RemoveAllListeners();

        levelImageBtn.onClick.AddListener(OnClick_RestartButton);
        levelTextImageBtn.onClick.AddListener(OnClick_RestartButton);

        btnContent = restartBtn.transform;
        Invoke(nameof(ScaleLevelButton), btnScaleDelay);
    }

    private void SetLevelManager()
    {
        levelManager = levelManager == null ? InterfaceManager.Instance.GetInterfaceInstance<LevelManager>() : levelManager;
    }

    public void OnClick_PlayButton()
    {
        Debug.Log($"On click Play button");

        SetLevelManager();

        levelManager.LoadLevelInGame();
    }

    public void OnClick_RestartButton()
    {
        SetLevelManager();
        levelManager.OnRestartWholeLevels();
    }
}
