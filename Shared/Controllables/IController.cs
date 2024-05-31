using Microsoft.Xna.Framework;

namespace Shared.Controllables;

public interface IController
{
    public Controls Controls { get; }
    public Vector2 LeftJoystick { get; }
    public Vector2 RightJoystick { get; }
}