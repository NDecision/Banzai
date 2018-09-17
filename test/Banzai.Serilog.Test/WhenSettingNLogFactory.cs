using Banzai.Logging;
using NUnit.Framework;
using FluentAssertions;

namespace Banzai.Serilog.Test
{
    [TestFixture]
    public class WhenSettingSerilogFactory
    {

        [Test]
        public void Factory_Returns_Correct_Logger()
        {
            LogWriter.SetFactory(new SerilogWriterFactory());

            var writer = LogWriter.GetLogger(typeof(object));

            writer.Should().BeOfType<SerilogWriter>();
        }

    }
}