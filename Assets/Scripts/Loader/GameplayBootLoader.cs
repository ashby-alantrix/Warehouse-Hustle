using UnityEngine;

public class GameplayBootLoader : BootLoader
{
    [SerializeField] private GameObject[] baseObjects;
    [SerializeField] private BaseSO[] scriptables;

    private bool hasInitializedScriptables = false;

    protected override void InitBootLoaders()
    {
        IBootLoader bootLoader = null;

        foreach (GameObject loader in baseObjects)
        {
            if (GetLoader<IBootLoader>(loader.transform, out bootLoader))
            {
                bootLoader.Initialize();
            }
        }
    }

    protected override void InitializeData()
    {
        InitializeScriptablesData();

        IDataLoader dataLoader = null;
        foreach (GameObject loader in baseObjects)
        {
            if (GetLoader<IDataLoader>(loader.transform, out dataLoader))
            {
                dataLoader.InitializeData();
            }
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    private void InitializeScriptablesData()
    {
        if (!hasInitializedScriptables)
        {
            foreach (BaseSO scriptableObject in scriptables)
            {
                scriptableObject.InitData();
            }

            hasInitializedScriptables = true;
        }
    }
}
