using Ibra.Comparers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IbraTests
{

    public static class OrderedComparerTest
    {
        [Theory]
        [InlineData("1000000", "10000000")]
        [InlineData("1000000", "1000001")]
        [InlineData("1000000", "9000000")]
        [InlineData("1000000", "9999999")]
        [InlineData("1000000", "12530338")]
        [InlineData("2000005", "2000006")]
        [InlineData("2000005", "2000007")]
        [InlineData("2000005", "19999999")]
        [InlineData("2000005", "9000005")]
        [InlineData("2000005", "10000000")]
        public static void String_LessThanWorks(string a, string b)
        {
            IReadOnlyList<char> A = a.ToList();
            IReadOnlyList<char> B = b.ToList();

            OrderedComparer<char> comparer = new();
            Assert.True(comparer.Compare(A, B) < 0);
            Assert.True(comparer.Compare(B, A) > 0);
        }

        [Theory]
        [InlineData("100000", "100000")]
        [InlineData("999999", "999999")]
        [InlineData("100001", "100001")]
        [InlineData("125303", "125303")]
        [InlineData("1000000", "1000000")]
        public static void String_EqualWorks(string a, string b)
        {
            IReadOnlyList<char> A = a.ToList();
            IReadOnlyList<char> B = b.ToList();

            OrderedComparer<char> comparer = new();
            Assert.True(comparer.Compare(A, B) == 0);
        }

        [Fact]
        public static void String_SortingWorks()
        {
            IReadOnlyList<char> A = "543472".ToList();
            IReadOnlyList<char> B = "576382".ToList();
            IReadOnlyList<char> C = "573832".ToList();
            IReadOnlyList<char> D = "83972".ToList();
            IReadOnlyList<char> E = "9320473".ToList();
            IReadOnlyList<char> F = "1498399".ToList();
            IReadOnlyList<char> G = "10823".ToList();
            IReadOnlyList<char> H = "9383".ToList();

            List<IReadOnlyList<char>> words = new(new IReadOnlyList<char>[] { A, B, C, D, E, F, G, H });

            words.Sort(new OrderedComparer<char>());

            Assert.Equal(H, words[0]);
            Assert.Equal(G, words[1]);
            Assert.Equal(D, words[2]);
            Assert.Equal(A, words[3]);
            Assert.Equal(C, words[4]);
            Assert.Equal(B, words[5]);
            Assert.Equal(F, words[6]);
            Assert.Equal(E, words[7]);
        }

    }
}
