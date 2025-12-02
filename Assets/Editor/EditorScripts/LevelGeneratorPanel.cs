using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum LevelEditorButtonType
{
    NONE,
    SHOW,
    HIDE,
    SHOW_BLOCKED_MESH,
    HIDE_BLOCKED_MESH,
    GENERATE_GRID_DATA
}

public class LevelGeneratorPanel : MonoBehaviour
{
    [SerializeField] private GridManagerEditor gridManagerEditor;
    [SerializeField] private GridEditorManager gridEditorManager;

    [SerializeField] private LevelEditorButton[] levelEditorBtns;

    private bool showNodeToggle;
    private bool blockNodeToggle;

    private Button prevButton;
    private Button clickedButton;

    public static LevelEditorButtonType levelEditorButtonState;

    public void OnClick_GenerateNewGrid()
    {
        levelEditorButtonState = LevelEditorButtonType.GENERATE_GRID_DATA;
        SetButtonStates(LevelEditorButtonType.GENERATE_GRID_DATA);
        gridEditorManager.GenerateNewGridData();
    }

    public void OnClick_ShowNodeBtn()
    {
        levelEditorButtonState = LevelEditorButtonType.SHOW;
        SetButtonStates(LevelEditorButtonType.SHOW);
    }

    public void OnClick_HideNodeBtn()
    {
        levelEditorButtonState = LevelEditorButtonType.HIDE;
        SetButtonStates(LevelEditorButtonType.HIDE);
    }

    public void OnClick_ShowBlockedMeshes()
    {
        levelEditorButtonState = LevelEditorButtonType.SHOW_BLOCKED_MESH;
        SetButtonStates(LevelEditorButtonType.SHOW_BLOCKED_MESH);

        gridManagerEditor.ToggleAllBlockedSets(true);
        gridManagerEditor.SetBlockedMeshState(true);
    }

    public void OnClick_HideBlockedMeshes()
    {
        levelEditorButtonState = LevelEditorButtonType.HIDE_BLOCKED_MESH;
        SetButtonStates(LevelEditorButtonType.HIDE_BLOCKED_MESH);

        gridManagerEditor.ToggleAllBlockedSets(false);
        gridManagerEditor.SetBlockedMeshState(false);
    }

    public void OnClick_GenerateJson()
    {
        gridManagerEditor.GenerateJsonUsingGridData();
    }

    private void SetButtonStates(LevelEditorButtonType type)
    {
        if (clickedButton != null)
        {
            prevButton = clickedButton;
            ColorBlock colorBlock = prevButton.colors;
            colorBlock.selectedColor = Color.white;
        }

        clickedButton = levelEditorBtns.FirstOrDefault(btnData => btnData.levelEditorButtonType == type).button;
        ColorBlock colorBlock1 = clickedButton.colors;
        colorBlock1.selectedColor = Color.magenta;
    }
}
