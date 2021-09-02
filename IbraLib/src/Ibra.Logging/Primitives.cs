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
        public readonly string Name;
        internal readonly int Index;
        internal readonly Logger Owner;

        internal Source(string name, Logger owner, int id)
        {
            Name = name;
            Index = id;
            Owner = owner;
        }
        public bool Equals(Source? other) => other != null && Index == other.Index && ReferenceEquals(Owner, other.Owner);
        public override bool Equals(object? other) => other is Source s && Equals(s);

        public override int GetHashCode() => Index.GetHashCode();
        public override string ToString() => Name;
    }
}
