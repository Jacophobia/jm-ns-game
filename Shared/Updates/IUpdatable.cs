using Microsoft.Xna.Framework;

namespace Shared.Updates;

public interface IUpdatable
{
    void Update(GameTime gameTime);
}