using System;

namespace Ibra.Polymorphic.Covariant
{
    /// <summary>
    /// A covariant type representing an executed function which is either successful (with a valid result)
    /// or failed (with an Exception indicating the failure).
    /// </summary>
    /// <remarks>
    /// The covariant Try is a non-equatable, non-hashable, heap-allocated reference type. If you require
    /// different storage or usage properties, use the invariant Try type instead.
    /// </remarks>
    public interface Try<out TResult>
    {
        /// <summary>
        /// Executes <paramref name="onSuccess"/> if this <see cref="Covariant.Try{TResult}"/> is
        /// successful, otherwise does nothing.
        /// </summary>
        /// <remarks>
        /// An exception being thrown in <paramref name="onSuccess"/> will result in the
        /// <see cref="Covariant.Try{Unit}"/>  being returned resolving to a <see cref="Failure{Unit}"/>.
        /// </remarks>
        Try<Unit> Then(Action<TResult> onSuccess);

        /// <summary>
        /// Executes <paramref name="onSuccess"/> if this <see cref="Covariant.Try{TResult}"/> is
        /// successful, otherwise executes <paramref name="onFailure"/>.
        /// </summary>
        /// <remarks>
        /// An exception being thrown in <paramref name="onSuccess"/> or <paramref name="onFailure"/>
        /// will result in the <see cref="Covariant.Try{Unit}"/> being returned resolving to a
        /// <see cref="Failure{Unit}"/>.
        /// </remarks>
        Try<Unit> Then(Action<TResult> onSuccess, Action<Exception> onFailure);
        
        /// <summary>
        /// Returns a Try of the result of <paramref name="onSuccess"/> if this Try(T) is successful. Otherwise
        /// returns the same Failure(T).
        /// </summary>
        /// <remarks>
        /// An exception being thrown in <paramref name="onSuccess"/> will result in the Try(<typeparam name="TResult2"/>)
        /// being returned resolving to a Failure(<typeparam name="TResult2" />).
        /// </remarks>
        Try<TResult2> Then<TResult2>(Func<TResult, TResult2> onSuccess);
        
        /// <summary>
        /// Returns a Try of the result of <paramref name="onSuccess"/> if this Try(T) is successful. Otherwise
        /// returns a Try of the result of <paramref name="onFailure"/> applied to the thrown Exception.
        /// </summary>
        /// <remarks>
        /// An exception being thrown in <paramref name="onSuccess"/> or <paramref name="onFailure"/> will result
        /// in the Try being returned resolving to a Failure(<typeparam name="TResult2" />).
        /// </remarks>
        Try<TResult2> Then<TResult2>(Func<TResult, TResult2> onSuccess, Func<Exception, TResult2> onFailure);
        
        /// <summary>
        /// Returns the result of this Try if it is successful, or throws the stored Exception otherwise.
        /// </summary>
        TResult GetOrThrow();
        
        /// <summary>
        /// Returns the reuslt of <paramref name="onSuccess"/> if this Try(T) is successful, or otherwise
        /// the result of <paramref name="onFailure"/>.
        /// </summary>
        /// <remarks>
        /// An exception being thrown in <paramref name="onSuccess"/> or <paramref name="onFailure"/> will perculate
        /// upwards and be thrown immediately when this method is called.
        /// </remarks>
        TResult2 Convert<TResult2>(Func<TResult, TResult2> onSuccess, Func<Exception, TResult2> onFailure);
    }

    /// <summary>
    /// Represents a successful action with a a valid result.
    /// </summary>
    public sealed class Success<TResult> : Try<TResult>
    {
        public Success(TResult result) { Result = result; }
        
        /// <summary>
        /// The successful result of our action.
        /// </summary>
        public TResult Result { get; }

        public TResult2 Convert<TResult2>(Func<TResult, TResult2> onSuccess, Func<Exception, TResult2> onFailure)
            => onSuccess(Result);

        public TResult GetOrThrow() => Result;

        public Try<Unit> Then(Action<TResult> onSuccess) => onSuccess.Try(Result);

        public Try<Unit> Then(Action<TResult> onSuccess, Action<Exception> onFailure) => onSuccess.Try(Result);

        public Try<TResult2> Then<TResult2>(Func<TResult, TResult2> onSuccess)
            => onSuccess.Try(Result);

        public Try<TResult2> Then<TResult2>(Func<TResult, TResult2> onSuccess, Func<Exception, TResult2> onFailure)
            => onSuccess.Try(Result);
    }
    
    /// <summary>
    /// Represents a failed action with an Exception indicating the nature of this failure.
    /// </summary>
    public sealed class Failure<TResult> : Try<TResult>
    {
        public Failure(Exception e) { Exception = e; }
        
        /// <summary>
        /// The error thrown during the execution of the action leading to this failure. This exception
        /// has not been caught by any previous call to `Catch`.
        /// </summary>
        public Exception Exception { get; }

