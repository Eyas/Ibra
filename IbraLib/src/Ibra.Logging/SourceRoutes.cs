using System.IO;
using System.Collections.Generic;

namespace Ibra.Logging
{
    internal class SourceRoutes
    {
        private readonly Dictionary<TextWriter, Level> _writers = new Dictionary<TextWriter, Level>();

        public SourceRoutes() { }
        public Level MaxVerbosity { get; private set; } = Level.NEVER;
        public void AddOutput(TextWriter writer, Level level)
        {
            _writers.TryGetValue(writer, out Level existing);

            if (level > existing)
            {
                _writers[writer] = level;
            }

            if (level > MaxVerbosity)
            {
                MaxVerbosity = level;
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
