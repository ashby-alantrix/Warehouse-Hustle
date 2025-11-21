using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBootLoader : BootLoader
{
    private GameObject[] baseObjects;

    protected override void InitBootLoaders()
    {
        IBootLoader bootLoader = null;
        foreach (Transform loader in MainSingleton.Instance.transform)
        {
            if (GetLoader<IBootLoader>(loader, out bootLoader))
            {
                bootLoader.Initialize();
            }
        }
    }
    
    private bool GetLoader<T>(Transform loader, out T outLoader)
    {
        outLoader = loader.GetComponent<T>();
        return outLoader != null;
    }

    protected override void Start()
    {
        base.Start();

    }

    protected override void InitializeDataLoaders()
    {
        IDataLoader dataLoader = null;
        foreach (Transform loader in MainSingleton.Instance.transform)
        {
            if (GetLoader<IDataLoader>(loader, out dataLoader))
            {
                dataLoader.InitializeData();
            }
        }
    }
}
