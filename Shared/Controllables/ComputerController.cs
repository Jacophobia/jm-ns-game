using Microsoft.Xna.Framework;

namespace Shared.Controllables;

/// <summary>
/// This class is the basic control center for enemy players. It will
/// need to be able to get records of past fights the player had with
/// similar enemies and will need to learn from those fights.
/// </summary>
public class ComputerController : IController
{
    public Controls Controls => throw new System.NotImplementedException();
    public Vector2 LeftJoystick => throw new System.NotImplementedException();
    public Vector2 RightJoystick => throw new System.NotImplementedException();
}