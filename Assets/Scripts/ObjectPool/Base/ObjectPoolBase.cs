using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPoolBase<T> : MonoBehaviour where T : ItemBase
{
    [SerializeField] protected T prefabInst;
    [SerializeField] protected int initialPoolCount;

    protected Queue<T> queue = new Queue<T>();

    public abstract ItemType GetPoolType();
    public abstract void InitPoolFirstTime();

    public T EnqueueNewInst()
    {
        T inst = Instantiate(prefabInst);
        inst.gameObject.SetActive(false);
        Enqueue(inst);

        return inst;
    }

    public virtual void Enqueue(T item)
    {
        queue.Enqueue(item);
    }

    public virtual T Dequeue()
    {
        return queue.Dequeue();
    }

    public bool IsEmpty()
    {
        return queue.Count < 1;
    }

    public virtual void ClearPool()
    {
        queue.Clear();
    }
}
