using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpatialPartition.Collision;

namespace SpatialPartition.Interfaces;

public interface ISpatialPartition<T> : ICollection<T> where T : ICollidable
{
    public void Update(GameTime gameTime);
}