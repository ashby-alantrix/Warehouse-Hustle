using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IBootLoader, IBase
{
    

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GameManager>(this);
    }

    
}
