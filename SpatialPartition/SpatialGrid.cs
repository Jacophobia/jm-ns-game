using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SpatialPartition.Collision;
using SpatialPartition.Interfaces;

namespace SpatialPartition;

public class SpatialGrid<T> : ISpatialPartition<T> where T : ICollidable
{
    private readonly Dictionary<Vector2, HashSet<T>> _partitions;
    private readonly List<T> _elements;

    public SpatialGrid(int partitionCountX, int partitionCountY)
    {
        if (partitionCountX <= 0 || partitionCountY <= 0)
        {
            throw new ArgumentException("Partition counts must be greater than zero.");
        }

        PartitionCountX = partitionCountX;
        PartitionCountY = partitionCountY;
        _partitions = new Dictionary<Vector2, HashSet<T>>();
        _elements = new List<T>();
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
        
        _elements.Add(item);
    }

    public bool Remove(T item)
    {
        var removed = false;
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

    public void Update()
    {
        foreach (var element in _elements)
        {
            var previousPartitionIndices = GetPartitionIndices(element);
            
            // Update the element (e.g., move it)
            element.Update();

            var partitionIndices = GetPartitionIndices(element);

            // Handle transitions between partitions
            HandlePartitionTransitions(element, previousPartitionIndices, partitionIndices);
            
            // Check for collisions in the current partition
            foreach (var index in partitionIndices)
            {
                foreach (var other in _partitions[index])
                {
                    if (!element.Equals(other) && element.CollidesWith(other, out var location))
                    {
                        element.HandleCollisionWith(other, location);
                    }
                }
                
            }
        }
    }

    private void HandlePartitionTransitions(T item, ISet<Vector2> previousIndices, ISet<Vector2> currentIndices)
    {
        // Calculate the intersection between previous and current indices
        var intersection = previousIndices.Intersect(currentIndices).ToHashSet();
    
        // Remove all unused indices
        foreach (var indexToRemove in previousIndices.Where(index => !intersection.Contains(index)))
        {
            _partitions[indexToRemove].Remove(item);
        }
    
        // Add the item to the new indices
        foreach (var currentIndex in currentIndices.Where(index => !intersection.Contains(index)))
        {
            if (!_partitions.TryGetValue(currentIndex, out var partition))
            {
                partition = new HashSet<T>();
                _partitions[currentIndex] = partition;
            }
    
            partition.Add(item);
        }
    }

    private ISet<Vector2> GetPartitionIndices(T item)
    {
        var minX = (int)((item.Position.X - item.Width / 2) / (PartitionCountX * 1f));
        var maxX = (int)((item.Position.X + item.Width / 2) / (PartitionCountX * 1f));
        var minY = (int)((item.Position.Y - item.Height / 2) / (PartitionCountY * 1f));
        var maxY = (int)((item.Position.Y + item.Height / 2) / (PartitionCountY * 1f));

        var indices = new HashSet<Vector2>();
        
        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                indices.Add(new Vector2(x, y));
            }
        }

        return indices;
    }
}

