using System;

namespace IO.Input;

[Flags]
public enum Controls
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
    AttackOne = 16,
    AttackTwo = 32,
    Jump = 64,
    Dodge = 128
}