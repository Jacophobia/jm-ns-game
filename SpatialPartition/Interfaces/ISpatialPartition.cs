using System.Collections.Generic;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpatialPartition.Collision;

namespace SpatialPartition.Interfaces;

public interface ISpatialPartition<T> : ICollection<T> where T : ICollidable
{
    public void Update(GameTime gameTime, Controls controls);
    public void Draw(SpriteBatch spriteBatch, Camera camera, GameTime gameTime);
}