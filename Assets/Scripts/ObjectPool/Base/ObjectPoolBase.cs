using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPoolBase : MonoBehaviour
{
    public abstract bool IsEmpty();
    public abstract ObjectBase CreateNewPooledItem();
    public abstract void Enqueue(ObjectBase item);
    public abstract ObjectBase Dequeue();
}

public class ObjectPoolBase<T> : ObjectPoolBase where T : ObjectBase
{
    [SerializeField] protected T prefabInst;
    [SerializeField] protected Transform prefabsParent;
    [SerializeField] protected int initialPoolCount;

    protected Queue<T> queue = new Queue<T>();

    public virtual void InitPoolFirstTime()
    {
        T itemInst = null;
        for (int i = 0; i < initialPoolCount; i++)
        {
            itemInst = (T)CreateNewPooledItem();
            Enqueue(itemInst);
        }
    }

    public override ObjectBase CreateNewPooledItem()
    {
        T inst = Instantiate(prefabInst);
        inst.gameObject.SetActive(false); // enqueue these objects later
        inst.transform.parent = prefabsParent;
        
        return inst;
    }

    public override void Enqueue(ObjectBase item)
    {
        queue.Enqueue((T)item);
    }

    public override ObjectBase Dequeue()
    {
        return queue.Dequeue();
    }

    // public T CreateNewPoolItem()
    // {
    //     T inst = Instantiate(prefabInst);
    //     inst.gameObject.SetActive(false); // enqueue these objects later
    //     inst.transform.parent = prefabsParent;
        
    //     // Enqueue(inst);

    //     return inst;
    // }

    // public virtual void Enqueue(T item)
    // {
    //     queue.Enqueue(item);
    // }

    // public virtual T Dequeue()
    // {
    //     return queue.Dequeue();
    // }

    public override bool IsEmpty()
    {
        return queue.Count < 1;
    }

    public virtual void ClearPool()
    {
        queue.Clear();
    }

    
}
