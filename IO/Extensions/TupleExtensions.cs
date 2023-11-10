using System;
using System.Collections.Generic;

namespace IO.Extensions;

public static class TupleExtensions
{
    public static IEnumerable<T> Enumerate<T>(this Tuple<T> tuple)
    {
        yield return tuple.Item1;
    }
    
    public static IEnumerable<T> Enumerate<T>(this Tuple<T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
    }
    
    public static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
    }
    
    public static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
    }
    
    public static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T, T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
    }
    
    public static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T, T, T, T> tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
        yield return tuple.Item3;
        yield return tuple.Item4;
        yield return tuple.Item5;
        yield return tuple.Item6;
    }
    
    public static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T, T, T, T, T> tuple)
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