// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Interfaces;

namespace MonoGame.DataStructures;

public class SpatialGrid<T> : ISpatialPartition<T> where T : ICollidable, IRenderable, IUpdatable
{
    private readonly List<T> _elements;
    private readonly List<T> _staticElements;
    private readonly List<IPlayer> _players;
    private readonly ObjectPool<HashSet<PartitionKey>> _hashSetPool;
    private double _averageHeight;
    private double _averageWidth;
    private Dictionary<PartitionKey, HashSet<T>> _partitions;
    private int _partitionSizeX;
    private int _partitionSizeY;

    public SpatialGrid()
    {
        _elements = new List<T>();
        _staticElements = new List<T>();
        _players = new List<IPlayer>();
        _hashSetPool = new ObjectPool<HashSet<PartitionKey>>();
        _partitionSizeX = 0;
        _partitionSizeY = 0;
    }

    public SpatialGrid(IEnumerable<T> elements) : this()
    {
        Add(elements);
    }

    private Dictionary<PartitionKey, HashSet<T>> Partitions => _partitions ??= new Dictionary<PartitionKey, HashSet<T>>();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public int Count => Partitions.Values.Sum(partition => partition.Count);

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        if (item?.IsStatic ?? false)
        {
            _staticElements.Add(item);
        }
        else
        {
            _elements.Add(item);
            UpdateAverages(item);
            CheckAndOptimize();
        }

        var indices = _hashSetPool.Get();
        GetPartitionIndices(item, indices);
        foreach (var partitionIndex in indices)
        {
            if (!Partitions.TryGetValue(partitionIndex, out var partition))
            {
                partition = new HashSet<T>();
                Partitions[partitionIndex] = partition;
            }

            partition.Add(item);
        }

