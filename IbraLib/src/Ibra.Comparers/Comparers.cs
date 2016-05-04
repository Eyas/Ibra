using System.Collections.Generic;

namespace Ibra.Comparers
{
    /// <summary>
    /// Compares two lists according to some comparer.
    /// </summary>
    /// <typeparam name="T">Contained type in the Lists being compared.</typeparam>
    public class OrderedEquator<T> : IEqualityComparer<IReadOnlyList<T>>
    {
        /// <summary>
        /// Creates an OrderedEquator with the Default equality comparer for the elements.
        /// </summary>
        /// <remarks>Will use T.Equals() and T.GetHashCode()</remarks>
        public OrderedEquator()
        {
            _cmp = EqualityComparer<T>.Default;
        }
        /// <summary>
        /// Creates an OrderedEquator with a specified comparer
        /// </summary>
        /// <param name="comparer">
        /// The backing comparer used to compare individual elements of the sequence
        /// for equality.
        /// </param>
        public OrderedEquator(IEqualityComparer<T> comparer)
        {
            _cmp = comparer;
        }
        public bool Equals(IReadOnlyList<T> a, IReadOnlyList<T> b)
        {
            if (a == b) return true;
            if (a == null) return false;
            if (b == null) return false;

            int ca = a.Count, cb = b.Count;
            if (ca != cb) return false;

            for (int i = 0; i < ca; i++)
            {
                if (!_cmp.Equals(a[i], b[i])) return false;
            }
            return true;
        }
        public int GetHashCode(IReadOnlyList<T> x)
        {
            if (x == null) return -1;
            int h = 0;
            foreach (T item in x)
            {
                h = h ^ _cmp.GetHashCode(item);
            }
            return h;
        }
        private readonly IEqualityComparer<T> _cmp;
    }
    /// <summary>
    /// Compares two lists according to some comparer, in a left-padded lexicographic order.
    /// </summary>
    /// <remarks>
    /// Left-padded lexicographic ordering is the ordering used to sort positive decimal
    /// integers. Longer sequences are always greater than shorter sequences. This is unlike
    /// typical (dictionary) lexicographic ordering, where lists of different lengths are
    /// compared based on the values of individual items, assuming each is right-padded with
    /// elements of smaller orderings.
    /// </remarks>
    /// <example>
    /// When comparing lists of integers, we will get:
    /// [1, 2, 3] EQ [ 1, 2, 3]
    /// [0, 2, 3] LT [ 1, 2, 3]
    /// [2, 2, 3] GT [ 1, 2, 3]
    /// [   2, 3] LT [ 1, 2, 3]
    /// [   2, 3] LT [ 0, 2, 3]
    /// [   2, 3] LT [-1, 2, 3]
    /// [1, 2, 3] LT [ 1, 2,-3]
    ///    null   LT [ 1, 2, 3]
    ///    null   EQ    null
    /// </example>
    /// <typeparam name="T">Contained type in the Lists being compared.</typeparam>
    public class OrderedComparer<T> : IComparer<IReadOnlyList<T>>
    {
        public OrderedComparer()
        {
            _cmp = Comparer<T>.Default;
        }
        public OrderedComparer(IComparer<T> comparer)
        {
            _cmp = comparer;
        }
        public int Compare(IReadOnlyList<T> a, IReadOnlyList<T> b)
        {
            if (a == b) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            int ca = a.Count, cb = b.Count;
            if (ca > cb) return 1;
            if (cb > ca) return -1;

            for (int i = 0; i < ca; ++i)
            {
                int r = _cmp.Compare(a[i], b[i]);
                if (r != 0) return r;
            }
            return 0;
        }
        private readonly IComparer<T> _cmp;
    }
    /// <summary>
    /// Compares two lists according to some comparer, in a (right-padded) lexicographic order.
    /// </summary>
    /// <remarks>
    /// Right-padded lexicographic order is also known as 'dictionary ordering' and referred to
    /// simply as 'lexicographic ordering' as well. Sequences of different lengths are compared
    /// element by element. If one sequence is a prefix of the other, the longer sequence comes
    /// after.
    /// </remarks>
    /// <example>
    /// When comparing lists of letters, we will get:
    /// [B, B, C] EQ [B, B, C]
    /// [A, B, C] LT [B, B, C]
    /// [C, B, C] GT [B, B, C]
    /// [B, B   ] LT [B, B, C]
    /// [B, C   ] GT [B, B, C]
    /// [B, A   ] LT [B, B, C]
    ///    null   LT [B, B, C]
    ///    null   EQ   null
    /// </example>
    /// <typeparam name="T">Contained type in the Lists being compared.</typeparam>
    public class LexicographicalComparer<T> : IComparer<IReadOnlyList<T>>
    {
        public LexicographicalComparer()
        {
            _cmp = Comparer<T>.Default;
        }
        public LexicographicalComparer(IComparer<T> comparer)
        {
            _cmp = comparer;
        }
        public int Compare(IReadOnlyList<T> a, IReadOnlyList<T> b)
        {
            if (a == b) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            int ca = a.Count, cb = b.Count;
            int min = (ca <= cb) ? ca : cb;

            for (int i = 0; i < min; ++i)
            {
                int r = _cmp.Compare(a[i], b[i]);
                if (r != 0) return r;
            }

            return
                (ca == cb) ? 0 :
                (ca > cb) ? 1 : -1;
        }
        private readonly IComparer<T> _cmp;
    }
}
