using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum LevelEditorButtonType
{
    SHOW,
    HIDE
}

public class LevelGeneratorPanel : MonoBehaviour
{
    [SerializeField] private LevelEditorButton[] levelEditorBtns;

    private bool showNodeToggle;
    private bool blockNodeToggle;

    private Button prevButton;
    private Button clickedButton;

    public static LevelEditorButtonType levelEditorButtonState;

    ////private Dictionary<LevelEditorButtonType, bool> toggleStates = new Dictionary<LevelEditorButtonType, bool>();

    ////private void Awake()
    ////{
    ////    foreach (var type in Enum.GetValues(typeof(LevelEditorButtonType)))
    ////    {
    ////        toggleStates.Add((LevelEditorButtonType)type, false);
    ////    }
    ////}

    ////public bool GetToggleState(LevelEditorButtonType type)
    ////{
    ////    return toggleStates[type];
    ////}

    public void OnShowNodeClicked(Button btn)
    {
        levelEditorButtonState = LevelEditorButtonType.SHOW;
        SetButtonStates(LevelEditorButtonType.SHOW);
    }

    public void OnHideNodeClicked(Button btn)
    {
        levelEditorButtonState = LevelEditorButtonType.HIDE;
        SetButtonStates(LevelEditorButtonType.HIDE);
    }

    private void SetButtonStates(LevelEditorButtonType type)
    {
        if (clickedButton != null)
        {
            prevButton = clickedButton;
            ColorBlock colorBlock = prevButton.colors;
            colorBlock.pressedColor = Color.white;
        }

        clickedButton = levelEditorBtns.FirstOrDefault(btnData => btnData.levelEditorButtonType == type).button;
        ColorBlock colorBlock1 = clickedButton.colors;
        colorBlock1.pressedColor = Color.grey;
    }

    ////public void SetToggleStates(LevelEditorButtonType type)
    ////{
    ////    foreach (var item in toggleStates)
    ////    {
    ////        toggleStates[item.Key] = item.Key == type;
    ////    }
    ////}
}
