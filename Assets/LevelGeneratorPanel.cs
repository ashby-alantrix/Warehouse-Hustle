using System.Collections;
using System.Collections.Generic;
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

    public void OnShowNodeClicked()
    {
        blockNodeToggle = false;
        showNodeToggle = true;

        // ColorBlock cb1 = showBtn.colors;
        // cb1.pressedColor = Color.grey;

        // ColorBlock cb2 = hideBtn.colors;
        // cb2.pressedColor = Color.white;
    }

    public void OnHideNodeClicked()
    {
        blockNodeToggle = true;
        showNodeToggle = false;
    }
}
