using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] baseObjects;
    [SerializeField] private GameObject[] uiBases;

    private void Start()
    {
        // DG.Tweening.DOTween.useSafeMode = false;
        // DG.Tweening.DOTween.logBehaviour = LogBehaviour.ErrorsOnly;
        
        InterfaceManager.InitInstance();

        if (baseObjects != null && baseObjects.Length > 0)
            foreach (GameObject bootloader in baseObjects)
                bootloader.GetComponent<IBootLoader>().Initialize();

        if (uiBases != null && uiBases.Length > 0)
            foreach (GameObject uiBase in uiBases)
                uiBase.GetComponent<IUIBase>().Initialize();
    }
}
