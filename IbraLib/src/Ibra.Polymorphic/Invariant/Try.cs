using System;

namespace Ibra.Polymorphic.Invariant
{
    public struct Try<TResult> where TResult : notnull
    {
        private readonly Exception? _exception;
        private readonly TResult? _success;

        internal Try(TResult success)
        {
            _exception = null;
            _success = success;
        }

        internal Try(Exception failure)
        {
            _exception = failure;
            _success = default;
        }

        /// <summary>
        /// Returns the result of this Try if it is successful, or throws the stored Exception otherwise.
        /// </summary>
        public TResult GetOrThrow()
        {
            if (_exception != null) throw _exception;
            return _success!;
        }

        /// <summary>
        /// Returns the reuslt of <paramref name="onSuccess"/> if this <see cref="Invariant.Try{TResult}"/>
        /// is successful, or otherwise the result of <paramref name="onFailure"/>.
        /// </summary>
        /// <remarks>
        /// An exception being thrown in <paramref name="onSuccess"/> or <paramref name="onFailure"/> will perculate
        /// upwards and be thrown immediately when this method is called.
        /// </remarks>
        public TResult2 Convert<TResult2>(Func<TResult, TResult2> onSuccess, Func<Exception, TResult2> onFailure)
        {
            if (_exception != null) return onFailure(_exception);
            else return onSuccess(_success!);
        }

        /// <summary>
        /// If a <see cref="Invariant.Try{TResult}"/> is a failure whose inner Exception
        /// if of type <typeparamref name="TException"/> , catch the result and turn that
        /// into a <see cref="Invariant.Try{TResult}"/> with the value of
        /// <paramref name="catcher"/> (if it succeeds).
        /// </summary>
        /// <remarks>
        /// - If our `Try` is successful, this method simply returns the same `Try` object.
        /// - If our `Try` is failed, with an exception that is not a sub-type of `TException`,
        ///   this method simply returns the same failed `Try` object.
        /// - If our `Try` is failed, with an exception that *is* `TException`, this method
        ///   returns either:
        ///   (a) a successful `Try` object with the value of <paramref name="catcher"/>
        ///       applied to the exception, if the function is evaluated successfully, or
        ///   (b) a new failed `Try` object, with the new exception thrown during the evaluation
        ///       of <paramref name="catcher"/>.
        /// </remarks>
        public Try<TResult> Catch<TException>(Func<TException, TResult> catcher)
            where TException : Exception
        {
            if (_exception is TException te) return catcher.Try(te);
            return this;
        }

        /// <summary>
        /// Executes <paramref name="onSuccess"/> if this <see cref="Invariant.Try{TResult}"/> is
        /// successful, otherwise does nothing.
        /// </summary>
        /// <remarks>
        /// An exception being thrown in <paramref name="onSuccess"/> will result in the
        /// <see cref="Invariant.Try{Unit}"/>  being returned containing an Exception.
        /// </remarks>
        public Try<Unit> Then(Action<TResult> onSuccess)
        {
            if (_exception != null) return new Try<Unit>(_exception);
            return onSuccess.Try(_success!);
        }

        /// <summary>
        /// Executes <paramref name="onSuccess"/> if this <see cref="Invariant.Try{TResult}"/> is
        /// successful, otherwise executes <paramref name="onFailure"/>.
        /// </summary>
        /// <remarks>
        /// An exception being thrown in <paramref name="onSuccess"/> or <paramref name="onFailure"/>
        /// will result in the <see cref="Invariant.Try{Unit}"/> being returned containing an Exception.
        /// </remarks>
        public Try<Unit> Then(Action<TResult> onSuccess, Action<Exception> onFailure)
        {
            if (_exception != null) return onFailure.Try(_exception);
            else return onSuccess.Try(_success!);
        }

        /// <summary>
        /// Returns a Try of the result of <paramref name="onSuccess"/> if this
        /// <see cref="Invariant.Try{TResult}"/> is successful. Otherwise returns
        /// a <see cref="Invariant.Try{TResult2}"/> with an exception.
        /// </summary>
        /// <remarks>
        /// An exception being thrown in <paramref name="onSuccess"/> will result
        /// in the Try(<typeparam name="TResult2"/>) being returned containing an
        /// exception.
        /// </remarks>
        public Try<TResult2> Then<TResult2>(Func<TResult, TResult2> onSuccess) where TResult2 : notnull
        {
            if (_exception != null) return new Try<TResult2>(_exception);
            else return onSuccess.Try(_success!);
        }

