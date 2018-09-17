using System.Threading.Tasks;
using Banzai.Logging;
using Serilog;
using Serilog.Configuration;
using NUnit.Framework;
using FluentAssertions;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace Banzai.Serilog.Test
{
    [TestFixture]
    public class WhenRunningFlow
    {
        public WhenRunningFlow()
        {
            var target = new TestSink(new MessageTemplateTextFormatter("${message}", null));

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink(target, LogEventLevel.Verbose)
                .CreateLogger();

            LogWriter.SetFactory(new SerilogWriterFactory());
        }

        [Test]
        public async Task Logging_Set_With_Serilog_Doesnt_Err()
        {
            LogWriter.SetFactory(new SerilogWriterFactory());

            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);
        }
    }
}