        _hashSetPool.Return(indices);
    }

    public bool Remove(T item)
    {
        Debug.Assert(item != null, nameof(item) + " should not be null");
        
        if (item.IsStatic && !_staticElements.Remove(item))
            // Item not found, no need to update averages or remove from partitions
            return false;
        
        if (!_elements.Remove(item))
            // Item not found, no need to update averages or remove from partitions
            return false;
        
        // Remove item from partitions
        var indices = _hashSetPool.Get();
        GetPartitionIndices(item, indices);
        foreach (var partitionIndex in indices)
            if (Partitions.TryGetValue(partitionIndex, out var partition))
                partition.Remove(item);
        _hashSetPool.Return(indices);
        
        if (item.IsStatic)
            return true;

        // Update averages
        if (_elements.Count > 0)
        {
            _averageWidth = (_averageWidth * (_elements.Count + 1) - item.Bounds.Width) / _elements.Count;
            _averageHeight = (_averageHeight * (_elements.Count + 1) - item.Bounds.Height) / _elements.Count;
        }
        else
        {
            _averageWidth = 0;
            _averageHeight = 0;
        }

        // Check and optimize if necessary
        CheckAndOptimize();

        return true;
    }


    public void Clear()
    {
        _partitions?.Clear();
    }

    public bool Contains(T item)
    {
        return _elements.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var element in _elements)
        {
            array[arrayIndex] = element;
            arrayIndex++;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _elements.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _elements.GetEnumerator();
    }

    private void UpdateElement(T element, float deltaTime)
    {
        var previousIndices = _hashSetPool.Get();
        var currentIndices = _hashSetPool.Get();

        GetPartitionIndices(element, previousIndices);

        element.Update(deltaTime);

        GetPartitionIndices(element, currentIndices);

        CheckForCollisions(element, deltaTime, currentIndices);
            
        GetPartitionIndices(element, currentIndices);

        HandlePartitionTransitions(element, previousIndices, currentIndices);

        _hashSetPool.Return(previousIndices);
        _hashSetPool.Return(currentIndices);
    }

    public void Update(float deltaTime)
    {
        foreach (var player in _players)
        {
            player.Update(deltaTime);
        }
        
        foreach (var element in _elements)
        {
            UpdateElement(element, deltaTime);
        }
    }

    public void Draw(float deltaTime)
    {
        foreach (var player in _players)
            player.BeginDisplay();
        foreach (var element in _elements)
        foreach (var player in _players)
            element.Render(player);
        foreach (var element in _staticElements)
        foreach (var player in _players)
            element.Render(player);
        foreach (var player in _players)
            player.EndDisplay();
    }

    public void Add(IPlayer player)
    {
        _players.Add(player);
    }

    public void Remove(IPlayer player)
    {
        _players.Remove(player);
    }

    public void Add(IEnumerable<T> items)
    {
        var itemList = items.ToList();
        if (!itemList.Any()) return;

        // Calculate new averages with the batch of items
        var totalWidth = _averageWidth * _elements.Count + itemList.Sum(e => e.Bounds.Width);
        var totalHeight = _averageHeight * _elements.Count + itemList.Sum(e => e.Bounds.Height);
        var newCount = _elements.Count + itemList.Count;
        _averageWidth = totalWidth / newCount;
        _averageHeight = totalHeight / newCount;

        CheckAndOptimize();

        foreach (var item in itemList)
        {
            var indices = _hashSetPool.Get();
            GetPartitionIndices(item, indices);
            foreach (var partitionIndex in indices)
            {
                if (!Partitions.TryGetValue(partitionIndex, out var partition))
                {
                    partition = new HashSet<T>();
                    Partitions[partitionIndex] = partition;
                }

                partition.Add(item);
            }

            _hashSetPool.Return(indices);
            _elements.Add(item);
        }
    }

    private void CheckForCollisions(T element, float deltaTime, HashSet<PartitionKey> indices)
    {
        foreach (var index in indices)
            if (Partitions.TryGetValue(index, out var partition))
            {
                foreach (var other in partition)
                    if (!element.Equals(other) && element.CollidesWith(other, deltaTime, out var overlap))
                    {
                        Debug.Assert(overlap != null, $"{nameof(overlap)} should not be null");

                        var beforeIndices = _hashSetPool.Get();
                        var afterIndices = _hashSetPool.Get();

                        GetPartitionIndices(other, beforeIndices);

                        element.HandleCollisionWith(other, deltaTime, overlap.Value);

                        GetPartitionIndices(other, afterIndices);

                        HandlePartitionTransitions(other, beforeIndices, afterIndices);

                        _hashSetPool.Return(beforeIndices);
                        _hashSetPool.Return(afterIndices);
                    }
            }
    }

    private void HandlePartitionTransitions(T item, HashSet<PartitionKey> previousIndices, HashSet<PartitionKey> currentIndices)
    {
        foreach (var index in previousIndices)
            if (!currentIndices.Contains(index))
                if (Partitions.TryGetValue(index, out var partition))
                    partition.Remove(item);

        foreach (var index in currentIndices)
            if (!previousIndices.Contains(index))
            {
                if (!Partitions.TryGetValue(index, out var partition))
                {
                    partition = new HashSet<T>();
                    Partitions[index] = partition;
                }

                partition.Add(item);
            }
    }

    private void AddIndices(Rectangle rectangle, int depth, ISet<PartitionKey> indices)
    {
        indices.Clear();

        if (_partitionSizeX == 0 || _partitionSizeY == 0)
            return;
        
        var minX = rectangle.Left / _partitionSizeX;
        var maxX = (rectangle.Right + 1) / _partitionSizeX;
        var minY = rectangle.Top / _partitionSizeY;
        var maxY = (rectangle.Bottom + 1) / _partitionSizeY;

        for (var x = minX; x <= maxX; x++)
        for (var y = minY; y <= maxY; y++)
            indices.Add(new PartitionKey(x, y, depth));
    }

    private void GetPartitionIndices(T item, ISet<PartitionKey> indices)
    {
        AddIndices(item.Bounds, item.Layer, indices);
    }

    private void UpdateAverages(T item)
    {
        _averageWidth = (_averageWidth * (_elements.Count - 1) + item.Bounds.Width) / _elements.Count;
        _averageHeight = (_averageHeight * (_elements.Count - 1) + item.Bounds.Height) / _elements.Count;
    }

    private void CheckAndOptimize()
    {
        const double threshold = 0.1;
        var idealPartitionSizeX = 3 * _averageWidth;
        var idealPartitionSizeY = 3 * _averageHeight;

        if (Math.Abs(_partitionSizeX - idealPartitionSizeX) > threshold * idealPartitionSizeX ||
            Math.Abs(_partitionSizeY - idealPartitionSizeY) > threshold * idealPartitionSizeY)
            Optimize();
    }

    private void Optimize()
    {
        _partitionSizeX = (int)(3 * _averageWidth);
        _partitionSizeY = (int)(3 * _averageHeight);

        _partitions?.Clear();
        foreach (var element in _elements)
        {
            InsertItem(element);
        }

        foreach (var element in _staticElements)
        {
            InsertItem(element);
        }
    }

    private void InsertItem(T item)
    {
        var indices = _hashSetPool.Get();
        GetPartitionIndices(item, indices);
        foreach (var index in indices)
        {
            if (!Partitions.TryGetValue(index, out var partition))
            {
                partition = new HashSet<T>();
                Partitions[index] = partition;
            }

            partition.Add(item);
        }

        _hashSetPool.Return(indices);
    }
    
    public readonly struct PartitionKey : IEquatable<PartitionKey>
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _depth;
    
        public PartitionKey(int x, int y, int depth)
        {
            _x = x;
            _y = y;
            _depth = depth;
        }
    
        public override bool Equals(object obj)
        {
            return obj is PartitionKey key && Equals(key);
        }
    
        public bool Equals(PartitionKey other)
        {
            return _x == other._x && _y == other._y && _depth == other._depth;
        }
    
        public override int GetHashCode()
        {
            return HashCode.Combine(_x, _y, _depth);
        }
    
        public static bool operator ==(PartitionKey left, PartitionKey right)
        {
            return left.Equals(right);
        }
    
        public static bool operator !=(PartitionKey left, PartitionKey right)
        {
            return !(left == right);
        }
    }
}