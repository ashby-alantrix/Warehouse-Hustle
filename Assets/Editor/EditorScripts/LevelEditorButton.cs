using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorButton : MonoBehaviour
{
    public LevelEditorButtonType levelEditorButtonType;
    [HideInInspector] public Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }
}
