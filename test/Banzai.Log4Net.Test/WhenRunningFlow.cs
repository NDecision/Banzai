using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Banzai.Logging;
using log4net.Config;
using NUnit.Framework;
using FluentAssertions;

namespace Banzai.Log4Net.Test
{
    [TestFixture]
    public class WhenRunningFlow
    {
        public WhenRunningFlow()
        {
            XmlDocument log4NetConfig = new XmlDocument();
            log4NetConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);
        }

        [Test]
        public async Task Logging_Set_With_Log4Net_Doesnt_Err()
        {
            LogWriter.SetFactory(new Log4NetWriterFactory());

            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);
        }
    }
}