using System;
using System.Collections.Generic;

namespace MonoGame.Interfaces;

public interface ISpatialPartition<T> : ICollection<T>, IDisposable where T : ICollidable, IRenderable
{
    public void Update(float deltaTime);
    public void Draw(float deltaTime);
    public void Add(IPlayer player);
    public void Remove(IPlayer player);
    public void Remove(Guid playerId);
}