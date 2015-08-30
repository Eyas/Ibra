using System;
using System.Collections.Generic;

using Ibra.Polymorphic.Covariant;

using Xunit;

namespace IbraTests.Polymorphic.Covariant
{
    public class MaybeTests
    {
        [Fact]
        void Nothing_AlwaysThrows()
        {
            var vt1 = Nothing<int>.Instance;
            var vt2 = Nothing<KeyValuePair<int, double>>.Instance;
            var rt1 = Nothing<string>.Instance;
            var rt2 = Nothing<object>.Instance;

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
        void Just_AlwaysSucceeds()
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
        void Nothing_Map_HasNoEffect()
        {
            Func<int, int> iiMap = (x) => { Assert.True(false, "Expected this is unreached."); return x; };
            Func<int, string> isMap = (x) => { Assert.True(false, "Expected this is unreached."); return x.ToString(); };
            Func<string, object> soMap = (x) => { Assert.True(false, "Expected this is unreached."); return null; };

            var iNothing = Nothing<int>.Instance;
            var sNothing = Nothing<string>.Instance;

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
        void Just_Map_HasEffect()
        {
            Func<int, int> iiMap = (x) => x * 2;
            Func<int, string> isMap = (x) => x.ToString();
            Func<string, object> soMap = (x) => x;

            var i1 = Maybe.Just(3);
            var i2 = Maybe.Just(-483);
            var i3 = Maybe.Just(0);

            var s1 = Maybe.Just("fooooo");
            var s2 = Maybe.Just("baaaar");
            var s3 = Maybe.Just("blergh");

            var ii1 = i1.Map(iiMap);
            var ii2 = i2.Map(iiMap);
            var ii3 = i3.Map(iiMap);

            Assert.True(ii1.HasValue); Assert.Equal(ii1.Value, 6);
            Assert.True(ii2.HasValue); Assert.Equal(ii2.Value, -966);
            Assert.True(ii3.HasValue); Assert.Equal(ii3.Value, 0);

            var is1 = i1.Map(isMap);
            var is2 = i2.Map(isMap);
            var is3 = i3.Map(isMap);

            Assert.True(is1.HasValue); Assert.Equal(is1.Value, "3");
            Assert.True(is2.HasValue); Assert.Equal(is2.Value, "-483");
            Assert.True(is3.HasValue); Assert.Equal(is3.Value, "0");

            var so1 = s1.Map(soMap);
            var so2 = s2.Map(soMap);
            var so3 = s3.Map(soMap);

            Assert.True(so1.HasValue);
            Assert.True(so2.HasValue);
            Assert.True(so3.HasValue);
        }

        [Fact]
        void Nothing_FlatMap_DoesntEven()
        {
            // when FlatMap'ing a Nothing.. you can't even!
            Func<int, Maybe<int>> iiMap = (x) => { Assert.True(false, "Expected this is unreached."); return Maybe.Just(x); };
            Func<int, Maybe<string>> isMap = (x) => { Assert.True(false, "Expected this is unreached."); return Maybe.Just(x.ToString()); };
            Func<string, Maybe<object>> soMap = (x) => { Assert.True(false, "Expected this is unreached."); return Maybe.Just<object>(null); };

            var iNothing = Nothing<int>.Instance;
            var sNothing = Nothing<string>.Instance;

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
        void Anything_FlatMapWithNothing_CreatesNothing()
        {
            Func<int, Maybe<int>> iiMap = (x) => Nothing<int>.Instance;
            Func<int, Maybe<string>> isMap = (x) => Nothing<string>.Instance;
            Func<string, Maybe<object>> soMap = (x) => Nothing<object>.Instance;

            Maybe<int>[] int_maybes = new Maybe<int>[] { Maybe.Just(0), Maybe.Just(5), Maybe.Just(3282), Maybe.Just(-42), Nothing<int>.Instance };
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
                Assert.False(is_result.FlatMap(soMap).Map(o => o.ToString()).HasValue);
            }
        }

        [Fact]
        void Nothing_Convert_Sanity()
        {
            Maybe<int> nI = Nothing<int>.Instance;
            Maybe<string> nS = Nothing<string>.Instance;
            Maybe<object> nO = Nothing<object>.Instance;

            Assert.Equal("success", nI.Convert((just) => "fail", () => "success"));
            Assert.Equal("success", nS.Convert((just) => "fail", () => "success"));
            Assert.Equal("success", nO.Convert((just) => "fail", () => "success"));

            Assert.Equal("success", nI.Convert((just) => "fail", "success"));
            Assert.Equal("success", nS.Convert((just) => "fail", "success"));
            Assert.Equal("success", nO.Convert((just) => "fail", "success"));
        }

        [Fact]
        void Just_Convert_Sanity()
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
        void Nothing_TrueFilter_DoesntEven()
        {
            Predicate<int> cantEven = (o) => { Assert.True(false, "Expected unreached"); return true; };

            var nInt = Nothing<int>.Instance.Filter(cantEven);

            Assert.False(nInt.HasValue);
            Assert.Throws<NotSupportedException>(() => nInt.Value);
        }

        [Fact]
        void Just_TrueFilter_Doesnt()
        {
            Predicate<int> noEffect_int = (i) => true;
            Predicate<string> noEffect_string = (i) => true;

            var mInt = Maybe.Just(150).Filter(noEffect_int);
            var mStr = Maybe.Just("5").Filter(noEffect_string);

            Assert.True(mInt.HasValue);
            Assert.Equal(mInt.Value, 150);

            Assert.True(mStr.HasValue);
            Assert.Equal(mStr.Value, "5");
        }

        [Fact]
        void Just_FalseFilter_Filters()
        {
            Predicate<int> iFilter = (i) => false;
            Predicate<string> sFilter = (i) => false;

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
        void Nothing_GetOrElse_Else()
        {
            var nI = Nothing<int>.Instance;
            var nS = Nothing<string>.Instance;

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
        void Just_GetOrElse_Get()
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
    }
}
