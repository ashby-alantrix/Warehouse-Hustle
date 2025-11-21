using UnityEngine;

public class GameplayBootLoader : BootLoader
{
    [SerializeField] private GameObject[] baseObjects;

    protected override void InitBootLoaders()
    {
        foreach (GameObject bootloader in baseObjects)
        {
            bootloader.GetComponent<IBootLoader>().Initialize();
        }
    }

    protected override void InitializeDataLoaders()
    {
        IDataLoader dataLoader = null;
        foreach (GameObject bootloader in baseObjects)
        {
            dataLoader = bootloader.GetComponent<IDataLoader>();
            if (dataLoader != null)
                dataLoader.InitializeData();
        }
    }

    protected override void Start()
    {
        base.Start();
    }
}
