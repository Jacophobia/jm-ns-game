using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Resources;

namespace Shared.DataStructures;

public static class EnumerableExtensions
{
    public static Deque<T> ToDeque<T>(this IEnumerable<T> enumerable)
    {
        return new Deque<T>(enumerable);
    }
}

public class Deque<T> : IEnumerable<T>
{
    private T[] _array;
    private int _first;
    private int _last;
    private int _count;

    // Constructors
    public Deque() : this(16) // Default capacity
    {
    }

    public Deque(int capacity)
    {
        _array = new T[capacity];
        _first = 0;
        _last = -1; 
        _count = 0;
    }

    public Deque(IEnumerable<T> collection)
    {
        _array = collection.ToArray();
        _first = 0;
        _last = _count - 1;
        _count = _array.Length;
    }

    // Properties
    public int Count => _count;
    public bool Empty => _count == 0;
    public T Front
    {
        get => _array[_first];
        set => _array[_first] = value;
    }
    public T Back
    {
        get => _array[_last];
        set => _array[_last] = value;
    }

    // Indexer (Read-write random access)
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException();
            }
            return _array[(_first + index) % _array.Length];
        }
        set
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException();
            }
            _array[(_first + index) % _array.Length] = value;
        }
    }

    // Methods
    public void PushFront(T item)
    {
        _first = (_first - 1 + _array.Length) % _array.Length;
        _array[_first] = item;
        _count++;
        if (_count == _array.Length)
        {
            Expand();
        }
    }

    public void PushBack(T item)
    {
        _last = (_last + 1) % _array.Length;
        _array[_last] = item;
        _count++;
        if (_count == _array.Length)
        {
            Expand();
        }
    }

    public T PopFront()
    {
        if (Empty)
        {
            throw new InvalidOperationException("Deque is empty.");
        }

        var value = _array[_first];
        _array[_first] = default; // Clear the element
        _first = (_first + 1) % _array.Length;
        _count--;
        return value;
    }

    public T PopBack()
    {
        if (Empty)
        {
            throw new InvalidOperationException("Deque is empty.");
        }

        var value = _array[_last];
        _array[_last] = default; // Clear the element
        _last = (_last - 1 + _array.Length) % _array.Length;
        _count--;
        return value;
    }

    public void Clear()
    {
        Array.Clear(_array, 0, _array.Length); 
        _first = 0;
        _last = -1;
        _count = 0;
    }

    // void deque<T>::swap(deque& rhs); // Swap member function
    public void Swap(Deque<T> rhs)
    {
        Swap(this, rhs);
    }
    
    // void swap(deque<T>& lhs, deque<T>& rhs); // Swap stand-alone function
    public static void Swap(Deque<T> lhs, Deque<T> rhs)
    {
        UsefulOperations.Swap(ref lhs._array, ref rhs._array);
        UsefulOperations.Swap(ref lhs._first, ref rhs._first);
        UsefulOperations.Swap(ref lhs._last, ref rhs._last);
    }
        
    private void Expand()
    {
        var newCapacity = _array.Length * 2;
        var newArray = new T[newCapacity];
        for (var i = 0; i < _count; i++)
        {
            newArray[i] = _array[(_first + i) % _array.Length];
        }
        _array = newArray;
        _first = 0;
        _last = _count - 1;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new DequeEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    private class DequeEnumerator : IEnumerator<T>
    {
        private readonly Deque<T> _deque;
        private int _currentIndex;

        public DequeEnumerator(Deque<T> deque)
        {
            _deque = deque;
            _currentIndex = -1; // Start before the first element
        }

        public T Current => _deque[_currentIndex];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // nothing to dispose
        }

        public bool MoveNext()
        {
            _currentIndex++;
            return _currentIndex < _deque.Count;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }
    }
}