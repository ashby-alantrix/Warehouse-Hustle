using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, IBase, IBootLoader
{
    public bool IsInputEnabled
    {
        get;
        private set;
    }

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<InputManager>(this);
    }

    public void SetInputState(bool state)
    {
        IsInputEnabled = state;
    }
}