        /// <summary>
        /// Returns a Try of the result of <paramref name="onSuccess"/> if this
        /// <see cref="Invariant.Try{TResult}"/> is successful. Otherwise returns
        /// a Try of the result of <paramref name="onFailure"/> applied to the thrown
        /// Exception.
        /// </summary>
        /// <remarks>
        /// An exception being thrown in <paramref name="onSuccess"/> or
        /// <paramref name="onFailure"/> will result in the Try being returned containing
        /// an exception.
        /// </remarks>
        public Try<TResult2> Then<TResult2>(Func<TResult, TResult2> onSuccess, Func<Exception, TResult2> onFailure) where TResult2 : notnull
        {
            if (_exception != null) return onFailure.Try(_exception);
            else return onSuccess.Try(_success!);
        }
    }

    public static class TryExtensions
    {
        /// <summary>
        /// Executes a void function <paramref name="action"/> and returns
        /// <see cref="Invariant.Try{Unit}"/>.
        /// </summary>
        public static Try<Unit> Try(this Action action)
        {
            try
            {
                action();
                return new Try<Unit>(Unit.Instance);
            }
            catch (Exception e)
            {
                return new Try<Unit>(e);
            }
        }

        /// <summary>
        /// Executes a void function <paramref name="action"/>, which takes one
        /// argument, <paramref name="arg"/>and returns <see cref="Invariant.Try{Unit}"/>.
        /// </summary>
        public static Try<Unit> Try<TArg>(this Action<TArg> action, TArg arg)
        {
            try
            {
                action(arg);
                return new Try<Unit>(Unit.Instance);
            }
            catch (Exception e)
            {
                return new Try<Unit>(e);
            }
        }


        /// <summary>
        /// Executes a function and returns its result as a <see cref="Invariant.Try{TResult}"/>.
        /// </summary>
        public static Try<TResult> Try<TResult>(this Func<TResult> function) where TResult : notnull
        {
            try
            {
                return new Try<TResult>(function());
            }
            catch (Exception e)
            {
                return new Try<TResult>(e);
            }
        }

        /// <summary>
        /// Executes a function which takes one argument, <paramref name="arg"/>, and returns
        /// its result as a <see cref="Invariant.Try{TResult}"/>.
        /// </summary>
        /// <remarks>
        /// While this method can be implemented using <see cref="TryExtensions.Try{TResult}"/>
        /// and creating a closure, this (and ther) convinience methods here do away with the closre
        /// by re-implementing the `Try` method.
        /// </remarks>
        public static Try<TResult> Try<TArg, TResult>(this Func<TArg, TResult> function, TArg arg) where TArg : notnull where TResult : notnull
        {
            try
            {
                return new Try<TResult>(function(arg));
            }
            catch (Exception e)
            {
                return new Try<TResult>(e);
            }
        }

        /// <summary>
        /// Executes a function which takes arguments, <paramref name="arg1"/> and <paramref name="arg2"/>,
        /// and returns its result as a <see cref="Invariant.Try{TResult}"/>.
        /// </summary>
        public static Try<TResult> Try<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> function, TArg1 arg1, TArg2 arg2)
            where TArg1 : notnull
            where TArg2 : notnull
            where TResult : notnull
        {
            try
            {
                return new Try<TResult>(function(arg1, arg2));
            }
            catch (Exception e)
            {
                return new Try<TResult>(e);
            }
        }

        /// <summary>
        /// Executes a function which takes arguments, <paramref name="arg1"/>, <paramref name="arg2"/>, and
        /// <paramref name="arg3"/>, and returns its result as a <see cref="Invariant.Try{TResult}"/>.
        /// </summary>
        public static Try<TResult> Try<TArg1, TArg2, TArg3, TResult>(
            this Func<TArg1, TArg2, TArg3, TResult> function,
            TArg1 arg1, TArg2 arg2, TArg3 arg3)
            where TArg1 : notnull
            where TArg2 : notnull
            where TArg3 : notnull
            where TResult : notnull
        {
            try
            {
                return new Try<TResult>(function(arg1, arg2, arg3));
            }
            catch (Exception e)
            {
                return new Try<TResult>(e);
            }
        }

