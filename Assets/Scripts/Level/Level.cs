using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private int levelNum;
    [SerializeField] private GameObject playBtn;
    [SerializeField] private GameObject restartBtn;
    [SerializeField] private GameObject barricade;
    [SerializeField] private TextMeshProUGUI levelText;

    public int LevelNum => levelNum;

    public bool HasBarricade { get; internal set; }

    public void OnLevelCompleted()
    {
        TogglePlayBtnState(false);
        // grey out the level object sprite or replace the sprite with a greyed out one
    }

    public void OnLevelUnlocked()
    {
        // change to level selected sprite and scale the level object
        if (HasBarricade)
        {
            return;
        }
    }

    public void SetLevelText(int levelNum)
    {
        this.levelNum = levelNum;
        levelText.text = $"LEVEL {levelNum}";
    }

    public void TogglePlayBtnState(bool state)
    {
        playBtn.SetActive(state);
    }

    public void SetLevelEndBarricade()
    {
        HasBarricade = true;
        barricade.SetActive(true);
    }

    public void ShowRestartButton()
    {
        restartBtn.SetActive(true);
    }
}
