using System;
using System.Collections.Generic;
using System.Linq;

namespace Ibra.Enumerables
{
    public static class Extensions
    {
        /// <summary>
        /// Flatten out a nested sequence of sequences.
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> sequence) =>
            sequence.SelectMany(sublist => sublist);

        /// <summary>
        /// Return a sequence with only non-nullable values present.
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T?> sequence) where T : struct
        {
            foreach (var nullable in sequence)
            {
                if (nullable.HasValue) yield return nullable.Value;
            }
        }

        /// <summary>
        /// Project each sequence elemnt into a non-null result based on <param name="func"/>.
        /// </summary>
        public static IEnumerable<T> SelectWhere<U, T>(this IEnumerable<U> sequence, Func<U, T?> func) where T : struct
            => sequence.Select(func).Flatten();

        /// <summary>
        /// Project a sequence into the corresponding value in the <paramref name="dictionary"/>
        /// provided, skipping all keys in the input <paramref name="sequence"/> that don't exist
        /// in the dictionary.
        /// </summary>
        public static IEnumerable<V> TrySelectFromDictionary<K, V>(this IEnumerable<K> sequence, IReadOnlyDictionary<K, V> dictionary)
        {
            foreach (K key in sequence)
            {
                if (dictionary.TryGetValue(key, out V value)) yield return value;
            }
        }

        /// <summary>
        /// Treat a <see cref="Nullable{T}"/> <paramref name="maybe"/> a sequence with
        /// no elements (if null), or a single element (if not null).
        /// </summary>
        public static IEnumerable<T> ToEnumerable<T>(this T? maybe) where T : struct
            => maybe.HasValue ? One(maybe.Value) : Enumerable.Empty<T>();

        /// <summary>
        /// Creates an Enumerable with a single element, <paramref name="item"/>.
        /// </summary>
        public static IEnumerable<T> One<T>(T item) => new SingleEnumerable<T>(item);

        /// <summary>
        /// Project each element of <paramref name="sequence"/> into an element of
        /// type <typeparamref name="TTo"/>, if the input element is of that type.
        /// Skips every element where the cast to <typeparamref name="TTo"/> fails.
        /// </summary>
        /// <typeparam name="TFrom">Input runtime type of each element.</typeparam>
        /// <typeparam name="TTo">Output type of each remaining element.</typeparam>
        public static IEnumerable<TTo> WhereCast<TFrom, TTo>(this IEnumerable<TFrom> sequence) where TTo : class, TFrom
        {
            IEnumerator<TFrom> e = sequence.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current is TTo to) yield return to;
            }
        }

        /// <summary>
        /// Combines two input <see cref="IEnumerable{T}"/>s into a <see cref="ValueTuple{T1, T2}"/>
        /// of both elements.
        /// </summary>
        /// <seealso cref="System.Linq.Enumerable.Zip{TFirst, TSecond, TResult}(IEnumerable{TFirst}, IEnumerable{TSecond}, Func{TFirst, TSecond, TResult})"/>
        public static IEnumerable<(T1, T2)> Combine<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
            => first.Zip(second, ValueTuple.Create);

        /// <summary>
        /// Projects each value into a <see cref="ValueTuple{T1, T2}"/> with its index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <seealso cref="System.Linq.Enumerable.Zip{TFirst, TSecond, TResult}(IEnumerable{TFirst}, IEnumerable{TSecond}, Func{TFirst, TSecond, TResult})"/>
        public static IEnumerable<(int, T)> ZipWithIndex<T>(this IEnumerable<T> sequence)
            => sequence.Select((elt, idx) => ValueTuple.Create(idx, elt));
    }
}

