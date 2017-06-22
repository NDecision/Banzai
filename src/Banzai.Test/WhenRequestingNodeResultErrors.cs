using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRequestingNodeResultErrors
    {
        [Test]
        public async Task Pipeline_Run_With_Initial_Failure_Returns_Failed_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new FaultingTestNodeA());
            pipelineNode.AddChild(new SimpleTestNodeA1());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            IEnumerable<Exception> exceptions = result.GetFailExceptions();

            exceptions.Should().NotBeNull();
            exceptions.Count().Should().Be(1);
        }

        [Test]
        public async Task Pipeline_With_ContinueOnError_Excludes_Initial_Exception()
        {
            var pipelineNode = new PipelineNode<TestObjectA>
            {
                LocalOptions = new ExecutionOptions {ContinueOnFailure = true}
            };

            pipelineNode.AddChild(new FaultingTestNodeA());
            pipelineNode.AddChild(new SimpleTestNodeA1());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            IEnumerable<Exception> exceptions = result.GetFailExceptions();

            exceptions.Should().NotBeNull();
            exceptions.Count().Should().Be(0);
        }

        [Test]
        public async Task Pipeline_With_ContinueOnError_Returns_Exceptions_On_All_Failures()
        {
            var pipelineNode = new PipelineNode<TestObjectA>
            {
                LocalOptions = new ExecutionOptions { ContinueOnFailure = true }
            };

            pipelineNode.AddChild(new FaultingTestNodeA());
            pipelineNode.AddChild(new FaultingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            IEnumerable<Exception> exceptions = result.GetFailExceptions();

            exceptions.Should().NotBeNull();
            exceptions.Count().Should().Be(2);
        }

        [Test]
        public async Task Nested_Exception_Is_Included_In_Collection()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            var pipelineNode2 = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(pipelineNode2);

            pipelineNode2.AddChild(new FaultingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            IEnumerable<Exception> exceptions = result.GetFailExceptions();

            exceptions.Should().NotBeNull();
            exceptions.Count().Should().Be(1);
        }

        [Test]
        public async Task Group_Run_With_Multiple_Failures_Returns_Failed_Statuses()
        {
            var pipelineNode = new GroupNode<TestObjectA>();

            var faultNode1 = new FaultingTestNodeA();
            var faultNode2 = new FaultingTestNodeA();

            pipelineNode.AddChild(faultNode1);
            pipelineNode.AddChild(faultNode2);

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            IEnumerable<Exception> exceptions = result.GetFailExceptions();

            exceptions.Should().NotBeNull();
            exceptions.Count().Should().BeGreaterThan(0);
        } 
    }
}