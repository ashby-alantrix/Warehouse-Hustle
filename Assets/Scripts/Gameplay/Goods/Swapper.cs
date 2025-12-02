using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swapper : MonoBehaviour
{
    [SerializeField] private GoodsHandler goodsHandler;

    private LevelManager levelManager;
    private InputManager inputManager;
    private PopupManager popupManager;

    private void SetLevelManager()
    {
        Debug.Log($"Swapper being called");
        levelManager = levelManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<LevelManager>() : levelManager;
    }

    public void OnClick_Swap()
    {
        SetLevelManager();
        inputManager = inputManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputManager>() : inputManager;
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;

        if (!levelManager.CanPlayLevel || popupManager.GetActivePU() != null) return;

        goodsHandler.SwapInputPlatformsData();
    }
}
