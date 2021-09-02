using Ibra.Polymorphic.Invariant;
using System;
using System.Collections.Generic;
using Xunit;

namespace IbraTests.Polymorphic.Invariant
{
    public class MaybeTests
    {

        [Fact]
        public void Nothing_AlwaysThrows()
        {
            var vt1 = Maybe<int>.Nothing;
            var vt2 = Maybe<KeyValuePair<int, double>>.Nothing;
            var rt1 = Maybe<string>.Nothing;
            var rt2 = Maybe<object>.Nothing;

            Assert.False(vt1.HasValue);
            Assert.False(vt2.HasValue);
            Assert.False(rt1.HasValue);
            Assert.False(rt2.HasValue);

            Assert.Throws<NotSupportedException>(() => vt1.Value);
            Assert.Throws<NotSupportedException>(() => vt2.Value);
            Assert.Throws<NotSupportedException>(() => rt1.Value);
            Assert.Throws<NotSupportedException>(() => rt2.Value);
        }

        [Fact]
        public void Just_AlwaysSucceeds()
        {
            var vt1 = Maybe.Just(5);
            var vt2 = Maybe.Just(new KeyValuePair<int, double>(5, 8.0));
            var rt1 = Maybe.Just("fooo");
            var rt2 = Maybe.Just(new object { });

            Assert.True(vt1.HasValue);
            Assert.True(vt2.HasValue);
            Assert.True(rt1.HasValue);
            Assert.True(rt2.HasValue);

            Assert.Equal(5, vt1.Value);
            Assert.Equal(5, vt2.Value.Key);
            Assert.Equal(8.0, vt2.Value.Value);
            Assert.Equal("fooo", rt1.Value);
            Assert.NotNull(rt2.Value);
        }

        [Fact]
        public void Nothing_Map_HasNoEffect()
        {
            int iiMap(int x) { Assert.True(false, "Expected this is unreached."); return x; }
            string isMap(int x) { Assert.True(false, "Expected this is unreached."); return x.ToString(); }
            object soMap(string x) { Assert.True(false, "Expected this is unreached."); return ""; }

            var iNothing = Maybe<int>.Nothing;
            var sNothing = Maybe<string>.Nothing;

            var iiN = iNothing.Map(iiMap);
            var isN = iNothing.Map(isMap);
            var soN = sNothing.Map(soMap);

            Assert.False(iiN.HasValue);
            Assert.False(isN.HasValue);
            Assert.False(soN.HasValue);

            Assert.Throws<NotSupportedException>(() => iiN.Value);
            Assert.Throws<NotSupportedException>(() => isN.Value);
            Assert.Throws<NotSupportedException>(() => soN.Value);
        }

        [Fact]
        public void Just_Map_HasEffect()
        {
            int iiMap(int x) => x * 2;
            string isMap(int x) => x.ToString();
            object soMap(string x) => x;

            var i1 = Maybe.Just(3);
            var i2 = Maybe.Just(-483);
            var i3 = Maybe.Just(0);

            var s1 = Maybe.Just("fooooo");
            var s2 = Maybe.Just("baaaar");
            var s3 = Maybe.Just("blergh");

            var ii1 = i1.Map(iiMap);
            var ii2 = i2.Map(iiMap);
            var ii3 = i3.Map(iiMap);

            Assert.True(ii1.HasValue); Assert.Equal(6, ii1.Value);
            Assert.True(ii2.HasValue); Assert.Equal(ii2.Value, -966);
            Assert.True(ii3.HasValue); Assert.Equal(0, ii3.Value);

            var is1 = i1.Map(isMap);
            var is2 = i2.Map(isMap);
            var is3 = i3.Map(isMap);

            Assert.True(is1.HasValue); Assert.Equal("3", is1.Value);
            Assert.True(is2.HasValue); Assert.Equal("-483", is2.Value);
            Assert.True(is3.HasValue); Assert.Equal("0", is3.Value);

            var so1 = s1.Map(soMap);
            var so2 = s2.Map(soMap);
            var so3 = s3.Map(soMap);

            Assert.True(so1.HasValue);
            Assert.True(so2.HasValue);
            Assert.True(so3.HasValue);
        }

