using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsManager : MonoBehaviour, IBootLoader, IBase 
{
    private GoodsHandler m_GoodsHandler;

    public GoodsHandler GoodsHandler => m_GoodsHandler;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsManager>(this);
    }
}
