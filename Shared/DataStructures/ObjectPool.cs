using System.Collections.Generic;

namespace Shared.DataStructures;

public class ObjectPool<TPooled> where TPooled : new()
{
    private readonly Stack<TPooled> _items = new();

    internal TPooled Get()
    {
        if (_items.TryPop(out var item) && item != null)
            return item;
        
        return new TPooled();
    }

    internal void Return(TPooled item)
    {
        _items.Push(item);
    }
}