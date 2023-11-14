﻿using Microsoft.Xna.Framework;

namespace IO.Extensions;

public static class ColorExtensions
{
    public static Color ToStepped(this Color color)
    {
        color.R = (byte)(byte.MaxValue * (1f / 3f));
        color.G = (byte)(byte.MaxValue * (2f / 3f));
        color.B = byte.MaxValue;
        color.A = byte.MaxValue;
        return color;
    }
    
    public static void Next(this Color color)
    {
        color.R++;
        color.G++;
        color.B++;
        
        (color.R, color.G, color.B) = (color.G, color.B, color.R);
    }
}