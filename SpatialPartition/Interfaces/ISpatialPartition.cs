using System.Collections.Generic;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;

namespace SpatialPartition.Interfaces;

public interface ISpatialPartition<T> : ICollection<T> where T : ICollidable
{
    public void Update(GameTime gameTime, Controls controls);
    public void Draw(Renderer renderer, Camera camera, GameTime gameTime);
}