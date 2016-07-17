using System.IO;
using System.Collections.Generic;

namespace Ibra.Logging
{
    /// defines a set of logging destinations and functions that can be used in a static context.
    /// Typically intended to be used with a
    ///     using static Ibra.Logging.StaticLogger;
    /// directive. This allows the user to call:
    ///    var src = RegisterSource(...);
    ///    Log(src, Level.INFO)
    ///      ?.WriteLine(xxx)
    ///       .WriteLine(yyy);
    /// instead of using a the "Log" member function on a specific instance of the Ibra.Logging.Logger
    /// class.
    public static class StaticLogger
    {
        private static System.Lazy<Logger> _staticLogger = new System.Lazy<Logger>();
        public static Source RegisterSource(string name) => _staticLogger.Value.RegisterSource(name);
        public static void AddSource(Source source, Level level, TextWriter destination)
        {
            _staticLogger.Value.AddSource(source, level, destination);
        }
        public static LogWriter? Log(
            Source source, Level level,
            [System.Runtime.CompilerServices.CallerMemberName] string context = "")
        => _staticLogger.Value.Log(source, level, context);
    }

    public class Logger
    {

        private List<SourceRoutes> _logLevels = new List<SourceRoutes>();
        private Dictionary<string, Source> _logSources = new Dictionary<string, Source>();

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

            SourceRoutes routes = _logLevels[source.Index];
            routes.AddOutput(destination, level);
        }

        public LogWriter? Log(
            Source source, Level level,
            [System.Runtime.CompilerServices.CallerMemberName] string context = "")
        {
            if (source.Owner != this || source.Index >= _logLevels.Count)
                throw new LoggingException($"Source {source} not recognized.");

            SourceRoutes routes = _logLevels[source.Index];
            if (routes.MaxVerbosity >= level)
            {
                return new LogWriter(source, level, context, routes);
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
