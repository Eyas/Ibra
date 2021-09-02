namespace Ibra.Polymorphic
{
    public sealed class Unit
    {
        public static Unit Instance = new();
        private Unit() { }
    }
}