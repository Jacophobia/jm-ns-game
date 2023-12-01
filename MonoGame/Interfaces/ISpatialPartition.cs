using System;
using System.Collections.Generic;
using MonoGame.Input;
using MonoGame.Players;

namespace MonoGame.Interfaces;

public interface ISpatialPartition<T> : ICollection<T>, IDisposable where T : ICollidable, IRenderable
{
    public void Update(float deltaTime, Controls controls);
    public void Draw(IList<Player> players, float deltaTime);
}