        /// <summary>
        /// Executes a function which takes arguments, <paramref name="arg1"/>, <paramref name="arg2"/>,
        /// <paramref name="arg3"/>, and <paramref name="arg4"/>, and returns its result as a <see cref="Invariant.Try{TResult}"/>.
        /// </summary>
        public static Try<TResult> Try<TArg1, TArg2, TArg3, TArg4, TResult>(
            this Func<TArg1, TArg2, TArg3, TArg4, TResult> function,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
            where TArg1 : notnull
            where TArg2 : notnull
            where TArg3 : notnull
            where TArg4 : notnull
            where TResult : notnull
        {
            try
            {
                return new Try<TResult>(function(arg1, arg2, arg3, arg4));
            }
            catch (Exception e)
            {
                return new Try<TResult>(e);
            }
        }

        /// <summary>
        /// Executes a function which takes arguments, <paramref name="arg1"/>, <paramref name="arg2"/>,
        /// <paramref name="arg3"/>, <paramref name="arg4"/>, and <paramref name="arg5"/> and returns its
        /// result as a <see cref="Invariant.Try{TResult}"/>.
        /// </summary>
        public static Try<TResult> Try<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
            this Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> function,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
            where TArg1 : notnull
            where TArg2 : notnull
            where TArg3 : notnull
            where TArg4 : notnull
            where TArg5 : notnull
            where TResult : notnull
        {
            try
            {
                return new Try<TResult>(function(arg1, arg2, arg3, arg4, arg5));
            }
            catch (Exception e)
            {
                return new Try<TResult>(e);
            }
        }

        /// <summary>
        /// Executes a function <paramref name="func" />, catching the any exception of type `TException` thrown and turning
        /// that into a successful argument. If another `Exception` of unknown type was thrown, the result will continue to
        /// be a `Failure`.
        /// </summary>
        public static Try<TResult> TryCatch<TResult, TException>(this Func<TResult> func, Func<TException, TResult> catcher)
            where TResult : notnull
            where TException : Exception
        {
            try
            {
                return new Try<TResult>(func());
            }
            catch (TException e)
            {
                return catcher.Try(e);
            }
            catch (Exception e)
            {
                return new Try<TResult>(e);
            }
        }

        /// <seealso cref="TryExtensions.TryCatch{TResult, TException}" />
        public static Try<TResult> TryCatch<TResult, TException1, TException2>(
            this Func<TResult> func, Func<TException1, TResult> catcher1, Func<TException2, TResult> catcher2)
            where TResult : notnull
            where TException1 : Exception
            where TException2 : Exception
        {
            try
            {
                return new Try<TResult>(func());
            }
            catch (TException1 e)
            {
                return catcher1.Try(e);
            }
            catch (TException2 e)
            {
                return catcher2.Try(e);
            }
            catch (Exception e)
            {
                return new Try<TResult>(e);
            }
        }

        /// <seealso cref="TryExtensions.TryCatch{TResult, TException}" />
        public static Try<TResult> TryCatch<TResult, TException1, TException2, TException3>(
            this Func<TResult> func, Func<TException1, TResult> catcher1, Func<TException2, TResult> catcher2, Func<TException3, TResult> catcher3)
            where TResult : notnull
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            try
            {
                return new Try<TResult>(func());
            }
            catch (TException1 e)
            {
                return catcher1.Try(e);
            }
            catch (TException2 e)
            {
                return catcher2.Try(e);
            }
            catch (TException3 e)
            {
                return catcher3.Try(e);
            }
            catch (Exception e)
            {
                return new Try<TResult>(e);
            }
        }

        /// <seealso cref="TryExtensions.TryCatch{TResult, TException}" />
        public static Try<TResult> TryCatch<TResult, TException1, TException2, TException3, TException4>(
            this Func<TResult> func, Func<TException1, TResult> catcher1, Func<TException2, TResult> catcher2, Func<TException3, TResult> catcher3, Func<TException4, TResult> catcher4)
            where TResult : notnull
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
            where TException4 : Exception
        {
            try
            {
                return new Try<TResult>(func());
            }
            catch (TException1 e)
            {
                return catcher1.Try(e);
            }
            catch (TException2 e)
            {
                return catcher2.Try(e);
            }
            catch (TException3 e)
            {
                return catcher3.Try(e);
            }
            catch (TException4 e)
            {
                return catcher4.Try(e);
            }
            catch (Exception e)
            {
                return new Try<TResult>(e);
            }
        }

    }
}
