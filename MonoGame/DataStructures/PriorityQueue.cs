using System;
using System.Collections.Generic;

namespace MonoGame.DataStructures;

internal class PriorityQueue<T>
{
    private readonly MinHeap<TimestampedItem> _queue;

    internal PriorityQueue()
    {
        _queue = new MinHeap<TimestampedItem>();
    }

    internal IEnumerable<T> GetAll()
    {
        while (!_queue.IsEmpty)
        {
            yield return _queue.Get().Item;
        }
    }

    internal void Put(T value, long timeSent)
    {
        _queue.Put(new TimestampedItem(value, timeSent));
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