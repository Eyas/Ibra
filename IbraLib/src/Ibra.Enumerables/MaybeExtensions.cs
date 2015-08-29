using System;
using System.Collections.Generic;
using System.Linq;

using static Ibra.Enumerables.Extensions;

namespace Ibra.Enumerables
{

    public static class MaybeExtensions
    {

        #region Extensions for Polymorphic.Invariant

        public static IEnumerable<T> ToEnumerable<T>(this Polymorphic.Invariant.Maybe<T> maybe)
            => maybe.Convert(One, Enumerable.Empty<T>);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<Polymorphic.Invariant.Maybe<T>> sequence)
        {
            foreach (var maybe in sequence)
            {
                if (maybe.HasValue) yield return maybe.Value;
            }
        }

        public static IEnumerable<T> SelectWhere<U, T>(this IEnumerable<U> sequence, Func<U, Polymorphic.Invariant.Maybe<T>> func)
            => sequence.Select(func).Flatten();

        public static IEnumerable<T> AppendMaybe<T>(this IEnumerable<T> sequence, Polymorphic.Invariant.Maybe<T> maybe)
            => maybe.HasValue ? sequence.Concat(One(maybe.Value)) : sequence;

        #endregion

        #region Extensions for Polymorphic Covariant

        public static IEnumerable<T> ToEnumerable<T>(this Polymorphic.Covariant.Maybe<T> maybe)
            => maybe.Convert(One, Enumerable.Empty<T>);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<Polymorphic.Covariant.Maybe<T>> sequence)
        {
            foreach (var maybe in sequence)
            {
                if (maybe.HasValue) yield return maybe.Value;
            }
        }

        public static IEnumerable<T> SelectWhere<U, T>(this IEnumerable<U> sequence, Func<U, Polymorphic.Covariant.Maybe<T>> func)
            => sequence.Select(func).Flatten();

        public static IEnumerable<T> AppendMaybe<T>(this IEnumerable<T> sequence, Polymorphic.Covariant.Maybe<T> maybe)
            => maybe.HasValue ? sequence.Concat(One(maybe.Value)) : sequence;

        public static IEnumerable<TCommon> MergeAll<TA, TB, TCommon>(this IEnumerable<Polymorphic.Covariant.Either<TA, TB>> sequence)
            where TA : TCommon
            where TB : TCommon
            => sequence.Select(either => either.Map<TCommon>(a => a, b => b));

        public static IEnumerable<TCommon> MergeAll<TA, TB, TC, TCommon>(this IEnumerable<Polymorphic.Covariant.Either<TA, TB, TC>> sequence)
            where TA : TCommon
            where TB : TCommon
            where TC : TCommon
            => sequence.Select(either => either.Map<TCommon>(a => a, b => b, c => c));

        #endregion

    }
}
