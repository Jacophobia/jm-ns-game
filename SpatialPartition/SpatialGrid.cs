// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Collision.Interfaces;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Interfaces;

namespace SpatialPartition;

public class SpatialGrid<T> : ISpatialPartition<T>, IDisposable where T : ICollidable
{
    private readonly List<T> _elements;
    private readonly ObjectPool<HashSet<Vector2>> _hashSetPool;
    private double _averageHeight;
    private double _averageWidth;
    private Dictionary<Vector2, HashSet<T>> _partitions;
    private int _partitionSizeX;
    private int _partitionSizeY;

    public SpatialGrid()
    {
        _elements = new List<T>();
        _hashSetPool = new ObjectPool<HashSet<Vector2>>();
        _partitionSizeX = 0;
        _partitionSizeY = 0;

        #if DEBUG
        _totalRuntimeStopwatch.Start();
        #endif
    }

    public SpatialGrid(IEnumerable<T> elements) : this()
    {
        Add(elements);
    }

    private Dictionary<Vector2, HashSet<T>> Partitions => _partitions ??= new Dictionary<Vector2, HashSet<T>>();

    public void Dispose()
    {
        #if DEBUG
        _totalRuntimeStopwatch.Stop();
        var totalSeconds = _totalRuntimeStopwatch.Elapsed.TotalSeconds;
        if (!(totalSeconds > 0)) return;
        var updatesPerSecond = _updateCallCount / totalSeconds;
        Debug.WriteLine($"Average Updates per Second: {updatesPerSecond}");
        Console.WriteLine($"Average Updates per Second: {updatesPerSecond}");
        #endif
    }

    public int Count => Partitions.Values.Sum(partition => partition.Count);

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        _elements.Add(item);
        UpdateAverages(item);
        CheckAndOptimize();

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
        if (!_elements.Remove(item))
            // Item not found, no need to update averages or remove from partitions
            return false;

        // Update averages
        if (_elements.Count > 0)
        {
            Debug.Assert(item != null, nameof(item) + " should not be null");
            _averageWidth = (_averageWidth * (_elements.Count + 1) - item.Destination.Width) / _elements.Count;
            _averageHeight = (_averageHeight * (_elements.Count + 1) - item.Destination.Height) / _elements.Count;
        }
        else
        {
            _averageWidth = 0;
            _averageHeight = 0;
        }

        // Check and optimize if necessary
        CheckAndOptimize();

        // Remove item from partitions
        var indices = _hashSetPool.Get();
        GetPartitionIndices(item, indices);
        foreach (var partitionIndex in indices)
            if (Partitions.TryGetValue(partitionIndex, out var partition))
                partition.Remove(item);
        _hashSetPool.Return(indices);

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
        return GetEnumerator();
    }

    public void Update(GameTime gameTime, Controls controls)
    {
        foreach (var element in _elements)
        {
            var previousIndices = _hashSetPool.Get();
            var currentIndices = _hashSetPool.Get();

            GetPartitionIndices(element, previousIndices);

            element.Update(gameTime, controls);

            GetPartitionIndices(element, currentIndices);

            HandlePartitionTransitions(element, previousIndices, currentIndices);

            CheckForCollisions(element, gameTime, currentIndices);

            _hashSetPool.Return(previousIndices);
            _hashSetPool.Return(currentIndices);
        }

        #if DEBUG
        _updateCallCount++;
        #endif
    }

    public void Draw(Renderer renderer, Camera camera, GameTime gameTime)
    {
        var indices = _hashSetPool.Get();

        foreach (var element in _elements)
            element.Draw(renderer, camera);

        _hashSetPool.Return(indices);
    }

    public void Add(IEnumerable<T> items)
    {
        var itemList = items.ToList();
        if (!itemList.Any()) return;

        // Calculate new averages with the batch of items
        var totalWidth = _averageWidth * _elements.Count + itemList.Sum(e => e.Destination.Width);
        var totalHeight = _averageHeight * _elements.Count + itemList.Sum(e => e.Destination.Height);
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

    private void CheckForCollisions(T element, GameTime gameTime, HashSet<Vector2> indices)
    {
        foreach (var index in indices)
            if (Partitions.TryGetValue(index, out var partition))
            {
                foreach (var other in partition)
                    if (!element.Equals(other) && element.Depth == other.Depth &&
                        element.CollidesWith(other, out var location, out var overlap))
                    {
                        var beforeIndices = _hashSetPool.Get();
                        var afterIndices = _hashSetPool.Get();

                        GetPartitionIndices(other, beforeIndices);

                        element.HandleCollisionWith(other, gameTime, location, overlap);

                        GetPartitionIndices(other, afterIndices);

                        HandlePartitionTransitions(other, beforeIndices, afterIndices);

                        _hashSetPool.Return(beforeIndices);
                        _hashSetPool.Return(afterIndices);
                    }
            }
            else
            {
                Debug.WriteLine($"Index: {index} has yet to be created");
                // Debug.Assert(false,
                //     $"The partition {index} that was checked does not " +
                //     "exist. There is likely an issue with " +
                //     "HandlePartitionTransitions");
            }
    }

    private void HandlePartitionTransitions(T item, HashSet<Vector2> previousIndices, HashSet<Vector2> currentIndices)
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

    private void AddIndices(Rectangle rectangle, ISet<Vector2> indices)
    {
        indices.Clear();
        var minX = (int)MathF.Round((rectangle.Center.X - rectangle.Width / 2f) / (_partitionSizeX * 1f));
        var maxX = (int)MathF.Round((rectangle.Center.X + rectangle.Width / 2f) / (_partitionSizeX * 1f));
        var minY = (int)MathF.Round((rectangle.Center.Y - rectangle.Height / 2f) / (_partitionSizeY * 1f));
        var maxY = (int)MathF.Round((rectangle.Center.Y + rectangle.Height / 2f) / (_partitionSizeY * 1f));

        for (var x = minX; x <= maxX; x++)
        for (var y = minY; y <= maxY; y++)
            indices.Add(new Vector2(x, y));
    }

    private void GetPartitionIndices(T item, ISet<Vector2> indices)
    {
        AddIndices(item.Destination, indices);
    }

    private void UpdateAverages(T item)
    {
        _averageWidth = (_averageWidth * (_elements.Count - 1) + item.Destination.Width) / _elements.Count;
        _averageHeight = (_averageHeight * (_elements.Count - 1) + item.Destination.Height) / _elements.Count;
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
            var indices = _hashSetPool.Get();
            GetPartitionIndices(element, indices);
            foreach (var index in indices)
            {
                if (!Partitions.TryGetValue(index, out var partition))
                {
                    partition = new HashSet<T>();
                    Partitions[index] = partition;
                }

                partition.Add(element);
            }

            _hashSetPool.Return(indices);
        }
    }

    private class ObjectPool<TPooled> where TPooled : new()
    {
        private readonly Stack<TPooled> _items = new();

        public TPooled Get()
        {
            return _items.Count > 0 ? _items.Pop() : new TPooled();
        }

        public void Return(TPooled item)
        {
            _items.Push(item);
        }
    }

    #if DEBUG
    private readonly Stopwatch _totalRuntimeStopwatch = new();
    private int _updateCallCount;
    #endif
}