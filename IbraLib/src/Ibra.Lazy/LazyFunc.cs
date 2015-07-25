using System;
using System.Collections.Generic;

namespace Ibra.Lazy
{
    /// <summary>
    /// Provides support for memoized functions, computed once for each unique input.
    /// </summary>
    /// <typeparam name="T">Input type</typeparam>
    /// <typeparam name="R">Return type</typeparam>
    public interface ILazyFunc<in T, out R>
    {
        R this[T input] { get; }
        R Get(T input);
    }

    /// <seealso cref="ILazyFunc{T, R}"/>
    public interface ILazyFunc<in T1, in T2, out R>
    {
        R Get(T1 arg1, T2 arg2);
    }

    /// <seealso cref="ILazyFunc{T, R}"/>
    public interface ILazyFunc<in T1, in T2, in T3, out R>
    {
        R Get(T1 arg1, T2 arg2, T3 arg3);
    }

    /// <seealso cref="ILazyFunc{T, R}"/>
    public interface ILazyFunc<in T1, in T2, in T3, in T4, out R>
    {
        R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    /// <seealso cref="ILazyFunc{T, R}"/>
    public interface ILazyFunc<in T1, in T2, in T3, in T4, in T5, out R>
    {
        R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    /// <seealso cref="ILazyFunc{T, R}"/>
    public interface ILazyFunc<in T1, in T2, in T3, in T4, in T5, in T6, out R>
    {
        R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    /// <seealso cref="ILazyFunc{T, R}"/>
    public interface ILazyFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out R>
    {
        R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }

    public sealed class LazyFunc<T, R> : ILazyFunc<T, R>
    {
        #region Internals
        private sealed class ResultInfo // needs to be a mutable reference type
        {
            public bool HasValue = false;
            public R Value = default(R);
        }
        /// <summary>
        /// Uncached version of our function
        /// </summary>
        private Func<T, R> _func;
        /// <summary>
        /// There are three states in this dictionary:
        /// (1) t is not in the dictionary: uncached
        /// (2) t is in the dictionary, with value Nothing-- faulted but not cached; don't infinitely recurse
        /// (3) t is in the dictionary, with a defined value-- cached and can be safely returned.
        /// </summary>
        private Dictionary<T, ResultInfo> _lazy;
        #endregion

        public LazyFunc(Func<T, R> func, IEqualityComparer<T> comparer)
        {
            _func = func;
            _lazy = new Dictionary<T, ResultInfo>(comparer);
        }
        public LazyFunc(Func<T, R> func)
        {
            _func = func;
            _lazy = new Dictionary<T, ResultInfo>();
        }
        public R this[T input]
        {
            get
            {
                ResultInfo result;
                if (_lazy.TryGetValue(input, out result))
                {
                    if (result.HasValue) return result.Value;
                    else throw new InvalidOperationException("Attemptign to recursively get value from LazyFunc.");
                } // all code paths return/throw

                result = new ResultInfo(); // HasValue = false on construction
                _lazy.Add(input, result); // add before calling to detect recursive calls
                R output = _func(input);
                result.Value = output;
                result.HasValue = true;
                return output;
            }
        }
        public R Get(T input) => this[input];
    }
    public sealed class LazyFunc<T1, T2, R> : ILazyFunc<T1, T2, R>
    {
        private LazyFunc<Tuple<T1, T2>, R> _lazy;

        public LazyFunc(Func<T1, T2, R> func, IEqualityComparer<Tuple<T1, T2>> comparer)
        {
            _lazy = new LazyFunc<Tuple<T1, T2>, R>((tuple) => func(tuple.Item1, tuple.Item2), comparer);
        }
        public LazyFunc(Func<T1, T2, R> func)
        {
            _lazy = new LazyFunc<Tuple<T1, T2>, R>((tuple) => func(tuple.Item1, tuple.Item2));
        }
        public R Get(T1 arg1, T2 arg2)
        {
            var input = Tuple.Create(arg1, arg2);
            return _lazy[input];
        }
    }
    public sealed class LazyFunc<T1, T2, T3, R> : ILazyFunc<T1, T2, T3, R>
    {
        private LazyFunc<Tuple<T1, T2, T3>, R> _lazy;

