using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    private Button buttonComp;
    private SoundManager soundManager;

    private void Awake()
    {
        buttonComp = GetComponent<Button>();
    }

    private void OnEnable()
    {
        buttonComp.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        buttonComp.onClick.RemoveListener(OnClick);
    }

    public void OnClick()
    {
        SetSoundManager();
        soundManager.PlayButtonSoundClip(SoundType.Button_Click);
    }

    private void SetSoundManager()
    {
        soundManager = soundManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<SoundManager>() : soundManager;
    }
}
