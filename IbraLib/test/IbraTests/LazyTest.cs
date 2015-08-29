using Ibra.Lazy;

using Xunit;

namespace IbraTests
{
    public static class LazyTest
    {
        [Fact]
        public static void LazyFunc_OneToOne_Simple()
        {
            LazyFunc<int, int> lazy = new LazyFunc<int, int>(x => x * 2);

            Assert.Equal(8, lazy[4]);
            Assert.Equal(8, lazy[4]);
            Assert.Equal(10, lazy[5]);
            Assert.Equal(16, lazy[8]);
            Assert.Equal(22, lazy[11]);
            Assert.Equal(0, lazy[0]);
            Assert.Equal(8, lazy[4]);
            Assert.Equal(-2, lazy[-1]);
        }

        [Fact]
        public static void LazyFunc_OneToOne_ActuallyLazy()
        {
            int counter = 0;
            LazyFunc<int, int> lazy = new LazyFunc<int, int>(x => // here's an impure function for ya
            {
                counter++;
                return x * x;
            });

            Assert.Equal(16, lazy[4]);
            Assert.Equal(1, counter);

            Assert.Equal(16, lazy[4]);
            Assert.Equal(1, counter);

            Assert.Equal(25, lazy[5]);
            Assert.Equal(2, counter);

            Assert.Equal(64, lazy[8]);
            Assert.Equal(3, counter);

            Assert.Equal(121, lazy[11]);
            Assert.Equal(4, counter);

            Assert.Equal(0, lazy[0]);
            Assert.Equal(5, counter);

            Assert.Equal(16, lazy[4]);
            Assert.Equal(5, counter);

            Assert.Equal(1, lazy[-1]);
            Assert.Equal(6, counter);

        }

        [Fact]
        public static void LazyFunc_OneToOne_RecursiveSucceeds()
        {
            LazyFunc<int, int> fibonacci = null;
            fibonacci = new LazyFunc<int, int>(x =>
            {
                if (x <= 2) return 1;
                return fibonacci[x - 1] + fibonacci[x - 2];
            });

            Assert.Equal(fibonacci[19], 4181);
            Assert.Equal(fibonacci[20], 6765);
            Assert.Equal(fibonacci[11], 89);
            Assert.Equal(fibonacci[7], 13);
        }

        [Fact]
        public static void LazyFunc_OneToOne_RecursiveFails()
        {
            LazyFunc<int, int> recurse = null;
            recurse = new LazyFunc<int, int>(x =>
            {
                return recurse[x];
            });

            Assert.Throws<System.InvalidOperationException>(() => recurse[5]);
            Assert.Throws<System.InvalidOperationException>(() => recurse[6]);
            Assert.Throws<System.InvalidOperationException>(() => recurse[5]);
        }

        [Fact]
        public static void LazyFunc_ManyInputs_Simple()
        {
            LazyFunc<int, int, int> adder = new LazyFunc<int, int, int>((a, b) => a + b);

            Assert.Equal(adder.Get(0, 0), 0);
            Assert.Equal(adder.Get(1, 0), 1);
            Assert.Equal(adder.Get(0, 1), 1);
            Assert.Equal(adder.Get(5, 3), 8);
            Assert.Equal(adder.Get(3, 5), 8);
        }

        [Fact]
        public static void LazyFunc_ManyInputs_SafeRecursive()
        {
            LazyFunc<int, int, int> adder = null;
            adder = new LazyFunc<int, int, int>((a, b) => 
            {
                if (a < b) return adder.Get(b, a);
                return a + b;
            });

            Assert.Equal(adder.Get(0, 0), 0);
            Assert.Equal(adder.Get(1, 0), 1);
            Assert.Equal(adder.Get(0, 1), 1);
            Assert.Equal(adder.Get(5, 3), 8);
            Assert.Equal(adder.Get(3, 5), 8);
        }

        [Fact]
        public static void LazyFunc_ManyInputs_ThrowingRecursive()
        {
            LazyFunc<int, int, int> adder = null;
            adder = new LazyFunc<int, int, int>((a, b) => adder.Get(b, a));

            Assert.Throws<System.InvalidOperationException>(() => adder.Get(0, 0));
            Assert.Throws<System.InvalidOperationException>(() => adder.Get(1, 0));
            Assert.Throws<System.InvalidOperationException>(() => adder.Get(0, 1));
            Assert.Throws<System.InvalidOperationException>(() => adder.Get(5, 3));
            Assert.Throws<System.InvalidOperationException>(() => adder.Get(3, 5));
        }

        [Fact]
        public static void LazyFunc_CustomComparator_ActuallyLazy()
        {
            int counter = 0;
            LazyFunc<string, string> lazy = new LazyFunc<string, string>(str =>
            {
                ++counter;
                return str.ToUpperInvariant();
            }, System.StringComparer.OrdinalIgnoreCase);

            Assert.Equal("FIRST OUTPUT", lazy["first output"]);
            Assert.Equal(1, counter);

            Assert.Equal("FIRST OUTPUT", lazy["fIRST outPut"]);
            Assert.Equal(1, counter);

            Assert.Equal("SECOND OUTPUT", lazy["SECOND OUTPUT"]);
            Assert.Equal(2, counter);

            Assert.Equal("THIRD", lazy["third"]);
            Assert.Equal(3, counter);

            Assert.Equal("FOURTH", lazy["FOurth"]);
            Assert.Equal(4, counter);

            Assert.Equal("FIFTHO", lazy["fifthO"]);
            Assert.Equal(5, counter);

            Assert.Equal("FIRST OUTPUT", lazy["first ouTput"]);
            Assert.Equal(5, counter);

            Assert.Equal("THIRD", lazy["THiRD"]);
            Assert.Equal(5, counter);

            Assert.Equal("BILLIONTH", lazy["billionth"]);
            Assert.Equal(6, counter);
        }
    }
}
