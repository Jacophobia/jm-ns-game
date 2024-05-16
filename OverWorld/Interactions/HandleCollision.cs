using OverWorld.GameObjects;

namespace OverWorld.Interactions;

public class HandleCollision : Interaction
{
    public override void Apply(IGameObject lhs, IGameObject rhs)
    {
        if (IsThereACollision(lhs, rhs))
        {
            PerformCollision(lhs, rhs);
        }
    }

    private static bool IsThereACollision(IGameObject lhs, IGameObject rhs)
    {
        throw new System.NotImplementedException();
    }

    private static void PerformCollision(IGameObject lhs, IGameObject rhs)
    {
        throw new System.NotImplementedException();
    }
}