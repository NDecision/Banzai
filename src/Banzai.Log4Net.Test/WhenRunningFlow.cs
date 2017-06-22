﻿using System.Threading.Tasks;
using Banzai.Logging;
using FluentAssertions;
using log4net.Config;
using NUnit.Framework;

namespace Banzai.Log4Net.Test
{
    [TestFixture]
    public class WhenRunningFlow
    {
        public WhenRunningFlow()
        {
            BasicConfigurator.Configure();
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