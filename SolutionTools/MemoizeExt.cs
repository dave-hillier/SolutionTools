using System;
using System.Collections.Concurrent;

namespace SolutionTools
{
    // Taken from http://stackoverflow.com/a/1255107/1575281
    internal static class MemoizeExt
    {
        public static Func<TArg, TResult> Memoize<TArg, TResult>(this Func<TArg, TResult> f)
        {
            var map = new ConcurrentDictionary<TArg, Lazy<TResult>>();
            return a =>
            {
                var lazy = new Lazy<TResult>(() => f(a), true);
                return !map.TryAdd(a, lazy) ? map[a].Value : lazy.Value;
            };
        }
    }
}