        [Fact]
        public void Nothing_FlatMap_DoesntEven()
        {
            // when FlatMap'ing a Nothing.. you can't even!
            Maybe<int> iiMap(int x) { Assert.True(false, "Expected this is unreached."); return Maybe.Just(x); }
            Maybe<string> isMap(int x) { Assert.True(false, "Expected this is unreached."); return Maybe.Just(x.ToString()); }
            Maybe<object> soMap(string x) { Assert.True(false, "Expected this is unreached."); return Maybe.Just<object>(""); }

            var iNothing = Maybe<int>.Nothing;
            var sNothing = Maybe<string>.Nothing;

            var iiN = iNothing.Map(iiMap);
            var isN = iNothing.Map(isMap);
            var soN = sNothing.Map(soMap);

            Assert.False(iiN.HasValue);
            Assert.False(isN.HasValue);
            Assert.False(soN.HasValue);

            Assert.Throws<NotSupportedException>(() => iiN.Value);
            Assert.Throws<NotSupportedException>(() => isN.Value);
            Assert.Throws<NotSupportedException>(() => soN.Value);
        }

        [Fact]
        public void Anything_FlatMapWithNothing_CreatesNothing()
        {
            Maybe<int> iiMap(int x) => Maybe<int>.Nothing;
            Maybe<string> isMap(int x) => Maybe<string>.Nothing;
            Maybe<object> soMap(string x) => Maybe<object>.Nothing;

            Maybe<int>[] int_maybes = new Maybe<int>[] { Maybe.Just(0), Maybe.Just(5), Maybe.Just(3282), Maybe.Just(-42), Maybe<int>.Nothing };
            foreach (Maybe<int> m in int_maybes)
            {
                Maybe<int> ii_result = m.FlatMap(iiMap);
                Assert.False(ii_result.HasValue);
                Assert.Throws<NotSupportedException>(() => ii_result.Value);

                Assert.False(ii_result.FlatMap(iiMap).HasValue);
                Assert.False(ii_result.FlatMap(iiMap).FlatMap(iiMap).HasValue);
                Assert.False(ii_result.FlatMap(iiMap).Map(x => x * 2).HasValue);

                Maybe<string> is_result = m.FlatMap(isMap);
                Assert.False(is_result.HasValue);
                Assert.Throws<NotSupportedException>(() => is_result.Value);

                Assert.False(is_result.FlatMap(soMap).HasValue);
                Assert.False(is_result.FlatMap(soMap).Map(o => o.ToString() ?? "").HasValue);
            }
        }

        [Fact]
        public void Nothing_Convert_Sanity()
        {
            Maybe<int> nI = Maybe<int>.Nothing;
            Maybe<string> nS = Maybe<string>.Nothing;
            Maybe<object> nO = Maybe<object>.Nothing;

            Assert.Equal("success", nI.Convert((just) => "fail", () => "success"));
            Assert.Equal("success", nS.Convert((just) => "fail", () => "success"));
            Assert.Equal("success", nO.Convert((just) => "fail", () => "success"));

            Assert.Equal("success", nI.Convert((just) => "fail", "success"));
            Assert.Equal("success", nS.Convert((just) => "fail", "success"));
            Assert.Equal("success", nO.Convert((just) => "fail", "success"));
        }

