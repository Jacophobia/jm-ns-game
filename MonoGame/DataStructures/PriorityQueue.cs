using System;
using System.Collections.Generic;
using System.Threading;

namespace MonoGame.DataStructures;

internal class PriorityQueue<T>
{
    private readonly MinHeap<TimestampedItem> _queue;
    private readonly SemaphoreSlim _semaphore;

    internal PriorityQueue()
    {
        _queue = new MinHeap<TimestampedItem>();
        _semaphore = new SemaphoreSlim(0);
    }

    internal bool IsEmpty => _queue.IsEmpty;

    internal T Get()
    {
        _semaphore.Wait();
        return _queue.Get().Item;
    }

    internal IEnumerable<T> GetAll()
    {
        while (!_queue.IsEmpty)
        {
            _semaphore.Wait();
            yield return _queue.Get().Item;
        }
    }

    internal void Put(T value, long timeSent)
    {
        _queue.Put(new TimestampedItem(value, timeSent));
        _semaphore.Release();
    }

    private readonly struct TimestampedItem : IComparable<TimestampedItem>
    {
        public T Item { get; }
        public long Time { get; }

        public TimestampedItem(T item, long time)
        {
            Item = item;
            Time = time;
        }

        public int CompareTo(TimestampedItem other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}