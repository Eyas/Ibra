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
    public interface Maybe<out T>
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
        Maybe<TResult> Map<TResult>(Func<T, TResult> mapper);

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
        Maybe<TResult> FlatMap<TResult>(Func<T, Maybe<TResult>> mapper);

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
    }

    public class Just<T> : Maybe<T>
    {
        public Just(T value) { _value = value; }

        public bool HasValue => true;
        public T Value => _value;

        public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper) => new Just<TResult>(mapper(_value));
        public Maybe<TResult> FlatMap<TResult>(Func<T, Maybe<TResult>> mapper) => mapper(_value);
        public TResult Convert<TResult>(Func<T, TResult> justFunc, Func<TResult> nothingFunc) => justFunc(_value);
        public TResult Convert<TResult>(Func<T, TResult> justFunc, TResult nothing) => justFunc(_value);
        public Maybe<T> Filter(Predicate<T> filter)
        {
            if (filter(_value)) return this;
            else return Nothing<T>.Instance;
        }

        private readonly T _value;
    }

    public class Nothing<T> : Maybe<T>
    {
        public static readonly Nothing<T> Instance = new Nothing<T>();

        private Nothing() { }

        public bool HasValue => false;
        public T Value { get { throw new NotSupportedException($"Attempting to access {nameof(Value)} on a {nameof(Nothing<T>)}."); } }

        public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper) => Nothing<TResult>.Instance;
        public Maybe<TResult> FlatMap<TResult>(Func<T, Maybe<TResult>> mapper) => Nothing<TResult>.Instance;
        public TResult Convert<TResult>(Func<T, TResult> justFunc, Func<TResult> nothingFunc) => nothingFunc();
        public TResult Convert<TResult>(Func<T, TResult> justFunc, TResult nothing) => nothing;
        public Maybe<T> Filter(Predicate<T> filter) => this;

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
        public static bool MaybeEqual<T1, T2>(Maybe<T1> lhs, Maybe<T2> rhs)
        {
            if (!lhs.HasValue && !rhs.HasValue) return true;
            if (!lhs.HasValue || !rhs.HasValue) return false;
            return lhs.Value.Equals(rhs.Value);
        }

        /// <summary>
        /// Returns true if and only if this Maybe(T) type has a value
        /// and that value is equal <paramref name="other"/>.
        /// </summary>
        public static bool JustEquals<T>(this Maybe<T> maybe, T other) => maybe.Convert(v => v.Equals(other), false);

        /// <summary>
        /// Get the contained value of a Maybe polymorphic type if it is
        /// present, or <paramref name="defaultValue"/> otherwise.
        /// </summary>
        /// <typeparam name="T">Contained type</typeparam>
        public static T GetOrElse<T>(this Maybe<T> maybe, T defaultValue) => maybe.HasValue ? maybe.Value : defaultValue;

        /// <summary>
        /// Get the contained value of a Maybe polymorphic type if it is
        /// present, or the result of applying <paramref name="nothingFunc"/>
        /// otherwise.
        /// </summary>
        /// <typeparam name="T">Contained type</typeparam>
        public static T GetOrElse<T>(this Maybe<T> maybe, Func<T> nothingFunc) => maybe.HasValue ? maybe.Value : nothingFunc();

        /// <summary>
        /// Get a Maybe type out of a definite value.
        /// </summary>
        /// <typeparam name="T">The type of the definite value</typeparam>
        public static Just<T> Just<T>(T value) => new Just<T>(value);

        /// <summary>
        /// Map a Maybe of a discriminated union into a Maybe of some type
        /// </summary>
        public static Maybe<T> Map<T, EA, EB>(this Maybe<Either<EA, EB>> maybe, Func<EA, T> mapA, Func<EB, T> mapB) =>
            maybe.Map(either => either.Map(mapA, mapB));

        /// <summary>
        /// Map a Maybe of a discriminated union into a Maybe of some type
        /// </summary>
        public static Maybe<T> Map<T, EA, EB, EC>(
            this Maybe<Either<EA, EB, EC>> maybe, Func<EA, T> mapA, Func<EB, T> mapB, Func<EC, T> mapC) =>
            maybe.Map(either => either.Map(mapA, mapB, mapC));

        /// <summary>
        /// Converts a Maybe(<paramref name="T"/>) to a <paramref name="T"/>?
        /// nullable object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <returns></returns>
        public static T? ToNullable<T>(this Maybe<T> from) where T : struct =>
            from.HasValue ? from.Value : (T?)null;
    }

}
