using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager
{
    private Dictionary<string, IBase> interfacesDict = new Dictionary<string, IBase>();

    #region Singleton
    public static InterfaceManager Instance { get; private set; }

    public InterfaceManager()
    {
        interfacesDict = new Dictionary<string, IBase>();
        if (Instance == null)
        {
            Instance = new InterfaceManager();
            return;
        }
    }
    #endregion

    public void RegisterInterface<T>(IBase interfaceInst) where T : IBase
    {
        string interfaceType = typeof(T).ToString();

        if (!interfacesDict.ContainsKey(interfaceType))
            interfacesDict.Add(interfaceType, interfaceInst);
    }

    public IBase GetInterfaceInstance<T>() where T : IBase
    {
        string interfaceType = typeof(T).ToString();

        if (interfacesDict.ContainsKey(interfaceType))
            return interfacesDict[interfaceType];

        return null;
    }
}
