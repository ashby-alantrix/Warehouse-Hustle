using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swapper : MonoBehaviour
{
    [SerializeField] private GoodsHandler goodsHandler;

    private LevelManager levelManager;
    private InputManager inputManager;

    private void SetLevelManager()
    {
        Debug.Log($"Swapper being called");
        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
    }

    public void OnClick_Swap()
    {
        SetLevelManager();
        inputManager = inputManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputManager>() : inputManager;

        if (!levelManager.CanPlayLevel || !inputManager.IsInputEnabled) return;

        goodsHandler.SwapInputPlatformsData();
    }
}
