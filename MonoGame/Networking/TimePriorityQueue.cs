using System;
using System.Collections.Generic;

namespace MonoGame.Networking;

internal class MinHeap<T> where T : IComparable<T>
{
    private readonly List<T> _elements;

    public MinHeap()
    {
        _elements = new List<T>();
    }

    public int Count => _elements.Count;

    public void Put(T item)
    {
        _elements.Add(item);
        HeapifyUp(_elements.Count - 1);
    }

    public T Get()
    {
        if (_elements.Count == 0)
            throw new InvalidOperationException("Heap is empty");

        var result = _elements[0];
        _elements[0] = _elements[^1];
        _elements.RemoveAt(_elements.Count - 1);
        HeapifyDown(0);

        return result;
    }
    
    public T Peek()
    {
        if (_elements.Count == 0)
            throw new InvalidOperationException("Heap is empty");

        return _elements[0];
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            var parentIndex = (index - 1) / 2;
            if (_elements[index].CompareTo(_elements[parentIndex]) >= 0)
                break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        while (true)
        {
            var smallest = index;
            var leftChildIndex = 2 * index + 1;
            var rightChildIndex = 2 * index + 2;

            if (leftChildIndex < _elements.Count && _elements[leftChildIndex].CompareTo(_elements[smallest]) < 0)
                smallest = leftChildIndex;

            if (rightChildIndex < _elements.Count && _elements[rightChildIndex].CompareTo(_elements[smallest]) < 0)
                smallest = rightChildIndex;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int first, int second)
    {
        (_elements[first], _elements[second]) = (_elements[second], _elements[first]);
    }
}
    
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