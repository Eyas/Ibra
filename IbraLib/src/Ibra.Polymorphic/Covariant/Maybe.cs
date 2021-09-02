using System;

namespace Ibra.Polymorphic.Covariant
{

    /// <summary>
    /// A Covariant optional type that represents a value that may or may not be present.
    /// </summary>
    /// <remarks>
    /// As a covariant interface, Maybe(T) and its concrete implementations, Just(T) and
    /// Nothing(T), are reference types. This means that a variable of type Maybe(T) can
    /// still be null. Maybe(T) is provided as a tool for those programmers who wish to
    /// refrain from using null values, so care should be taken on part of the developer
    /// using Maybe(T) to make sure that value is never set to null, and that Nothing(T)
    /// is always use in those places.
    /// 
    /// As a reference type, Maybe(T) is heap-allocated. Those wishing, for performance
    /// reasons, to reduce allocations, should consider using the invariant Maybe type,
    /// which is a value type and as such stack allocated.
    /// 
    /// Covariant Maybe(T) does NOT support equality semantics out of the box and cannot
    /// be used as a key in a HashSet or Dictionary. To compare two covariant Maybe types,
    /// use static helper method Maybe.MaybeEqual{T1, T2}(Maybe{T1}, Maybe{T2}).
    /// </remarks>
    /// <typeparam name="T">The type that could be present in this object</typeparam>
    public interface Maybe<out T> where T : notnull
    {
        /// <summary>
        /// Indicates whether or not a value is present in this instance
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// If HasValue is true, provides the value, else throws NotSupportedException
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If attempting to access a value when no value is present.
        /// </exception>
        /// <remarks>
        /// Access to Value must be restricted to logic that is conditional on HasValue
        /// </remarks>
        [Obsolete("Direct use of Maybe<T>.Value is deprecated. Use Map, FlatMap, Convert, or Do instead.")]
        T Value { get; }

        /// <summary>
        /// Maps this into a Maybe(<typeparamref name="TResult"/>).
        /// Uses <paramref name="mapper"/> if a value is present.
        /// </summary>
        /// <typeparam name="TResult">
        /// The intended output type.
        /// </typeparam>
        /// <param name="mapper">
        /// The function to use to map an existing value into the output type.
        /// </param>
        /// <returns>
        /// A Just(<typeparamref name="TResult"/>) with <paramref name="mapper"/>(Value),
        /// if a value is present, or a Nothing(<typeparamref name="TResult"/>) otherwise.
        /// </returns>
        Maybe<TResult> Map<TResult>(Func<T, TResult> mapper) where TResult : notnull;

        /// <summary>
        /// Maps this into a Maybe(<typeparamref name="TResult"/>).
        /// Uses <paramref name="mapper"/> if a value is present.
        /// </summary>
        /// <typeparam name="TResult">
        /// The intended output type.
        /// </typeparam>
        /// <param name="mapper">
        /// The function to use to map an existing value into the output type.
        /// </param>
        /// <returns>
        /// A Maybe(<typeparamref name="TResult"/>) with <paramref name="mapper"/>(Value),
        /// if a value is present, or a Nothing(<typeparamref name="TResult"/>) otherwise.
        /// </returns>
        Maybe<TResult> FlatMap<TResult>(Func<T, Maybe<TResult>> mapper) where TResult : notnull;

        /// <summary>
        /// Returns a concrete value from this Maybe(<typeparamref name="T"/>).
        /// </summary>
        /// <typeparam name="TResult">Intended return type.</typeparam>
        /// <param name="justFunc">
        /// Mapping function to use to get the <typeparamref name="TResult"/> value
        /// if a value is present.
        /// </param>
        /// <param name="nothingFunc">
        /// Function to use to get the <typeparamref name="TResult"/> value if no
        /// value is present.
        /// </param>
        /// <returns>
        /// A <typeparamref name="TResult"/> produced by applying either
        /// <paramref name="justFunc"/> or or <paramref name="nothingFunc"/>.
        /// </returns>
        TResult Convert<TResult>(Func<T, TResult> justFunc, Func<TResult> nothingFunc);

        /// <summary>
        /// Returns a concrete value from this Maybe(<typeparamref name="T"/>).
        /// </summary>
        /// <typeparam name="TResult">Intended return type.</typeparam>
        /// <param name="justFunc">
        /// Mapping function to use to get the <typeparamref name="TResult"/> value
        /// if a value is present.
        /// </param>
        /// <param name="nothing">
        /// Default value if none is present.
        /// </param>
        /// <returns>
        /// Either a <typeparamref name="TResult"/> produced by applying
        /// <paramref name="justFunc"/>, or <paramref name="nothing"/>.
        /// </returns>
        TResult Convert<TResult>(Func<T, TResult> justFunc, TResult nothing);

        /// <summary>
        /// Apply a predicate to the current value and return a Maybe which
        /// holds a value if this previously held a value and the predicate
        /// matches that value.
        /// </summary>
        /// <param name="filter">
        /// The predicate to be matched against the value.
        /// </param>
        /// <returns>
        /// A Maybe(<typeparamref name="T"/>). Either a Just(<typeparamref name="T"/>)
        /// if and only if this is a Just(<typeparamref name="T"/>) and the predicate
        /// matches Value, or otherwise a Nothing(<typeparamref name="T"/>).
        /// </returns>
        Maybe<T> Filter(Predicate<T> filter);

        void Do(Action<T> justFunc);
        void Do(Action<T> justFunc, Action nothingFunc);
    }

