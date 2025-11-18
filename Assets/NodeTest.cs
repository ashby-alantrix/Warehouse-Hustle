using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTest : MonoBehaviour
{
    private GridManagerTest gridManagerTest;

    private int row, col;

    public void OnMouseDown()
    {
        switch (LevelGeneratorPanel.levelEditorButtonState)
        {
            case LevelEditorButtonType.SHOW:
                // remove from blocked dict if added
                // regenerate
                gridManagerTest.RemoveBlockedGridValue(row, col);
                gridManagerTest.GenerateNewGridData();
                break;
            case LevelEditorButtonType.HIDE:
                // add to blocked dict
                // regenerate
                gridManagerTest.AddBlockedGridValue(row, col);
                gridManagerTest.GenerateNewGridData();
                break;
        }
    }

    public void InitGridManagerTest(GridManagerTest gridManagerTest, int row, int col)
    {
        this.row = row;
        this.col = col;

        this.gridManagerTest = gridManagerTest;
    }
}
