using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalBootLoader : BootLoader
{
    [SerializeField] private GameObject[] baseLoaders;

    protected override void Start()
    {
        base.Start();

        
    }

    protected override void InitBootLoaders()
    {
        // Initialize any global boot loaders here
        IBootLoader bootLoader = null;

        foreach (GameObject loader in baseLoaders)
        {
            if (GetLoader<IBootLoader>(loader.transform, out bootLoader))
            {
                bootLoader.Initialize();
            }
        }
    }

    protected override void InitializeData()
    {
        // Initialize any data loaders here
    }
}
