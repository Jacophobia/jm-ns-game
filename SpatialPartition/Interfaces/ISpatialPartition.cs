using System;
using System.Collections.Generic;
using SpatialPartition.Collision;

namespace SpatialPartition.Interfaces;

public interface ISpatialPartition<T> : IEnumerable<T>, ICollection<T> where T : ICollidable, IComparable<T>
{
    
}