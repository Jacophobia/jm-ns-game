using System;
using System.Collections.Generic;
using System.Linq;

namespace IO.Extensions;

public static class EnumerableExtensions
{
    public static Tuple<T, T> ToPair<T>(this IEnumerable<T> enumerable)
    {
        var array = enumerable.Take(2).ToArray();
        return Tuple.Create(array[0], array[1]);
    }

    public static SortedSet<TSource> ToSortedSet<TSource>(this IEnumerable<TSource> source)
    {
        return new SortedSet<TSource>(source);
    }
}