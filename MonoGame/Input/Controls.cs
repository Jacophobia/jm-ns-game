using System;

namespace MonoGame.Input;

[Flags]
public enum Controls
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
    Jump = 16,
    ModifierOne = 32,
    ModifierTwo = 64,
    ModifierThree = 128,
    ModifierFour = 256,
    CommandOne = 512,
    CommandTwo = 1_024,
    CommandThree = 2_048,
    CommandFour = 4_096,
    Start = 8_192,
    Select = 16_384,
}