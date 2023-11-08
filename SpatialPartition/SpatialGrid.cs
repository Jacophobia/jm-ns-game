using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Tuple;
using SpatialPartition.Collision;
using SpatialPartition.Interfaces;

namespace SpatialPartition;

public class SpatialGrid<T> : ISpatialPartition<T> where T : ICollidable, IComparable<T>
{
    private readonly Dictionary<Tuple<int, int>, HashSet<T>> _partitions;

    public SpatialGrid(int partitionCountX, int partitionCountY)
    {
        if (partitionCountX <= 0 || partitionCountY <= 0)
        {
            throw new ArgumentException("Partition counts must be greater than zero.");
        }

        PartitionCountX = partitionCountX;
        PartitionCountY = partitionCountY;
        _partitions = new Dictionary<Tuple<int, int>, HashSet<T>>();
    }

    private int PartitionCountX { get; }
    private int PartitionCountY { get; }

    public int Count => _partitions.Values.Sum(partition => partition.Count);

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        foreach (var partitionIndex in GetPartitionIndices(item))
        {
            if (!_partitions.TryGetValue(partitionIndex, out var partition))
            {
                partition = new HashSet<T>();
                _partitions[partitionIndex] = partition;
            }

            partition.Add(item);
        }
    }

    public bool Remove(T item)
    {
        bool removed = false;
        foreach (var partitionIndex in GetPartitionIndices(item))
        {
            if (_partitions.TryGetValue(partitionIndex, out var partition))
            {
                removed |= partition.Remove(item);
            }
        }

        return removed;
    }

    public void Clear()
    {
        _partitions.Clear();
    }

    public bool Contains(T item)
    {
        foreach (var partitionIndex in GetPartitionIndices(item))
        {
            if (_partitions.TryGetValue(partitionIndex, out var partition) && partition.Contains(item))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var partition in _partitions.Values)
        {
            foreach (var item in partition)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _partitions.Values.SelectMany(partition => partition).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Update()
    {
        foreach (var partition in _partitions.Values)
        {
            foreach (var item in partition.ToList())
            {
                var previousPartitionIndices = GetPartitionIndices(item).ToSet();

                // Update the item (e.g., move it)
                item.Update();

                var currentPartitionIndices = GetPartitionIndices(item).ToSet();

                // Handle transitions between partitions
                HandlePartitionTransitions(item, previousPartitionIndices, currentPartitionIndices);

                // Check for collisions in the current partition
                foreach (var other in partition)
                {
                    if (!item.Equals(other) && item.CollidesWith(other))
                    {
                        // Handle collision, e.g., call a method or trigger an event.
                    }
                }
            }
        }
    }

    private void HandlePartitionTransitions(T item, ISet<Tuple<int, int>> previousIndices, ISet<Tuple<int, int>> currentIndices)
    {
        // Calculate the intersection between previous and current indices
        var intersection = previousIndices.Intersect(currentIndices).ToArray();
        
        // Calculate the unused indices (previous - intersection)
        previousIndices.ExceptWith(intersection);
    
        // Remove all unused indices
        foreach (var indexToRemove in previousIndices)
        {
            _partitions[indexToRemove].Remove(item);
        }
    
        // Calculate the new indices (current - intersection)
        currentIndices.ExceptWith(intersection);
    
        // Add the item to the new indices
        foreach (var currentIndex in currentIndices)
        {
            if (!_partitions.TryGetValue(currentIndex, out var partition))
            {
                partition = new HashSet<T>();
                _partitions[currentIndex] = partition;
            }
    
            partition.Add(item);
        }
    }

    private IEnumerable<Tuple<int, int>> GetPartitionIndices(T item)
    {
        var minX = (int)((item.Position.X - item.Width / 2) / (PartitionCountX * 1.0));
        var maxX = (int)((item.Position.X + item.Width / 2) / (PartitionCountX * 1.0));
        var minY = (int)((item.Position.Y - item.Height / 2) / (PartitionCountY * 1.0));
        var maxY = (int)((item.Position.Y + item.Height / 2) / (PartitionCountY * 1.0));

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                yield return new Tuple<int, int>(x, y);
            }
        }
    }
}

