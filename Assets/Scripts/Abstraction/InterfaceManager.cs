using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    private Dictionary<string, IBase> interfacesDict = new Dictionary<string, IBase>();

    #region Singleton
    public static InterfaceManager Instance { get; private set; }

    public static void InitInstance()
    {
        if (Instance == null)
        {
            Instance = new InterfaceManager();
            Debug.Log("Initialized interface manager");
        }
        Debug.Log($"Initialized interface manager: {Instance}");
    }

    // // void Awake()
    // // {
    // //     if (Instance == null)
    // //         Instance = this;
    // //     else 
    // //         DestroyImmediate(gameObject);
        
    // //     DontDestroyOnLoad(Instance);
    // // }

    public InterfaceManager()
    {
        interfacesDict = new Dictionary<string, IBase>();
    }
    #endregion

    public void RegisterInterface<T>(IBase interfaceInst) where T : IBase
    {
        string interfaceType = typeof(T).ToString();

        if (!interfacesDict.ContainsKey(interfaceType))
        {
            interfacesDict.Add(interfaceType, interfaceInst);
        }
    }

    public T GetInterfaceInstance<T>() where T : IBase
    {
        string interfaceType = typeof(T).ToString();

        if (interfacesDict.ContainsKey(interfaceType))
            return (T)interfacesDict[interfaceType];

        return default;
    }
}
