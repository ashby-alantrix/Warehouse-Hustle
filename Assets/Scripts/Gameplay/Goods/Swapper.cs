using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swapper : MonoBehaviour
{
    [SerializeField] private GoodsHandler goodsHandler;

    public void OnMouseDown()
    {
        goodsHandler.SwapInputPlatformsData();
    }
}
