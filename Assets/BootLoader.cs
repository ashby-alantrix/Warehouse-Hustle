using UnityEngine;

public interface IBootLoader
{
    public void Initialize();
}

public class BootLoader : MonoBehaviour
{
    [SerializeField] private GoodsManager goodsManager;

    private void Start()
    {
        InterfaceManager.InitInstance();
        goodsManager.Initialize();
    }
}
