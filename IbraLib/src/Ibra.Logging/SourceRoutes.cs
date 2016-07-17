using System.IO;
using System.Collections.Generic;

namespace Ibra.Logging
{
    internal class SourceRoutes
    {
        private Level _maxVerbosity = Level.NEVER;
        private Dictionary<TextWriter, Level> _writers = new Dictionary<TextWriter, Level>();

        public SourceRoutes() { }
        public Level MaxVerbosity => _maxVerbosity;
        public void AddOutput(TextWriter writer, Level level)
        {
            Level existing = Level.NEVER;
            _writers.TryGetValue(writer, out existing);

            if (level > existing)
            {
                _writers[writer] = level;
            }

            if (level > _maxVerbosity)
            {
                _maxVerbosity = level;
            }
        }
        public void WriteLine(string prelude, string text, Level level)
        {
            foreach (var kv in _writers)
            {
                if (kv.Value >= level)
                {
                    kv.Key.Write(prelude);
                    kv.Key.WriteLine(text);
                }
            }
        }
    }
}
