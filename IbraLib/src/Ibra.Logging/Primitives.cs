namespace Ibra.Logging
{
    public enum Level
    {
        NEVER = 0,
        FATAL = 1,
        ERROR = 2,
        WARNING = 3,
        NOTICE = 4,
        INFO = 5,
        VERBOSE = 6,
        DEBUG = 7,
        TRACE = 8
    }

    public class Source : System.IEquatable<Source>
    {
        private static readonly int g_lastId;

        public readonly string Name;
        internal readonly int Index;
        internal readonly Logger Owner;

        internal Source(string name, Logger owner, int id)
        {
            Name = name;
            Index = id;
            Owner = owner;
        }
        public bool Equals(Source other) => Index == other.Index && object.ReferenceEquals(Owner, other.Owner);
        public override bool Equals(object other)
        {
            Source s = other as Source;
            if (s != null) {
                return Equals(s);
            } else return false;
        }
        public override int GetHashCode() => Index.GetHashCode();
        public override string ToString() => Name;
    }
}
