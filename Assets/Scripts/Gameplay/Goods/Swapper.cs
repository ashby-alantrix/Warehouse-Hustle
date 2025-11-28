using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swapper : MonoBehaviour
{
    [SerializeField] private GoodsHandler goodsHandler;

    private LevelManager levelManager;

    private void SetLevelManager()
    {
        Debug.Log($"Swapper being called");
        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
    }

    public void OnClick_Swap()
    {
        SetLevelManager();
        if (!levelManager.CanPlayLevel) return;

        goodsHandler.SwapInputPlatformsData();
    }
}
