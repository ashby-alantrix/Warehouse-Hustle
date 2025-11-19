
using UnityEngine;

public interface IStoreData
{
    string GetLevelsJson();    
}

public abstract class StoreDataBase : MonoBehaviour, IStoreData
{
    public abstract string GetLevelsJson();
}
