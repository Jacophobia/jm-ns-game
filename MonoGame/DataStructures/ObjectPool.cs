using System.Collections.Generic;

namespace MonoGame.DataStructures;

public class ObjectPool<TPooled> where TPooled : new()
{
    private readonly Stack<TPooled> _items = new();

    internal TPooled Get()
    {
        return _items.Count > 0 ? _items.Pop() : new TPooled();
    }

    internal void Return(TPooled item)
    {
        _items.Push(item);
    }
}