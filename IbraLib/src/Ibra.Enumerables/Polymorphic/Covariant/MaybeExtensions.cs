using System;
using System.Collections.Generic;
using System.Linq;

using static Ibra.Enumerables.Extensions;

namespace Ibra.Polymorphic.Covariant.Extensions
{

    public static class MaybeExtensions
    {

        public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe)
            => maybe.Convert(One, Enumerable.Empty<T>);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<Maybe<T>> sequence)
        {
            foreach (var maybe in sequence)
            {
                if (maybe is Just<T> just) yield return just.Value;
            }
        }

        public static IEnumerable<T> SelectWhere<U, T>(this IEnumerable<U> sequence, Func<U, Maybe<T>> func)
            => sequence.Select(func).Flatten();

        public static IEnumerable<T> AppendMaybe<T>(this IEnumerable<T> sequence, Maybe<T> maybe)
            => maybe.HasValue ? sequence.Concat(One(((Just<T>)maybe).Value)) : sequence;

        public static Maybe<T> FirstMaybe<T>(this IEnumerable<T> sequence)
        {
            IEnumerator<T> enumerator = sequence.GetEnumerator();
            if (enumerator.MoveNext()) return Maybe.Just(enumerator.Current);
            else return Nothing<T>.Instance;
        }

        public static Maybe<T> FirstMaybe<T>(this IEnumerable<T> sequence, Predicate<T> predicate)
        {
            IEnumerator<T> enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current)) return Maybe.Just(enumerator.Current);
            }
            return Nothing<T>.Instance;
        }

        public static Maybe<TValue> Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out TValue result))
            {
                return Maybe.Just(result);
            }
            else
            {
                return Nothing<TValue>.Instance;
            }
        }

        public static Maybe<TValue> Get<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out TValue result))
            {
                return Maybe.Just(result);
            }
            else
            {
                return Nothing<TValue>.Instance;
            }
        }

        public static IEnumerable<TCommon> MergeAll<TA, TB, TCommon>(this IEnumerable<Either<TA, TB>> sequence)
            where TA : TCommon
            where TB : TCommon
            => sequence.Select(either => either.Map<TCommon>(a => a, b => b));

        public static IEnumerable<TCommon> MergeAll<TA, TB, TC, TCommon>(this IEnumerable<Either<TA, TB, TC>> sequence)
            where TA : TCommon
            where TB : TCommon
            where TC : TCommon
            => sequence.Select(either => either.Map<TCommon>(a => a, b => b, c => c));

    }
}
