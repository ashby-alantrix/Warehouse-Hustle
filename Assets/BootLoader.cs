using UnityEngine;

public interface IBootLoader
{
    public void Initialize();
}

public class BootLoader : MonoBehaviour
{
    [SerializeField] private GoodsManager goodsManager;
    [SerializeField] private ObjectPoolManager objectPoolManager;

    private void Start()
    {
        InterfaceManager.InitInstance();
        objectPoolManager.Initialize();
        goodsManager.Initialize();

    }
}
