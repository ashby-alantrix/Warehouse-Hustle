using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swapper : MonoBehaviour
{
    [SerializeField] private GoodsHandler goodsHandler;

    private LevelManager levelManager;

    public void SetLevelManager()
    {
        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
    }

    public void OnMouseDown()
    {
        SetLevelManager();
        if (!levelManager.CanPlayLevel) return;

        goodsHandler.SwapInputPlatformsData();
    }
}
