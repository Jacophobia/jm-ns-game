using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Input;
using MonoGame.Output;

namespace MonoGame.Interfaces;

public interface ISpatialPartition<T> : ICollection<T>, IDisposable where T : ICollidable, IRenderable
{
    public void Update(float deltaTime, IList<Controls> controls);
    public void Draw(Renderer renderer, Camera[] cameras, float deltaTime);
}