    public interface Just<out T> : Maybe<T> where T : notnull
    {
        new T Value { get; }
    }


    /// Concrete implementation of the `Just` covariant interface.
    internal class CJust<T> : Just<T> where T : notnull
    {
        public CJust(T value) { Value = value; }

        public bool HasValue => true;
        public T Value { get; }
        T Maybe<T>.Value => Value;

        public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper) where TResult : notnull
        {
            return new CJust<TResult>(mapper(Value));
        }

        public Maybe<TResult> FlatMap<TResult>(Func<T, Maybe<TResult>> mapper) where TResult : notnull => mapper(Value);
        public TResult Convert<TResult>(Func<T, TResult> justFunc, Func<TResult> nothingFunc) => justFunc(Value);
        public TResult Convert<TResult>(Func<T, TResult> justFunc, TResult nothing) => justFunc(Value);
        public Maybe<T> Filter(Predicate<T> filter)
        {
            if (filter(Value)) return this;
            else return Nothing<T>.Instance;
        }
        public void Do(Action<T> justFunc)
        {
            justFunc(Value);
        }
        public void Do(Action<T> justFunc, Action nothingFunc)
        {
            justFunc(Value);
        }
    }

    public class Nothing<T> : Maybe<T> where T : notnull
    {
        public static readonly Nothing<T> Instance = new();

        private Nothing() { }

        public bool HasValue => false;
        T Maybe<T>.Value { get { throw new NotSupportedException($"Attempting to access `Value` on a {nameof(Nothing<T>)}."); } }

        public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper) where TResult : notnull => Nothing<TResult>.Instance;
        public Maybe<TResult> FlatMap<TResult>(Func<T, Maybe<TResult>> mapper) where TResult : notnull => Nothing<TResult>.Instance;
        public TResult Convert<TResult>(Func<T, TResult> justFunc, Func<TResult> nothingFunc) => nothingFunc();
        public TResult Convert<TResult>(Func<T, TResult> justFunc, TResult nothing) => nothing;
        public Maybe<T> Filter(Predicate<T> filter) => this;
        public void Do(Action<T> justFunc) { }
        public void Do(Action<T> justFunc, Action nothingFunc)
        {
            nothingFunc();
        }

    }

    public static class Maybe
    {
        /// <summary>
        /// Indicates that two Maybe instances are equal if and only of:
        /// <list type="bullet">
        /// <item><description>
        /// both Maybe instances hold no value, regardless of their type, or
        /// </description></item>
        /// <item><description>
        /// both Maybe instances hold a value, and their values are Equal
        /// </description></item>
        /// </list>
        /// </summary>
        public static bool MaybeEqual<T1, T2>(Maybe<T1> lhs, Maybe<T2> rhs) where T1 : notnull where T2 : notnull
        {
            if (!lhs.HasValue && !rhs.HasValue) return true;
            if (!lhs.HasValue || !rhs.HasValue) return false;
            return ((Just<T1>)lhs).Value.Equals(((Just<T2>)rhs).Value);
        }

        /// <summary>
        /// Returns true if and only if this Maybe(T) type has a value
        /// and that value is equal <paramref name="other"/>.
        /// </summary>
        public static bool JustEquals<T>(this Maybe<T> maybe, T other) where T : notnull => maybe.Convert(v => v.Equals(other), false);

        /// <summary>
        /// Get the contained value of a Maybe polymorphic type if it is
        /// present, or <paramref name="defaultValue"/> otherwise.
        /// </summary>
        /// <typeparam name="T">Contained type</typeparam>
        public static T GetOrElse<T>(this Maybe<T> maybe, T defaultValue) where T : notnull => maybe.HasValue ? ((Just<T>)maybe).Value : defaultValue;

        /// <summary>
        /// Get the contained value of a Maybe polymorphic type if it is
        /// present, or the result of applying <paramref name="nothingFunc"/>
        /// otherwise.
        /// </summary>
        /// <typeparam name="T">Contained type</typeparam>
        public static T GetOrElse<T>(this Maybe<T> maybe, Func<T> nothingFunc) where T : notnull => maybe.HasValue ? ((Just<T>)maybe).Value : nothingFunc();

        /// <summary>
        /// Get a Maybe type out of a definite value.
        /// </summary>
        /// <typeparam name="T">The type of the definite value</typeparam>
        public static Just<T> Just<T>(T value) where T : notnull => new CJust<T>(value);

        /// <summary>
        /// Map a Maybe of a discriminated union into a Maybe of some type
        /// </summary>
        public static Maybe<T> Map<T, EA, EB>(this Maybe<Either<EA, EB>> maybe, Func<EA, T> mapA, Func<EB, T> mapB) where T : notnull where EA : notnull where EB : notnull =>
            maybe.Map(either => either.Map(mapA, mapB));

        /// <summary>
        /// Map a Maybe of a discriminated union into a Maybe of some type
        /// </summary>
        public static Maybe<T> Map<T, EA, EB, EC>(
            this Maybe<Either<EA, EB, EC>> maybe, Func<EA, T> mapA, Func<EB, T> mapB, Func<EC, T> mapC)
                where T : notnull
            where EA : notnull
            where EB : notnull
            where EC : notnull
            =>
            maybe.Map(either => either.Map(mapA, mapB, mapC));

        /// <summary>
        /// Converts a Maybe(<paramref name="T"/>) to a <paramref name="T"/>?
        /// nullable object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <returns></returns>
        public static T? ToNullable<T>(this Maybe<T> from) where T : struct =>
            from.Convert(v => (T?)v, () => (T?)null);
    }

}
