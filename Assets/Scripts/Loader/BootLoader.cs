using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class BootLoader : MonoBehaviour
{
    [Tooltip("For keeping track of all ui related objects: screens and popups")]
    [SerializeField] private GameObject[] uiBases;

    protected virtual void Start()
    {
        InterfaceManager.InitInstance();

        InitBootLoaders();
        InitializeUIBases();
        InitializeData();
    }

    private void InitializeUIBases()
    {
        if (uiBases != null && uiBases.Length > 0)
            foreach (GameObject uiBase in uiBases)
                uiBase.GetComponent<IUIBase>().Initialize();
    }

    protected abstract void InitBootLoaders();
    protected abstract void InitializeData();

    protected bool GetLoader<T>(Transform loader, out T outLoader)
    {
        outLoader = loader.GetComponent<T>();
        return outLoader != null;
    }
}
