using System;
using System.Diagnostics;

namespace Shared.Extensions;

internal static class ArrayExtensions
{
    internal static T Get<T>(this T[] array, int x, int y, int width)
    {
        Debug.Assert(array.Length % width == 0, "The array is missing elements or the width is incorrect");
        // Get the pixel indices
        return array[x + y * width];
    }
    
    
    internal static void Set<T>(this T[] array, int x, int y, int width, T value)
    {
        Debug.Assert(array.Length % width == 0, "The array is missing elements or the width is incorrect");
        // Get the pixel indices
        array[x + y * width] = value;
    }

    internal static bool TryGet<T>(this T[] array, int x, int y, int width, int height, out T element)
    {
        if (x > width - 1 || x < 0 || y > height - 1 || y < 0)
        {
            element = default;
            return false;
        }

        try
        {
            element = array.Get(x, y, width);
            return true;
        }
        catch (Exception)
        {
            element = default;
            return false;
        }
    }
}