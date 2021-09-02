using System;
using System.Collections.Generic;
using System.Linq;

using static Ibra.Enumerables.Extensions;

namespace Ibra.Polymorphic.Invariant.Extensions
{

    public static class MaybeExtensions
    {

        public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe) where T : notnull
            => maybe.Convert(One, Enumerable.Empty<T>);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<Maybe<T>> sequence) where T : notnull
        {
            foreach (var maybe in sequence)
            {
                if (maybe.HasValue) yield return maybe.Value;
            }
        }

        public static IEnumerable<T> SelectWhere<U, T>(this IEnumerable<U> sequence, Func<U, Maybe<T>> func) where T : notnull
            => sequence.Select(func).Flatten();

        public static IEnumerable<T> AppendMaybe<T>(this IEnumerable<T> sequence, Maybe<T> maybe) where T : notnull
            => maybe.HasValue ? sequence.Concat(One(maybe.Value)) : sequence;

        public static Maybe<T> FirstMaybe<T>(this IEnumerable<T> sequence) where T : notnull
        {
            IEnumerator<T> enumerator = sequence.GetEnumerator();
            if (enumerator.MoveNext()) return Maybe.Just(enumerator.Current);
            else return Maybe<T>.Nothing;
        }

        public static Maybe<T> FirstMaybe<T>(this IEnumerable<T> sequence, Predicate<T> predicate) where T : notnull
        {
            IEnumerator<T> enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current)) return Maybe.Just(enumerator.Current);
            }
            return Maybe<T>.Nothing;
        }

        public static Maybe<TValue> Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : notnull
        {
            if (dictionary.TryGetValue(key, out TValue? result))
            {
                return Maybe.Just(result);
            }
            else
            {
                return Maybe<TValue>.Nothing;
            }
        }

        public static Maybe<TValue> Get<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) where TValue : notnull
        {
            if (dictionary.TryGetValue(key, out TValue? result))
            {
                return Maybe.Just(result);
            }
            else
            {
                return Maybe<TValue>.Nothing;
            }
        }
    }
}
