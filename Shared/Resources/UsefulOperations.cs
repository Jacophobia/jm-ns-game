namespace Shared.Resources;

public static class UsefulOperations
{
    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        (lhs, rhs) = (rhs, lhs);
    }
    
}