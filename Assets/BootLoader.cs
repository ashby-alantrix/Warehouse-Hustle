using System.Collections.Generic;
using UnityEngine;

public interface IBootLoader
{
    public void Initialize();
}

public class BootLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] baseObjects;

    private void Start()
    {
        InterfaceManager.InitInstance();

        foreach (GameObject bootloader in baseObjects)
            bootloader.GetComponent<IBootLoader>().Initialize();
    }
}
