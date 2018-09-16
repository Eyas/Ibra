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
        private readonly Func<T, R> _func;
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
        public LazyFunc(Func<Func<T, R>, T, R> recursiveFunc, IEqualityComparer<T> comparer)
        {
            _func = (T param) => recursiveFunc(Get, param);
            _lazy = new Dictionary<T, ResultInfo>(comparer);
        }
        public LazyFunc(Func<Func<T, R>, T, R> recursiveFunc)
        {
            _func = (T param) => recursiveFunc(Get, param);
            _lazy = new Dictionary<T, ResultInfo>();
        }
        public R this[T input]
        {
            get
            {
                if (_lazy.TryGetValue(input, out ResultInfo result))
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
        private readonly LazyFunc<(T1, T2), R> _lazy;

        public LazyFunc(Func<T1, T2, R> func, IEqualityComparer<(T1, T2)> comparer)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2), comparer);
        }
        public LazyFunc(Func<T1, T2, R> func)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2), EqualityComparer<(T1, T2)>.Default);
        }
        public LazyFunc(Func<Func<T1, T2, R>, T1, T2, R> recursiveFunc, IEqualityComparer<(T1, T2)> comparer)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2), comparer);
        }
        public LazyFunc(Func<Func<T1, T2, R>, T1, T2, R> recursiveFunc)
        {
            _lazy = LazyFunc.Create(input => recursiveFunc(Get, input.Item1, input.Item2), EqualityComparer<(T1, T2)>.Default);
        }

        public R Get(T1 arg1, T2 arg2)
        {
            return _lazy[(arg1, arg2)];
        }
    }
    public sealed class LazyFunc<T1, T2, T3, R> : ILazyFunc<T1, T2, T3, R>
    {
        private readonly LazyFunc<(T1, T2, T3), R> _lazy;

        public LazyFunc(Func<T1, T2, T3, R> func, IEqualityComparer<(T1, T2, T3)> comparer)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2, tuple.Item3), comparer);
        }
        public LazyFunc(Func<T1, T2, T3, R> func)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2, tuple.Item3), EqualityComparer<(T1, T2, T3)>.Default);
        }
        public LazyFunc(Func<Func<T1, T2, T3, R>, T1, T2, T3, R> recursiveFunc, IEqualityComparer<(T1, T2, T3)> comparer)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2, input.Item3), comparer);
        }
        public LazyFunc(Func<Func<T1, T2, T3, R>, T1, T2, T3, R> recursiveFunc)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2, input.Item3), EqualityComparer<(T1, T2, T3)>.Default);
        }
        public R Get(T1 arg1, T2 arg2, T3 arg3)
        {
            return _lazy[(arg1, arg2, arg3)];
        }
    }
    public sealed class LazyFunc<T1, T2, T3, T4, R> : ILazyFunc<T1, T2, T3, T4, R>
    {
        private readonly LazyFunc<(T1, T2, T3, T4), R> _lazy;

        public LazyFunc(Func<T1, T2, T3, T4, R> func, IEqualityComparer<(T1, T2, T3, T4)> comparer)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4), comparer);
        }
        public LazyFunc(Func<T1, T2, T3, T4, R> func)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4), EqualityComparer<(T1, T2, T3, T4)>.Default);
        }
        public LazyFunc(Func<Func<T1, T2, T3, T4, R>, T1, T2, T3, T4, R> recursiveFunc, IEqualityComparer<(T1, T2, T3, T4)> comparer)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2, input.Item3, input.Item4), comparer);
        }
        public LazyFunc(Func<Func<T1, T2, T3, T4, R>, T1, T2, T3, T4, R> recursiveFunc)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2, input.Item3, input.Item4), EqualityComparer<(T1, T2, T3, T4)>.Default);
        }
        public R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return _lazy[(arg1, arg2, arg3, arg4)];
        }
    }
    public sealed class LazyFunc<T1, T2, T3, T4, T5, R> : ILazyFunc<T1, T2, T3, T4, T5, R>
    {
        private readonly LazyFunc<(T1, T2, T3, T4, T5), R> _lazy;

        public LazyFunc(Func<T1, T2, T3, T4, T5, R> func, IEqualityComparer<(T1, T2, T3, T4, T5)> comparer)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5), comparer);
        }
        public LazyFunc(Func<T1, T2, T3, T4, T5, R> func)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5), EqualityComparer<(T1, T2, T3, T4, T5)>.Default);
        }
        public LazyFunc(Func<Func<T1, T2, T3, T4, T5, R>, T1, T2, T3, T4, T5, R> recursiveFunc, IEqualityComparer<(T1, T2, T3, T4, T5)> comparer)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2, input.Item3, input.Item4, input.Item5), comparer);
        }
        public LazyFunc(Func<Func<T1, T2, T3, T4, T5, R>, T1, T2, T3, T4, T5, R> recursiveFunc)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2, input.Item3, input.Item4, input.Item5), EqualityComparer<(T1, T2, T3, T4, T5)>.Default);
        }
        public R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return _lazy[(arg1, arg2, arg3, arg4, arg5)];
        }
    }
    public sealed class LazyFunc<T1, T2, T3, T4, T5, T6, R> : ILazyFunc<T1, T2, T3, T4, T5, T6, R>
    {
        private readonly LazyFunc<(T1, T2, T3, T4, T5, T6), R> _lazy;

        public LazyFunc(Func<T1, T2, T3, T4, T5, T6, R> func, IEqualityComparer<(T1, T2, T3, T4, T5, T6)> comparer)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6), comparer);
        }
        public LazyFunc(Func<T1, T2, T3, T4, T5, T6, R> func)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6), EqualityComparer<(T1, T2, T3, T4, T5, T6)>.Default);
        }
        public LazyFunc(Func<Func<T1, T2, T3, T4, T5, T6, R>, T1, T2, T3, T4, T5, T6, R> recursiveFunc, IEqualityComparer<(T1, T2, T3, T4, T5, T6)> comparer)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2, input.Item3, input.Item4, input.Item5, input.Item6), comparer);
        }
        public LazyFunc(Func<Func<T1, T2, T3, T4, T5, T6, R>, T1, T2, T3, T4, T5, T6, R> recursiveFunc)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2, input.Item3, input.Item4, input.Item5, input.Item6), EqualityComparer<(T1, T2, T3, T4, T5, T6)>.Default);
        }
        public R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return _lazy[(arg1, arg2, arg3, arg4, arg5, arg6)];
        }
    }
    public sealed class LazyFunc<T1, T2, T3, T4, T5, T6, T7, R> : ILazyFunc<T1, T2, T3, T4, T5, T6, T7, R>
    {
        private readonly LazyFunc<(T1, T2, T3, T4, T5, T6, T7), R> _lazy;

        public LazyFunc(Func<T1, T2, T3, T4, T5, T6, T7, R> func, IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7)> comparer)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7), comparer);
        }
        public LazyFunc(Func<T1, T2, T3, T4, T5, T6, T7, R> func)
        {
            _lazy = LazyFunc.Create(tuple => func(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7), EqualityComparer<(T1, T2, T3, T4, T5, T6, T7)>.Default);
        }
        public LazyFunc(Func<Func<T1, T2, T3, T4, T5, T6, T7, R>, T1, T2, T3, T4, T5, T6, T7, R> recursiveFunc, IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7)> comparer)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2, input.Item3, input.Item4, input.Item5, input.Item6, input.Item7), comparer);
        }
        public LazyFunc(Func<Func<T1, T2, T3, T4, T5, T6, T7, R>, T1, T2, T3, T4, T5, T6, T7, R> recursiveFunc)
        {
            _lazy = LazyFunc.Create(
                input => recursiveFunc(Get, input.Item1, input.Item2, input.Item3, input.Item4, input.Item5, input.Item6, input.Item7), EqualityComparer<(T1, T2, T3, T4, T5, T6, T7)>.Default);
        }
        public R Get(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return _lazy[(arg1, arg2, arg3, arg4, arg5, arg6, arg7)];
        }
    }

    public static class LazyFunc
    {
        public static LazyFunc<T, R> Create<T, R>(Func<T, R> func, IEqualityComparer<T> comparer)
        {
            return new LazyFunc<T, R>(func, comparer);
        }
        public static LazyFunc<T, R> Create<T, R>(Func<T, R> func)
        {
            return new LazyFunc<T, R>(func);
        }
        public static LazyFunc<T, R> Create<T, R>(Func<Func<T, R>, T, R> recursiveFunc, IEqualityComparer<T> comparer)
        {
            return new LazyFunc<T, R>(recursiveFunc, comparer);
        }
        public static LazyFunc<T, R> Create<T, R>(Func<Func<T, R>, T, R> recursiveFunc)
        {
            return new LazyFunc<T, R>(recursiveFunc);
        }
        public static LazyFunc<T1, T2, R> Create<T1, T2, R>(Func<T1, T2, R> func, IEqualityComparer<(T1, T2)> comparer)
        {
            return new LazyFunc<T1, T2, R>(func, comparer);
        }
        public static LazyFunc<T1, T2, R> Create<T1, T2, R>(Func<T1, T2, R> func)
        {
            return new LazyFunc<T1, T2, R>(func);
        }
        public static LazyFunc<T1, T2, R> Create<T1, T2, R>(Func<Func<T1, T2, R>, T1, T2, R> recursiveFunc, IEqualityComparer<(T1, T2)> comparer)
        {
            return new LazyFunc<T1, T2, R>(recursiveFunc, comparer);
        }
        public static LazyFunc<T1, T2, R> Create<T1, T2, R>(Func<Func<T1, T2, R>, T1, T2, R> recursiveFunc)
        {
            return new LazyFunc<T1, T2, R>(recursiveFunc);
        }
        public static LazyFunc<T1, T2, T3, R> Create<T1, T2, T3, R>(Func<T1, T2, T3, R> func, IEqualityComparer<(T1, T2, T3)> comparer)
        {
            return new LazyFunc<T1, T2, T3, R>(func, comparer);
        }
        public static LazyFunc<T1, T2, T3, R> Create<T1, T2, T3, R>(Func<T1, T2, T3, R> func)
        {
            return new LazyFunc<T1, T2, T3, R>(func);
        }
        public static LazyFunc<T1, T2, T3, R> Create<T1, T2, T3, R>(Func<Func<T1, T2, T3, R>, T1, T2, T3, R> recursiveFunc, IEqualityComparer<(T1, T2, T3)> comparer)
        {
            return new LazyFunc<T1, T2, T3, R>(recursiveFunc, comparer);
        }
        public static LazyFunc<T1, T2, T3, R> Create<T1, T2, T3, R>(Func<Func<T1, T2, T3, R>, T1, T2, T3, R> recursiveFunc)
        {
            return new LazyFunc<T1, T2, T3, R>(recursiveFunc);
        }
        public static LazyFunc<T1, T2, T3, T4, R> Create<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func, IEqualityComparer<(T1, T2, T3, T4)> comparer)
        {
            return new LazyFunc<T1, T2, T3, T4, R>(func, comparer);
        }
        public static LazyFunc<T1, T2, T3, T4, R> Create<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func)
        {
            return new LazyFunc<T1, T2, T3, T4, R>(func);
        }
        public static LazyFunc<T1, T2, T3, T4, R> Create<T1, T2, T3, T4, R>(Func<Func<T1, T2, T3, T4, R>, T1, T2, T3, T4, R> recursiveFunc, IEqualityComparer<(T1, T2, T3, T4)> comparer)
        {
            return new LazyFunc<T1, T2, T3, T4, R>(recursiveFunc, comparer);
        }
        public static LazyFunc<T1, T2, T3, T4, R> Create<T1, T2, T3, T4, R>(Func<Func<T1, T2, T3, T4, R>, T1, T2, T3, T4, R> recursiveFunc)
        {
            return new LazyFunc<T1, T2, T3, T4, R>(recursiveFunc);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, R> Create<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func, IEqualityComparer<(T1, T2, T3, T4, T5)> comparer)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, R>(func, comparer);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, R> Create<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, R>(func);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, R> Create<T1, T2, T3, T4, T5, R>(Func<Func<T1, T2, T3, T4, T5, R>, T1, T2, T3, T4, T5, R> recursiveFunc, IEqualityComparer<(T1, T2, T3, T4, T5)> comparer)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, R>(recursiveFunc, comparer);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, R> Create<T1, T2, T3, T4, T5, R>(Func<Func<T1, T2, T3, T4, T5, R>, T1, T2, T3, T4, T5, R> recursiveFunc)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, R>(recursiveFunc);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, T6, R> Create<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, IEqualityComparer<(T1, T2, T3, T4, T5, T6)> comparer)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, T6, R>(func, comparer);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, T6, R> Create<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, T6, R>(func);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, T6, R> Create<T1, T2, T3, T4, T5, T6, R>(Func<Func<T1, T2, T3, T4, T5, T6, R>, T1, T2, T3, T4, T5, T6, R> recursiveFunc, IEqualityComparer<(T1, T2, T3, T4, T5, T6)> comparer)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, T6, R>(recursiveFunc, comparer);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, T6, R> Create<T1, T2, T3, T4, T5, T6, R>(Func<Func<T1, T2, T3, T4, T5, T6, R>, T1, T2, T3, T4, T5, T6, R> recursiveFunc)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, T6, R>(recursiveFunc);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, T6, T7, R> Create<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7)> comparer)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, T6, T7, R>(func, comparer);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, T6, T7, R> Create<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, T6, T7, R>(func);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, T6, T7, R> Create<T1, T2, T3, T4, T5, T6, T7, R>(Func<Func<T1, T2, T3, T4, T5, T6, T7, R>, T1, T2, T3, T4, T5, T6, T7, R> recursiveFunc, IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7)> comparer)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, T6, T7, R>(recursiveFunc, comparer);
        }
        public static LazyFunc<T1, T2, T3, T4, T5, T6, T7, R> Create<T1, T2, T3, T4, T5, T6, T7, R>(Func<Func<T1, T2, T3, T4, T5, T6, T7, R>, T1, T2, T3, T4, T5, T6, T7, R> recursiveFunc)
        {
            return new LazyFunc<T1, T2, T3, T4, T5, T6, T7, R>(recursiveFunc);
        }
    }
}