        public TResult2 Convert<TResult2>(Func<TResult, TResult2> onSuccess, Func<Exception, TResult2> onFailure)
            => onFailure(Exception);

        public TResult GetOrThrow() { throw Exception; }

        public Try<Unit> Then(Action<TResult> onSuccess) => new Failure<Unit>(Exception);

        public Try<Unit> Then(Action<TResult> onSuccess, Action<Exception> onFailure) => onFailure.Try(Exception);

        public Try<TResult2> Then<TResult2>(Func<TResult, TResult2> onSuccess)
            => new Failure<TResult2>(Exception);

        public Try<TResult2> Then<TResult2>(Func<TResult, TResult2> onSuccess, Func<Exception, TResult2> onFailure)
            => onFailure.Try(Exception);
    }
    
    public static class TryExtensions {
        /// <summary>
        /// Executes a void function <paramref name="action"/> and returns
        /// <see cref="Covariant.Try{Unit}"/>.
        /// </summary>
        public static Try<Unit> Try(this Action func)
        {
            try
            {
                func();
                return new Success<Unit>(Unit.Instance);
            }
            catch (Exception e)
            {
                return new Failure<Unit>(e);
            }
        }

        /// <summary>
        /// Executes a void function <paramref name="action"/>, which takes one
        /// argument, <paramref name="arg"/>and returns <see cref="Covariant.Try{Unit}"/>.
        /// </summary>
        public static Try<Unit> Try<TArg>(this Action<TArg> func, TArg arg)
        {
            try
            {
                func(arg);
                return new Success<Unit>(Unit.Instance);
            }
            catch (Exception e)
            {
                return new Failure<Unit>(e);
            }
        }

        /// <summary>
        /// Executes a function and returns its result as a `Try(T)`.
        /// </summary>
        public static Try<TResult> Try<TResult>(this Func<TResult> func)
        {
            try
            {
                return new Success<TResult>(func());
            }
            catch (Exception e)
            {
                return new Failure<TResult>(e);
            }
        }
        
        /// <summary>
        /// Executes a function which takes one argument, <paramref name="arg"/>, and returns
        /// its result as a `Try(T)`.
        /// </summary>
        /// <remarks>
        /// While this method can be implemented using <see cref="TryExtensions.Try{TResult}"/>
        /// and creating a closure, this (and other) convinience methods here do away with the closre
        /// by re-implementing the `Try` method.
        /// </remarks>
        public static Try<TResult> Try<TArg, TResult>(this Func<TArg, TResult> func, TArg arg)
        {
            try
            {
                return new Success<TResult>(func(arg));
            }
            catch (Exception e)
            {
                return new Failure<TResult>(e);
            }
        }
        
        /// <summary>
        /// Executes a function which takes arguments, <paramref name="arg1"/> and <paramref name="arg2"/>,
        /// and returns its result as a `Try(T)`.
        /// </summary>
        public static Try<TResult> Try<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> func,
            TArg1 arg1, TArg2 arg2)
        {
            try
            {
                return new Success<TResult>(func(arg1, arg2));
            }
            catch (Exception e)
            {
                return new Failure<TResult>(e);
            }
        }
        
        /// <summary>
        /// Executes a function which takes arguments, <paramref name="arg1"/>, <paramref name="arg2"/>, and
        /// <paramref name="arg3"/>, and returns its result as a `Try(T)`.
        /// </summary>
        public static Try<TResult> Try<TArg1, TArg2, TArg3, TResult>(this Func<TArg1, TArg2, TArg3, TResult> func,
            TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            try
            {
                return new Success<TResult>(func(arg1, arg2, arg3));
            }
            catch (Exception e)
            {
                return new Failure<TResult>(e);
            }
        }
        
        /// <summary>
        /// Executes a function which takes arguments, <paramref name="arg1"/>, <paramref name="arg2"/>,
        /// <paramref name="arg3"/>, and <paramref name="arg4"/>, and returns its result as a `Try(T)`.
        /// </summary>
        public static Try<TResult> Try<TArg1, TArg2, TArg3, TArg4, TResult>(this Func<TArg1, TArg2, TArg3, TArg4, TResult> func,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            try
            {
                return new Success<TResult>(func(arg1, arg2, arg3, arg4));
            }
            catch (Exception e)
            {
                return new Failure<TResult>(e);
            }
        }
        
        /// <summary>
        /// Executes a function which takes arguments, <paramref name="arg1"/>, <paramref name="arg2"/>,
        /// <paramref name="arg3"/>, <paramref name="arg4"/>, and <paramref name="arg5"/> and returns its
        /// result as a `Try(T)`.
        /// </summary>
        public static Try<TResult> Try<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(this Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> func,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            try
            {
                return new Success<TResult>(func(arg1, arg2, arg3, arg4, arg5));
            }
            catch (Exception e)
            {
                return new Failure<TResult>(e);
            }
        }
        