        [Fact]
        public void Just_Convert_Sanity()
        {
            Maybe<int> i1 = Maybe.Just(5);
            Maybe<int> i2 = Maybe.Just(0);

            Maybe<string> s1 = Maybe.Just("s1");
            Maybe<string> s2 = Maybe.Just("s2");

            Assert.Equal("success", i1.Convert((just) => "success", () => "fail"));
            Assert.Equal("success", i2.Convert((just) => "success", () => "fail"));
            Assert.Equal("success", s1.Convert((just) => "success", () => "fail"));
            Assert.Equal("success", s2.Convert((just) => "success", () => "fail"));

            Assert.Equal("success", i1.Convert((just) => "success", "fail"));
            Assert.Equal("success", i2.Convert((just) => "success", "fail"));
            Assert.Equal("success", s1.Convert((just) => "success", "fail"));
            Assert.Equal("success", s2.Convert((just) => "success", "fail"));

            Assert.Equal("5success", i1.Convert((just) => $"{just}success", () => "fail"));
            Assert.Equal("0success", i2.Convert((just) => $"{just}success", () => "fail"));
            Assert.Equal("s1success", s1.Convert((just) => $"{just}success", () => "fail"));
            Assert.Equal("s2success", s2.Convert((just) => $"{just}success", () => "fail"));

            Assert.Equal("5success", i1.Convert((just) => $"{just}success", "fail"));
            Assert.Equal("0success", i2.Convert((just) => $"{just}success", "fail"));
            Assert.Equal("s1success", s1.Convert((just) => $"{just}success", "fail"));
            Assert.Equal("s2success", s2.Convert((just) => $"{just}success", "fail"));
        }

        [Fact]
        public void Nothing_TrueFilter_DoesntEven()
        {
            static bool cantEven(int o) { Assert.True(false, "Expected unreached"); return true; }

            var nInt = Maybe<int>.Nothing.Filter(cantEven);

            Assert.False(nInt.HasValue);
            Assert.Throws<NotSupportedException>(() => nInt.Value);
        }

        [Fact]
        public void Just_TrueFilter_Doesnt()
        {
            bool noEffect_int(int i) => true;
            bool noEffect_string(string i) => true;

            var mInt = Maybe.Just(150).Filter(noEffect_int);
            var mStr = Maybe.Just("5").Filter(noEffect_string);

            Assert.True(mInt.HasValue);
            Assert.Equal(150, mInt.Value);

            Assert.True(mStr.HasValue);
            Assert.Equal("5", mStr.Value);
        }

        [Fact]
        public void Just_FalseFilter_Filters()
        {
            bool iFilter(int i) => false;
            bool sFilter(string i) => false;

            {
                var nInt = Maybe.Just(5).Filter(iFilter);
                Assert.False(nInt.HasValue);
                Assert.Throws<NotSupportedException>(() => nInt.Value);
            }

            {
                var nInt = Maybe.Just(3).Filter(iFilter);
                Assert.False(nInt.HasValue);
                Assert.Throws<NotSupportedException>(() => nInt.Value);
            }

            {
                var nStr = Maybe.Just("str").Filter(sFilter);
                Assert.False(nStr.HasValue);
                Assert.Throws<NotSupportedException>(() => nStr.Value);
            }

            {
                var nStr = Maybe.Just("").Filter(sFilter);
                Assert.False(nStr.HasValue);
                Assert.Throws<NotSupportedException>(() => nStr.Value);
            }
        }

        [Fact]
        public void Nothing_GetOrElse_Else()
        {
            var nI = Maybe<int>.Nothing;
            var nS = Maybe<string>.Nothing;

            Assert.Equal(5, nI.GetOrElse(5));
            Assert.Equal(0, nI.GetOrElse(0));
            Assert.Equal(5, nI.GetOrElse(() => 5));
            Assert.Equal(0, nI.GetOrElse(() => 0));

            Assert.Equal("5", nS.GetOrElse("5"));
            Assert.Equal("0", nS.GetOrElse("0"));
            Assert.Equal("5", nS.GetOrElse(() => "5"));
            Assert.Equal("0", nS.GetOrElse(() => "0"));
        }

