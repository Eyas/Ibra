using System.Text;
using Xunit;

using Ibra.Logging;

namespace IbraTests
{
    /// Implements a basic mocked TextWriter due to lack of proper mocking framework across
    /// .NET Framework and .NET Core as of now.!--.!--.
    public class MockedWriter : System.IO.TextWriter
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            _sb.Append(value);
        }

        public void Clear()
        {
            _sb.Clear();
        }

        public string Text => _sb.ToString();
    }

    public class LoggingTest
    {
        public string AssertNotCalled()
        {
            Assert.True(false, "Did not expect this method to be called.");
            return "BAD";
        }
        [Fact]
        public void VerboseLog_LazilyEvaluated()
        {
            Logger lg = new Logger();

            MockedWriter
              file1 = new MockedWriter(),
              file2 = new MockedWriter(),
              file3 = new MockedWriter();
            Source
              source1 = lg.RegisterSource("source1"),
              source2 = lg.RegisterSource("source2"),
              source3 = lg.RegisterSource("source3");

            lg.AddSource(source1, Level.INFO, file1);
            lg.AddSource(source2, Level.FATAL, file2);
            lg.AddSource(source3, Level.TRACE, file3);

            lg.Log(source1, Level.VERBOSE)
             ?.WriteLine("Message 1")
              .WriteLine("Mesasge 2")
              .WriteLine("Message 3")
              .WriteLine(AssertNotCalled());

            lg.Log(source1, Level.VERBOSE)
             ?.WriteLine(AssertNotCalled())
              .WriteLine(AssertNotCalled());
            
            lg.Log(source2, Level.INFO)
             ?.WriteLine(AssertNotCalled());

            Assert.Empty(file1.Text);
            Assert.Empty(file2.Text);
            Assert.Empty(file3.Text);

            file1.Clear();
            file2.Clear();
            file3.Clear();

            lg.Log(source1, Level.INFO)
              ?.WriteLine("Exists_1")
               .WriteLine("Exists_2")
               .WriteLine("Exists_3");
            
            Assert.Contains("INFO", file1.Text);
            Assert.Contains("Exists_1", file1.Text);
            Assert.Contains("Exists_2", file1.Text);
            Assert.Contains("Exists_3", file1.Text);
            Assert.Empty(file2.Text);
            Assert.Empty(file3.Text);

            file1.Clear();
            file2.Clear();
            file3.Clear();
        }
    }
}
