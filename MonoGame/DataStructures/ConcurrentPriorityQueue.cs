using System.Collections.Generic;
using System.Threading;

namespace MonoGame.DataStructures;

public class ConcurrentPriorityQueue<T, TSort> : PriorityQueue<T, TSort>
{
    private readonly SemaphoreSlim _semaphore;

    public ConcurrentPriorityQueue(int? capacity = null)
    {
        _semaphore = capacity.HasValue 
            ? new SemaphoreSlim(0, capacity.Value) 
            : new SemaphoreSlim(0);
    }

    public new void Enqueue(T item, TSort sortKey)
    {
        base.Enqueue(item, sortKey);
        _semaphore.Release();
    }

    public new void EnqueueRange(IEnumerable<(T item, TSort sortKey)> items)
    {
        foreach (var (item, sortKey) in items)
        {
            base.Enqueue(item, sortKey);
            _semaphore.Release();
        }
    }

    public new T EnqueueDequeue(T item, TSort sortKey)
    {
        _semaphore.Wait();
        var prevItem = base.Dequeue();
        base.Enqueue(item, sortKey);
        _semaphore.Release();
        return prevItem;
    }

    public new T Dequeue()
    {
        _semaphore.Wait();
        return base.Dequeue();
    }

    public new bool TryDequeue(out T item, out TSort sortKey)
    {
        if (Count <= 0)
        {
            item = default;
            sortKey = default;
            return false;
        }

        _semaphore.Wait();
        return base.TryDequeue(out item, out sortKey);
    }
}