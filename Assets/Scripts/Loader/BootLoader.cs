using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class BootLoader : MonoBehaviour
{
    [Tooltip("For keeping track of ui panels in the specific scene")]
    [SerializeField] private GameObject[] uiBases;

    protected virtual void Start()
    {
        InterfaceManager.InitInstance();

        InitBootLoaders();

        if (uiBases != null && uiBases.Length > 0)
            foreach (GameObject uiBase in uiBases)
                uiBase.GetComponent<IUIBase>().Initialize();
            
        InitializeDataLoaders();
    }

    protected abstract void InitBootLoaders();
    protected abstract void InitializeDataLoaders();
}
