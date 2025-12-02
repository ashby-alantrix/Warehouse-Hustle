using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeEditor : MonoBehaviour
{
    [SerializeField] private Transform childTrans;
    [SerializeField] private MeshRenderer meshRend;

    private GridEditorManager gridEditorManager;

    public Transform ChildTransform => childTrans;

    private int row, col;

    public int Row => row;
    public int Col => col;

    public void OnMouseDown()
    {
        switch (LevelGeneratorPanel.levelEditorButtonState)
        {
            case LevelEditorButtonType.SHOW:
                // remove from blocked dict if added
                // regenerate
                Debug.Log($"OnMouseDown SHOW Row: {row}, col: {col}");
                gridEditorManager.RemoveBlockedGridValue(row, col);
                gridEditorManager.GenerateNewGridData();
                break;
            case LevelEditorButtonType.HIDE:
                // add to blocked dict
                // regenerate
                Debug.Log($"OnMouseDown Hide Row: {row}, col: {col}");
                gridEditorManager.AddBlockedGridValue(row, col);
                gridEditorManager.GenerateNewGridData();
                break;
            default:
                break;
        }
    }

    public void InitGridManagerTest(GridEditorManager gridManagerTest, int row, int col)
    {
        this.row = row;
        this.col = col;

        this.gridEditorManager = gridManagerTest;
    }

    public void ToggleMeshState(bool state)
    {
        meshRend.enabled = state;
        // childTrans.gameObject.SetActive(state != false);
    }
}