        /// <summary>
        /// Executes a function <paramref name="func" />, catching the any exception of type `TException` thrown and turning
        /// that into a successful argument. If another `Exception` of unknown type was thrown, the result will continue to
        /// be a `Failure`.
        /// </summary>
        public static Try<TResult> TryCatch<TResult, TException>(this Func<TResult> func, Func<TException, TResult> catcher)
            where TException : Exception
        {
            try
            {
                return new Success<TResult>(func());
            }
            catch (TException e)
            {
                // recurse into the `Try` method with one fewer argument,
                // thus creating a new nested "try" here-- an exception
                // thrown during the evaluation of `catcher` should be
                // part of a `Failure<TResult>` being returned.
                return catcher.Try(e);
            }
            catch (Exception e)
            {
                return new Failure<TResult>(e);
            }
        }
        
        /// <seealso cref="TryExtensions.TryCatch{TResult, TException}" />
        public static Try<TResult> TryCatch<TResult, TException1, TException2>(this Func<TResult> func,
            Func<TException1, TResult> catcher1, Func<TException2, TResult> catcher2)
            where TException1 : Exception
            where TException2 : Exception
        {
            try
            {
                return new Success<TResult>(func());
            }
            catch (TException1 e)
            {
                // recurse into `Try` -- see comment in `TryCatch(func, catcher)` for why this is necessary
                return catcher1.Try(e);
            }
            catch (TException2 e)
            {
                // recurse into `Try` -- see comment in `TryCatch(func, catcher)` for why this is necessary
                return catcher2.Try(e);
            }
            catch (Exception e)
            {
                return new Failure<TResult>(e);
            }
        }
        
        /// <seealso cref="TryExtensions.TryCatch{TResult, TException}" />
        public static Try<TResult> TryCatch<TResult, TException1, TException2, TException3>(this Func<TResult> func,
            Func<TException1, TResult> catcher1, Func<TException2, TResult> catcher2, Func<TException3, TResult> catcher3)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            try
            {
                return new Success<TResult>(func());
            }
            catch (TException1 e)
            {
                // recurse into `Try` -- see comment in `TryCatch(func, catcher)` for why this is necessary
                return catcher1.Try(e);
            }
            catch (TException2 e)
            {
                // recurse into `Try` -- see comment in `TryCatch(func, catcher)` for why this is necessary
                return catcher2.Try(e);
            }
            catch (TException3 e)
            {
                // recurse into `Try` -- see comment in `TryCatch(func, catcher)` for why this is necessary
                return catcher3.Try(e);
            }
            catch (Exception e)
            {
                return new Failure<TResult>(e);
            }
        }
        
        /// <seealso cref="TryExtensions.TryCatch{TResult, TException}" />
        public static Try<TResult> TryCatch<TResult, TException1, TException2, TException3, TException4>(this Func<TResult> func,
            Func<TException1, TResult> catcher1, Func<TException2, TResult> catcher2, Func<TException3, TResult> catcher3, Func<TException4, TResult> catcher4)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
            where TException4 : Exception
        {
            try
            {
                return new Success<TResult>(func());
            }
            catch (TException1 e)
            {
                // recurse into `Try` -- see comment in `TryCatch(func, catcher)` for why this is necessary
                return catcher1.Try(e);
            }
            catch (TException2 e)
            {
                // recurse into `Try` -- see comment in `TryCatch(func, catcher)` for why this is necessary
                return catcher2.Try(e);
            }
            catch (TException3 e)
            {
                // recurse into `Try` -- see comment in `TryCatch(func, catcher)` for why this is necessary
                return catcher3.Try(e);
            }
            catch (TException4 e)
            {
                // recurse into `Try` -- see comment in `TryCatch(func, catcher)` for why this is necessary
                return catcher4.Try(e);
            }
            catch (Exception e)
            {
                return new Failure<TResult>(e);
            }
        }
        
        /// <summary>
        /// If a <see cref="Covariant.Try{TResult}"/> is a <see cref="Failre{TResult}"/> whose inner Exception
        /// if of type <typeparamref name="TException"/> , catch the result and turn that into a <see cref="Covariant.Try{TResult}"/>
        /// with the value of <paramref name="catcher"/> (if it succeeds).
        /// </summary>
        /// <remarks>
        /// - If our `Try` is successful, this method simply returns the same `Try` object.
        /// - If our `Try` is failed, with an exception that is not a sub-type of `TException`, this method simply returns
        ///   the same failed `Try` object.
        /// - If our `Try` is failed, with an exception that *is* `TException`, this method returns either:
        ///   (a) a successful `Try` object with the value of <paramref name="catcher"/> applied to the exception, if
        ///       the function is evaluated successfully, or
        ///   (b) a new failed `Try` object, with the new exception thrown during the evaluation of <paramref name="catcher"/>.
        /// </remarks>
        public static Try<TResult> Catch<TResult, TException>(this Try<TResult> tryer, Func<TException, TResult> catcher)
            where TException : Exception
        {
            if (tryer is Failure<TResult> f && f.Exception is TException e) return catcher.Try(e);

            return tryer;
        }
    }
}
