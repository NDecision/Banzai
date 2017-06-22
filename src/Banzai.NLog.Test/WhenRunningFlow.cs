using System.Threading.Tasks;
using Banzai.Logging;
using FluentAssertions;
using NLog;
using NLog.Config;
using NUnit.Framework;

namespace Banzai.NLog.Test
{
    [TestFixture]
    public class WhenRunningFlow
    {
        public WhenRunningFlow()
        {
            SimpleConfigurator.ConfigureForConsoleLogging(LogLevel.Trace);
        }

        [Test]
        public async Task Logging_Set_With_NLog_Doesnt_Err()
        {
            LogWriter.SetFactory(new NLogWriterFactory());

            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);
        }
    }
}