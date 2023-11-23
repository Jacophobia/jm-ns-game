using System;
using System.Collections.Generic;

namespace MonoGame.DataStructures;

internal class TimePriorityQueue<T>
{
    private readonly MinHeap<TimestampedItem> _queue;

    internal TimePriorityQueue()
    {
        _queue = new MinHeap<TimestampedItem>();
    }

    internal IEnumerable<T> Get(long currentTime)
    {
        bool done;
        lock (_queue)
        {
            done = _queue.Count > 0 && _queue.Peek().Time <= currentTime;
        }
        
        while (!done)
        {
            lock (_queue)
            {
                yield return _queue.Get().Item;
                done = _queue.Count > 0 && _queue.Peek().Time <= currentTime;
            }
        }
    }

    internal void Put(T value, long timeSent)
    {
        lock (_queue)
        {
            _queue.Put(new TimestampedItem(value, timeSent));
        }
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