        [Fact]
        public void Just_GetOrElse_Get()
        {
            Assert.Equal(5, Maybe.Just(5).GetOrElse(-1));
            Assert.Equal(15, Maybe.Just(15).GetOrElse(-1));
            Assert.Equal(0, Maybe.Just(0).GetOrElse(-1));
            Assert.Equal(5, Maybe.Just(5).GetOrElse(() => { Assert.False(true, "Expected unreached"); return -1; }));
            Assert.Equal(15, Maybe.Just(15).GetOrElse(() => { Assert.False(true, "Expected unreached"); return -1; }));
            Assert.Equal(0, Maybe.Just(0).GetOrElse(() => { Assert.False(true, "Expected unreached"); return -1; }));

            Assert.Equal("5", Maybe.Just("5").GetOrElse(""));
            Assert.Equal("15", Maybe.Just("15").GetOrElse(""));
            Assert.Equal("0", Maybe.Just("0").GetOrElse(""));
            Assert.Equal("5", Maybe.Just("5").GetOrElse(() => { Assert.False(true, "Expected unreached"); return ""; }));
            Assert.Equal("15", Maybe.Just("15").GetOrElse(() => { Assert.False(true, "Expected unreached"); return ""; }));
            Assert.Equal("0", Maybe.Just("0").GetOrElse(() => { Assert.False(true, "Expected unreached"); return ""; }));
        }

        [Fact]
        public void Equals_Nothing_Nothing_TrueSameType()
        {
            Assert.True(Maybe<string>.Nothing.Equals(Maybe<string>.Nothing));
            Assert.True(Maybe<object>.Nothing.Equals(Maybe<object>.Nothing));
            Assert.True(Maybe<int>.Nothing.Equals(Maybe<int>.Nothing));

            Assert.False(Maybe<string>.Nothing.Equals(Maybe<object>.Nothing));
            Assert.False(Maybe<object>.Nothing.Equals(Maybe<string>.Nothing));
            Assert.False(Maybe<string>.Nothing.Equals(Maybe<int>.Nothing));

            Assert.True(Maybe<string>.Nothing.Equals(Maybe<string>.Nothing as object));
            Assert.True(Maybe<object>.Nothing.Equals(Maybe<object>.Nothing as object));
            Assert.True(Maybe<int>.Nothing.Equals(Maybe<int>.Nothing as object));

            Assert.False(Maybe<int>.Nothing.Equals(null));
        }

        [Fact]
        public void Equals_Nothing_Just_AlwaysFalse()
        {
            Assert.False(Maybe<string>.Nothing.Equals(Maybe.Just("something")));
            Assert.False(Maybe<string>.Nothing.Equals(Maybe.Just("")));
            Assert.False(Maybe<string>.Nothing.Equals(Maybe.Just(5)));
            Assert.False(Maybe<string>.Nothing.Equals(Maybe.Just("") as object));

            Assert.False(Maybe<int>.Nothing.Equals(Maybe.Just("something")));
            Assert.False(Maybe<int>.Nothing.Equals(Maybe.Just("")));
            Assert.False(Maybe<int>.Nothing.Equals(Maybe.Just(5)));
            Assert.False(Maybe<int>.Nothing.Equals(Maybe.Just("") as object));
        }

        [Fact]
        public void Equals_JustDifferent_False()
        {
            Assert.False(Maybe.Just("nothing").Equals(Maybe.Just("something")));
            Assert.False(Maybe.Just("nothing").Equals(Maybe.Just("something") as object));
            Assert.False(Maybe.Just("nothing").Equals(Maybe.Just(5)));
            Assert.False(Maybe.Just("nothing").Equals(Maybe.Just(5 as object)));
            Assert.False(Maybe.Just("nothing").Equals(Maybe.Just("nothing" as object)));

            Assert.False(Maybe.Just(6).Equals(Maybe.Just("something")));
            Assert.False(Maybe.Just(6).Equals(Maybe.Just("something") as object));
            Assert.False(Maybe.Just(6).Equals(Maybe.Just(5)));
            Assert.False(Maybe.Just(6).Equals(Maybe.Just(5 as object)));
            Assert.False(Maybe.Just(6).Equals(Maybe.Just("nothing" as object)));
        }

        [Fact]
        public void Equals_JustEqual_True()
        {
            Assert.True(Maybe.Just(6).Equals(Maybe.Just(6)));
            Assert.True(Maybe.Just(6).Equals(Maybe.Just(6) as object));

            Assert.True(Maybe.Just("something").Equals(Maybe.Just("something")));
            Assert.True(Maybe.Just("something").Equals(Maybe.Just("something") as object));
        }
    }
}
