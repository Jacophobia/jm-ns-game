using OverWorld.GameObjects;
using Shared.Collision;

namespace OverWorld.Interactions;

public class HandleCollision : Interaction
{
    public HandleCollision() { }
    
    public override void Apply(GameObject lhs, GameObject rhs)
    {
        if (CollisionFunctions.IsThereACollision(lhs, rhs))
        {
            CollisionFunctions.HandleCollision(lhs, rhs);
        }
    }
}