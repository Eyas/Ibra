using System;
using System.Collections.Generic;
using System.Linq;

using static Ibra.Enumerables.Extensions;

namespace Ibra.Polymorphic.Invariant.Extensions
{

    public static class MaybeExtensions
    {

        public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe)
            => maybe.Convert(One, Enumerable.Empty<T>);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<Maybe<T>> sequence)
        {
            foreach (var maybe in sequence)
            {
                if (maybe.HasValue) yield return maybe.Value;
            }
        }

        public static IEnumerable<T> SelectWhere<U, T>(this IEnumerable<U> sequence, Func<U, Maybe<T>> func)
            => sequence.Select(func).Flatten();

        public static IEnumerable<T> AppendMaybe<T>(this IEnumerable<T> sequence, Maybe<T> maybe)
            => maybe.HasValue ? sequence.Concat(One(maybe.Value)) : sequence;

        public static Maybe<T> FirstMaybe<T>(this IEnumerable<T> sequence)
        {
            IEnumerator<T> enumerator = sequence.GetEnumerator();
            if (enumerator.MoveNext()) return Maybe.Just(enumerator.Current);
            else return Maybe<T>.Nothing;
        }

        public static Maybe<T> FirstMaybe<T>(this IEnumerable<T> sequence, Predicate<T> predicate)
        {
            IEnumerator<T> enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current)) return Maybe.Just(enumerator.Current);
            }
            return Maybe<T>.Nothing;
        }
    }
}
