using System;

namespace Ibra.Logging.Exceptions
{
    /// <summary>
    /// Exception type logged on construction.
    /// </summary>
    public class LoggedException : Exception
    {
        /// <summary>
        /// Constructs a <see cref="LoggedException"/> with no message. Logs
        /// that an exception of a certain Type has occurred.
        /// </summary>
        public LoggedException(
            Source logSource,
            Logger logger,
            [System.Runtime.CompilerServices.CallerMemberName] string caller = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) :
            base()
        {
            logger.Log(logSource, Level.ERROR, caller, line)?.WriteLine($"{GetType().Name} thrown.");
        }

        /// <summary>
        /// Constructs a <see cref="LoggedException"/> with a message. Logs the exception
        /// type and message.
        /// </summary>
        public LoggedException(
            string what,
            Source logSource,
            Logger logger,
            [System.Runtime.CompilerServices.CallerMemberName] string caller = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) :
            base(what)
        {
            logger.Log(logSource, Level.ERROR, caller, line)?.WriteLine($"{GetType().Name}: {what}");
        }

        /// <summary>
        /// Constructs a <see cref="LoggedException"/> with a message and
        /// <paramref name="inner"/> <see cref="Exception"/>. Logs the exception
        /// type and message, and the inner exception.
        /// </summary>
        public LoggedException(
            string what,
            Exception inner,
            Source logSource,
            Logger logger,
            [System.Runtime.CompilerServices.CallerMemberName] string caller = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) :
            base(what, inner)
        {
            logger.Log(logSource, Level.ERROR, caller, line)
                ?.WriteLine($"{GetType().Name}: {what}")
                .WriteLine($"\tInner Exception: {inner.GetType().Name}: {inner.Message}.")
                .WriteLine(inner.StackTrace);
        }
    }
}
