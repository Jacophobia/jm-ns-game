using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Extensions;

internal static class EnumerableExtensions
{
    internal static Tuple<T, T> ToPair<T>(this IEnumerable<T> enumerable)
    {
        var array = enumerable.Take(2).ToArray();
        return Tuple.Create(array[0], array[1]);
    }

    internal static SortedSet<TSource> ToSortedSet<TSource>(this IEnumerable<TSource> source)
    {
        return new SortedSet<TSource>(source);
    }
}