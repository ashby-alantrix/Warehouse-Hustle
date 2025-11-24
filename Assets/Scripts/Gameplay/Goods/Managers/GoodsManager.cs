using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsManager : MonoBehaviour, IBootLoader, IBase 
{
    [SerializeField] private GoodsHandler m_GoodsHandler;

    public GoodsHandler GoodsHandler => m_GoodsHandler;

    private LevelManager levelManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<GoodsManager>(this);

        levelManager = InterfaceManager.Instance.GetInterfaceInstance<LevelManager>();
        m_GoodsHandler.InitGoodsInfo(levelManager.GetCurrentLevelsInfo().availGoodTypes);
    }

    public void ClearGoodsInNodes(int count)
    {
        for (int indexI = 0; indexI < count; indexI++)
        {
            
        }
    }
}
