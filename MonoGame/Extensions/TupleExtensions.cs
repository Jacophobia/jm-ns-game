using System;
using System.Collections.Generic;

namespace MonoGame.Extensions;

internal static class TupleExtensions
{
    internal static IEnumerable<T> Enumerate<T>(this Tuple<T> tuple)
    {
        yield return tuple.Item1;
    }

    internal static IEnumerable<T> Enumerate<T>(this Tuple<T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
    }

    internal static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
    }

    internal static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
    }

    internal static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T, T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
    }

    internal static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T, T, T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
        yield return tuple.Item6;
    }

    internal static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T, T, T, T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
        yield return tuple.Item6;
        yield return tuple.Item7;
    }
}