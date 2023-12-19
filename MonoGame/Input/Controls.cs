using System;

namespace MonoGame.Input;

[Flags]
public enum Controls : byte
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