using System;

namespace Ibra.Logging
{
    public struct LogWriter
    {
        private readonly string _prelude;
        private readonly Level _level;
        private readonly SourceRoutes _routes;
        internal LogWriter(Source source, Level level, string context, SourceRoutes routes)
        {
            _prelude = $"{DateTime.Now,24} {source.Name,15} {level.ToString(),10} {context,30}  ";
            _level = level;
            _routes = routes;
        }
        public LogWriter WriteLine(string message)
        {
            _routes.WriteLine(_prelude, message, _level);
            return this;
        }
    }
}