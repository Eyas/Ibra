using Ibra.Comparers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IbraTests
{
    public class CharacterOrdinalIgnoreCase : IComparer<char>
    {
        public int Compare(char x, char y)
        {
            return (char.ToUpperInvariant(x) - char.ToUpperInvariant(y));
        }
    }

    public static class LexicographicalComparerTest
    {
        [Theory]
        [InlineData("Alphabet", "Alphabetical")]
        [InlineData("Alphabet", "Bob")]
        [InlineData("Alphabet", "Alphabets")]
        [InlineData("Alphabet", "Arc")]
        [InlineData("Alphabet", "Archaeology")]
        [InlineData("Alphabet", "alphabetical")]
        [InlineData("alphabet", "Alphabetical")]
        [InlineData("alphabet", "Bob")]
        public static void String_LessThanWorks(string a, string b)
        {
            IReadOnlyList<char> A = a.ToList();
            IReadOnlyList<char> B = b.ToList();

            LexicographicalComparer<char> comparer = new(new CharacterOrdinalIgnoreCase());
            Assert.True(comparer.Compare(A, B) < 0);
            Assert.True(comparer.Compare(B, A) > 0);
        }

        [Theory]
        [InlineData("Alphabet", "Alphabet")]
        [InlineData("alphabet", "Alphabet")]
        [InlineData("Alphabet", "alphabet")]
        [InlineData("Bob", "Bob")]
        [InlineData("Alphabets", "Alphabets")]
        [InlineData("Arc", "Arc")]
        [InlineData("Archaeology", "Archaeology")]
        [InlineData("Archaeology", "ArchaeologY")]
        [InlineData("archaeology", "archaeologY")]
        public static void String_EqualWorks(string a, string b)
        {
            IReadOnlyList<char> A = a.ToList();
            IReadOnlyList<char> B = b.ToList();

            LexicographicalComparer<char> comparer = new(new CharacterOrdinalIgnoreCase());
            Assert.True(comparer.Compare(A, B) == 0);
        }

        [Fact]
        public static void String_SortingWorks()
        {
            IReadOnlyList<char> A = "Alphabet".ToList();
            IReadOnlyList<char> B = "Alphabets".ToList();
            IReadOnlyList<char> C = "Alphabetical".ToList();
            IReadOnlyList<char> D = "Alphabetically".ToList();
            IReadOnlyList<char> E = "Alphanumeric".ToList();
            IReadOnlyList<char> F = "Alphanumerical".ToList();
            IReadOnlyList<char> G = "Bob".ToList();
            IReadOnlyList<char> H = "Abracadabra".ToList();

            List<IReadOnlyList<char>> words = new(new IReadOnlyList<char>[] { A, B, C, D, E, F, G, H });

            words.Sort(new LexicographicalComparer<char>(new CharacterOrdinalIgnoreCase()));

            Assert.Equal(H, words[0]);
            Assert.Equal(A, words[1]);
            Assert.Equal(C, words[2]);
            Assert.Equal(D, words[3]);
            Assert.Equal(B, words[4]);
            Assert.Equal(E, words[5]);
            Assert.Equal(F, words[6]);
            Assert.Equal(G, words[7]);
        }

    }
}
