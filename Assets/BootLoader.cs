using System.Collections.Generic;
using DG.Tweening;
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
        DG.Tweening.DOTween.useSafeMode = false;
        DG.Tweening.DOTween.logBehaviour = LogBehaviour.ErrorsOnly;
        
        InterfaceManager.InitInstance();

        foreach (GameObject bootloader in baseObjects)
            bootloader.GetComponent<IBootLoader>().Initialize();
    }
}
