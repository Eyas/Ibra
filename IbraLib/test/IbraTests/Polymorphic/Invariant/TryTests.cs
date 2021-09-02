using Ibra.Polymorphic.Invariant;
using System;
using Xunit;

namespace IbraTests.Polymorphic.Invariant
{
    internal class TestException : Exception { }
    internal class TestException2 : Exception { }

    public class TryTests
    {
        [Fact]
        public void Success_Basic()
        {
            Func<string> lambda = () => "successful result.";
            Try<string> succ = lambda.Try();

            Assert.Equal("successful result.", succ.GetOrThrow());
            Assert.Equal("successful result..", succ.Convert((s) => s + ".", (e) => "boo"));
            Assert.Equal("successful result.", succ.Catch((Exception e) => "caught").GetOrThrow());
            Assert.Equal("successful result.!", succ.Then((s) => s + "!").GetOrThrow());

            Try<string> thend1 = succ.Then((s) => "S");
            Assert.Equal("S", thend1.GetOrThrow());

            Try<string> thend2 = succ.Then((s) => "S", (e) => "F");
            Assert.Equal("S", thend2.GetOrThrow());

            Try<string> failOnSuccess = succ.Then<string>((s) => { throw new TestException(); });
            Assert.Throws<TestException>(() => failOnSuccess.GetOrThrow());
        }

        [Fact]
        public void Failure_Throws()
        {
            Func<string> lambda = () => { throw new TestException(); };
            Try<string> fail = lambda.Try();
            Assert.Throws<TestException>(() => fail.GetOrThrow());
            Assert.Equal("TestException :)", fail.Convert((s) => "success? " + s, (e) => "TestException :)"));

            Try<string> thend = fail.Then((succ) => succ);
            Assert.Throws<TestException>(() => thend.GetOrThrow());
            Assert.Equal("Fail", thend.Convert((s) => "succeeded", (e) => "Fail"));

            Try<string> successThen = fail.Then((succ) => "S", (e) => "F");
            Assert.Equal("F", successThen.GetOrThrow());

            Try<string> kindaFail = fail.Then((succ) => "S", (e) => { throw new TestException2(); });
            Assert.Throws<TestException2>(() => kindaFail.GetOrThrow());
        }
    }
}
