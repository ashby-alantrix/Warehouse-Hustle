using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBootLoader
{
    public void Initialize();
}

public class BootLoader : MonoBehaviour
{
    

    private void Start()
    {
        InterfaceManager interfaceManager = new InterfaceManager();
    }
}
