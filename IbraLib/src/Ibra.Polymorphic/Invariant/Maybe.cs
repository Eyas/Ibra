using System;

namespace Ibra.Polymorphic.Invariant
{
    /// <summary>
    /// An Invariant optional type that represents a value that may or may not be present.
    /// </summary>
    /// <remarks>
    /// The invariant Maybe(T) is impemented a value type. This means that it cannot be
    /// null. default(Maybe(<typeparamref name="T"/>)) represents a 'nothing' value.
    /// Maybe(<typeparamref name="T"/>).Nothing also represents the 'nothing' value for
    /// convinience.
    /// 
    /// As a value type, Maybe(T) is stack allocated. This is beneficial for performance
    /// reasons and heap allocations increase GC pressure which is especially harmful for
    /// multi-threaded code. One disadvantage of being stack allocated is that a Maybe(T)
    /// will take up the same space in memory whether or not it contains a valid value.
    /// This is inconsequential for reference types and small structs but could be more
    /// problematic if the Maybe(T) refers to a large struct type.
    /// 
    /// As an invariant type, Maybe(T) cannot be used covariantly. Use the Maybe.Vary
    /// extension method to convert a Maybe(T) to a Maybe(Super Type).
    /// </remarks>
    /// <typeparam name="T">The type that could be held by this instnace.</typeparam>
    public struct Maybe<T>
    {
        /// <summary>
        /// Represents a Maybe(<typeparamref name="T"/>) with no value.
        /// </summary>
        public static readonly Maybe<T> Nothing = default(Maybe<T>);

        /// <summary>
        /// Constructs a Maybe(<typeparamref name="T"/>) holding <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to be held by this instance</param>
        public Maybe(T value)
        {
            _hasValue = true;
            _value = value;
        }

        /// <summary>
        /// Indicates whether or not a value is present in this instance
        /// </summary>
        public bool HasValue => _hasValue;

        /// <summary>
        /// If HasValue is true, provides the value, else throws NotSupportedException
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If attempting to access a value when no value is present.
        /// </exception>
        /// <remarks>
        /// Access to Value must be restricted to logic that is conditional on HasValue
        /// </remarks>
        public T Value
        {
            get
            {
                if (!_hasValue) throw new NotSupportedException($"{nameof(Value)} can only be asked for if a value exists.");
                return _value;
            }
        }

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
        /// if a value is present, or Nothing otherwise.
        /// </returns>
        public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper) =>
            _hasValue ? new Maybe<TResult>(mapper(_value)) : Maybe<TResult>.Nothing;

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
        /// if a value is present, or Nothing otherwise.
        /// </returns>
        public Maybe<TResult> FlatMap<TResult>(Func<T, Maybe<TResult>> mapper) =>
            _hasValue ? mapper(_value) : Maybe<TResult>.Nothing;

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
        public TResult Convert<TResult>(Func<T, TResult> justFunc, Func<TResult> nothingFunc) =>
            _hasValue ? justFunc(_value) : nothingFunc();

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
        public TResult Convert<TResult>(Func<T, TResult> justFunc, TResult nothing) =>
            _hasValue ? justFunc(_value) : nothing;

        /// <summary>
        /// Apply a predicate to the current value and return a Maybe which
        /// holds a value if this previously held a value and the predicate
        /// matches that value.
        /// </summary>
        /// <param name="filter">
        /// The predicate to be matched against the value.
        /// </param>
        /// <returns>
        /// A Maybe(<typeparamref name="T"/>). Either a Maybe(<typeparamref name="T"/>)
        /// containing a value if and only if this contains a value and the predicate
        /// matches Value, or otherwise Nothing.
        /// </returns>
        Maybe<T> Filter(Predicate<T> filter) =>
            filter(_value) ? this : Nothing;

        /// <summary>
        /// Get the contained value of a Maybe polymorphic type if it is
        /// present, or <paramref name="defaultValue"/> otherwise.
        /// </summary>
        /// <typeparam name="T">Contained type</typeparam>
        public T GetOrElse(T defaultValue) => _hasValue ? _value : defaultValue;

        /// <summary>
        /// Get the contained value of a Maybe polymorphic type if it is
        /// present, or the result of applying <paramref name="nothingFunc"/>
        /// otherwise.
        /// </summary>
        /// <typeparam name="T">Contained type</typeparam>
        public T GetOrElse(Func<T> nothingFunc) => _hasValue ? _value : nothingFunc();

        private readonly bool _hasValue;
        private readonly T _value;
    }

    public static class Maybe
    {
        /// <summary>
        /// Get a Maybe type out of a definite value.
        /// </summary>
        /// <typeparam name="T">The type of the definite value</typeparam>
        public static Maybe<T> Just<T>(T value) => new Maybe<T>(value);

        /// <summary>
        /// Converts a Maybe of a subtype into a Maybe of a supertype
        /// </summary>
        public static Maybe<TTo> Vary<TFrom, TTo>(this Maybe<TFrom> from) where TFrom : TTo =>
            from.HasValue
            ? new Maybe<TTo>(from.Value)
            : Maybe<TTo>.Nothing;

    }
}
