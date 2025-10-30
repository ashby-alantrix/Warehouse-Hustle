using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolBase<T> : MonoBehaviour where T : ObjectBase
{
    protected Queue<T> queue = new Queue<T>();

    public virtual void Enqueue(T item)
    {
        queue.Enqueue(item);
    }

    public virtual T Dequeue(T item)
    {
        return queue.Dequeue();
    }

    public virtual bool IsEmpty()
    {
        return queue.Count < 1;
    }
}
