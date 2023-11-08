using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SpatialPartition.Collision;
using SpatialPartition.Interfaces;

namespace SpatialPartition;

public class SpatialGrid<T> : ISpatialPartition<T> where T : ICollidable, IComparable<T>
{
    private readonly Dictionary<int, List<T>> _partitions;

    public SpatialGrid(int partitionCountX, int partitionCountY)
    {
        if (partitionCountX <= 0 || partitionCountY <= 0)
        {
            throw new ArgumentException("Partition counts must be greater than zero.");
        }

        PartitionCountX = partitionCountX;
        PartitionCountY = partitionCountY;
        _partitions = new Dictionary<int, List<T>>();
    }

    private int PartitionCountX { get; }
    private int PartitionCountY { get; }

    public int Count => _partitions.Values.Sum(list => list.Count);

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        var partitionIndex = GetPartitionIndex(item);
        if (!_partitions.ContainsKey(partitionIndex))
        {
            _partitions[partitionIndex] = new List<T>();
        }

        _partitions[partitionIndex].Add(item);
    }

    public bool Remove(T item)
    {
        var partitionIndex = GetPartitionIndex(item);
        if (!_partitions.TryGetValue(partitionIndex, out var partition))
        {
            return false;
        }
        
        partition.Remove(item);
        return true;

    }

    public void Clear()
    {
        _partitions.Clear();
    }

    public bool Contains(T item)
    {
        var partitionIndex = GetPartitionIndex(item);
        return _partitions.TryGetValue(partitionIndex, out var partition) && partition.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var partition in _partitions.Values)
        {
            partition.CopyTo(array, arrayIndex);
            arrayIndex += partition.Count;
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

    public void CheckCollisions()
    {
        foreach (var partition in _partitions.Values)
        {
            for (var i = 0; i < partition.Count; i++)
            {
                for (var j = i + 1; j < partition.Count; j++)
                {
                    if (partition[i].CollidesWith(partition[j], out var location))
                    {
                        // Handle collision, e.g., call a method or trigger an event.
                    }
                }
            }
        }
    }

    private int GetPartitionIndex(T item)
    {
        var x = (int)(item.Position.X / (PartitionCountX * 1.0));
        var y = (int)(item.Position.Y / (PartitionCountY * 1.0));
        return x + y * PartitionCountX;
    }
}