        public LazyFunc(Func<T1, T2, T3, R> func, IEqualityComparer<Tuple<T1, T2, T3>> comparer)
        {
            _lazy = new LazyFunc<Tuple<T1, T2, T3>, R>((tuple) => func(tuple.Item1, tuple.Item2, tuple.Item3), comparer);
        }
        public LazyFunc(Func<T1, T2, T3, R> func)
        {
            _lazy = new LazyFunc<Tuple<T1, T2, T3>, R>((tuple) => func(tuple.Item1, tuple.Item2, tuple.Item3));
        }
        public R Get(T1 arg1, T2 arg2, T3 arg3)
        {
            var input = Tuple.Create(arg1, arg2, arg3);
            return _lazy[input];
        }
    }
    public sealed class LazyFunc<T1, T2, T3, T4, R> : ILazyFunc<T1, T2, T3, T4, R>
    {
        private LazyFunc<Tuple<T1, T2, T3, T4>, R> _lazy;

        public LazyFunc(Func<T1, T2, T3, T4, R> func, IEqualityComparer<Tuple<T1, T2, T3, T4>> comparer)
        {
            _lazy = new LazyFunc<Tuple<T1, T2, T3, T4>, R>((tuple) => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4), comparer);
        }
        public LazyFunc(Func<T1, T2, T3, T4, R> func)
        {
            _lazy = new LazyFunc<Tuple<T1, T2, T3, T4>, R>((tuple) => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));
        }
        public R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var input = Tuple.Create(arg1, arg2, arg3, arg4);
            return _lazy[input];
        }
    }
    public sealed class LazyFunc<T1, T2, T3, T4, T5, R> : ILazyFunc<T1, T2, T3, T4, T5, R>
    {
        private LazyFunc<Tuple<T1, T2, T3, T4, T5>, R> _lazy;

        public LazyFunc(Func<T1, T2, T3, T4, T5, R> func, IEqualityComparer<Tuple<T1, T2, T3, T4, T5>> comparer)
        {
            _lazy = new LazyFunc<Tuple<T1, T2, T3, T4, T5>, R>((tuple) => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5), comparer);
        }
        public LazyFunc(Func<T1, T2, T3, T4, T5, R> func)
        {
            _lazy = new LazyFunc<Tuple<T1, T2, T3, T4, T5>, R>((tuple) => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5));
        }
        public R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var input = Tuple.Create(arg1, arg2, arg3, arg4, arg5);
            return _lazy[input];
        }
    }
    public sealed class LazyFunc<T1, T2, T3, T4, T5, T6, R> : ILazyFunc<T1, T2, T3, T4, T5, T6, R>
    {
        private LazyFunc<Tuple<T1, T2, T3, T4, T5, T6>, R> _lazy;

        public LazyFunc(Func<T1, T2, T3, T4, T5, T6, R> func, IEqualityComparer<Tuple<T1, T2, T3, T4, T5, T6>> comparer)
        {
            _lazy = new LazyFunc<Tuple<T1, T2, T3, T4, T5, T6>, R>((tuple) => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6), comparer);
        }
        public LazyFunc(Func<T1, T2, T3, T4, T5, T6, R> func)
        {
            _lazy = new LazyFunc<Tuple<T1, T2, T3, T4, T5, T6>, R>((tuple) => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6));
        }
        public R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            var input = Tuple.Create(arg1, arg2, arg3, arg4, arg5, arg6);
            return _lazy[input];
        }
    }
    public sealed class LazyFunc<T1, T2, T3, T4, T5, T6, T7, R> : ILazyFunc<T1, T2, T3, T4, T5, T6, T7, R>
    {
        private LazyFunc<Tuple<T1, T2, T3, T4, T5, T6, T7>, R> _lazy;

        public LazyFunc(Func<T1, T2, T3, T4, T5, T6, T7, R> func, IEqualityComparer<Tuple<T1, T2, T3, T4, T5, T6, T7>> comparer)
        {
            _lazy = new LazyFunc<Tuple<T1, T2, T3, T4, T5, T6, T7>, R>((tuple) => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7), comparer);
        }
        public LazyFunc(Func<T1, T2, T3, T4, T5, T6, T7, R> func)
        {
            _lazy = new LazyFunc<Tuple<T1, T2, T3, T4, T5, T6, T7>, R>((tuple) => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7));
        }
        public R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            var input = Tuple.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return _lazy[input];
        }
    }
}
