using System.IO;
using System.Collections.Generic;

namespace Ibra.Logging
{
    /// <summary>
    /// Defines a set of logging destinations and functions that can be used in a static context.
    /// </summary>
    /// <example>
    /// Typically intended to be used with a
    ///     using static Ibra.Logging.StaticLogger;
    /// directive. This allows the user to call:
    ///    var src = RegisterSource(...);
    ///    Log(src, Level.INFO)
    ///      ?.WriteLine(xxx)
    ///       .WriteLine(yyy);
    /// instead of using a the "Log" member function on a specific instance of the Ibra.Logging.Logger
    /// class.
    /// </example>
    public static class StaticLogger
    {
        private static System.Lazy<Logger> _staticLogger = new System.Lazy<Logger>();
        public static Source RegisterSource(string name) => _staticLogger.Value.RegisterSource(name);
        public static void AddSource(Source source, Level level, TextWriter destination)
        {
            _staticLogger.Value.AddSource(source, level, destination);
        }

        /// <summary>
        /// If the program is configured to allow the current <paramref name="source"/>
        /// and <paramref name="context"/>, returns a <see cref="LogWriter"/> instance
        /// to be used to write individual log messages. Otherwise returns null.
        /// </summary>
        /// <example>
        /// Log(source, level)?.WriteLine("Foo");
        /// </example>
        /// <param name="context">Passed automatically by the compiler</param>
        /// <param name="line">Passed automatically by the compiler</param>
        public static LogWriter? Log(
            Source source, Level level,
            [System.Runtime.CompilerServices.CallerMemberName] string context = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        => _staticLogger.Value.Log(source, level, context, line);
        public static Source ALL => _staticLogger.Value.ALL;

        /// <summary>
        /// Exception type logged to the Static Log on construction. Similar to
        /// <see cref="Exceptions.LoggedException"/> but ses <see cref="StaticLogger"/>.
        /// </summary>
        public class LoggedException : Exceptions.LoggedException
        {
            /// <summary>
            /// Constructs a <see cref="LoggedException"/> with no message. Logs
            /// that an exception of a certain Type has occurred.
            /// </summary>
            public LoggedException(
                Source logSource,
                [System.Runtime.CompilerServices.CallerMemberName] string caller = "",
                [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) :
                base(logSource, _staticLogger.Value, caller, line)
            { }

            /// <summary>
            /// Constructs a <see cref="LoggedException"/> with a message. Logs the exception
            /// type and message.
            /// </summary>
            public LoggedException(
                string what,
                Source logSource,
                [System.Runtime.CompilerServices.CallerMemberName] string caller = "",
                [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) :
                base(what, logSource, _staticLogger.Value, caller, line)
            { }

            /// <summary>
            /// Constructs a <see cref="LoggedException"/> with a message and
            /// <paramref name="inner"/> <see cref="System.Exception"/>. Logs the exception
            /// type and message, and the inner exception.
            /// </summary>
            public LoggedException(
                string what,
                System.Exception inner,
                Source logSource,
                [System.Runtime.CompilerServices.CallerMemberName] string caller = "",
                [System.Runtime.CompilerServices.CallerLineNumber] int line = -1) :
                base(what, inner, logSource, _staticLogger.Value, caller, line)
            { }
        }
    }

    /// <summary>
    /// Top-level object for writing, initializing, and configuring logs.
    /// </summary>
    public class Logger
    {
        /// <summary>A log source applying to all individually registered sources.</summary>
        public readonly Source ALL;

        private readonly List<SourceRoutes> _logLevels;
        private readonly Dictionary<string, Source> _logSources;

        public Logger()
        {
        	ALL = new Source(nameof(ALL), this, -1);
            _logLevels = new List<SourceRoutes>();
        	_logSources = new Dictionary<string, Source>();
        }

        public Source RegisterSource(string name)
        {
            Source source;

            lock (_logSources)
            if (!_logSources.TryGetValue(name, out source))
            {
                int index = _logLevels.Count;
                source = new Source(name, this, index);
                _logSources.Add(name, source);
                _logLevels.Add(new SourceRoutes());
            }

            return source;
        }
        public void AddSource(Source source, Level level, TextWriter destination)
        {
            if (source.Owner != this)
                throw new LoggingException($"Attempting to add unregistered source {source}.");
            if (source.Index >= _logLevels.Count)
                throw new LoggingException($"Source {source} not recognized.");

            if (level <= Level.NEVER) return;

            if (source.Index < 0)
            {
                // wildcard "all" source
                foreach (SourceRoutes routes in _logLevels)
                {
                    routes.AddOutput(destination, level);
                }
            }
            else
            {
                SourceRoutes routes = _logLevels[source.Index];
                routes.AddOutput(destination, level);
            }
        }

        public LogWriter? Log(
            Source source, Level level,
            [System.Runtime.CompilerServices.CallerMemberName] string context = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int line = -1)
        {
            if (source.Owner != this || source.Index >= _logLevels.Count)
                throw new LoggingException($"Source {source} not recognized.");
            if (source.Index < 0)
                throw new LoggingException($"Wildcard source {source} cannot be used to write to log.");

            SourceRoutes routes = _logLevels[source.Index];
            if (routes.MaxVerbosity >= level)
            {
                return new LogWriter(source, level, context, line, routes);
            }
            else return null;
        }
    }

    public class LoggingException : System.Exception
    {
        public LoggingException() { }
        public LoggingException( string message ) : base( message ) { }
        public LoggingException( string message, System.Exception inner ) : base( message, inner ) { }
    }
}
