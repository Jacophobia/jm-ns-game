using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Input;
using MonoGame.Output;

namespace MonoGame.Interfaces;

public interface ISpatialPartition<T> : ICollection<T> where T : ICollidable
{
    public void Update(GameTime gameTime, Controls[] controls);
    public void Draw(Renderer renderer, Camera camera, GameTime gameTime);
}