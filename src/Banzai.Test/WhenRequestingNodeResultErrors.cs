using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRequestingNodeResultErrors
    {
        [Test]
        public async void Pipeline_Run_With_Initial_Failure_Returns_Failed_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new FaultingTestNode());
            pipelineNode.AddChild(new SimpleTestNodeA1());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            IEnumerable<Exception> exceptions = result.GetFailExceptions();

            exceptions.ShouldNotBeNull();
            exceptions.Count().ShouldEqual(1);
        }

        [Test]
        public async void Pipeline_With_ContinueOnError_Excludes_Initial_Exception()
        {
            var pipelineNode = new PipelineNode<TestObjectA>
            {
                LocalOptions = new ExecutionOptions {ContinueOnFailure = true}
            };

            pipelineNode.AddChild(new FaultingTestNode());
            pipelineNode.AddChild(new SimpleTestNodeA1());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            IEnumerable<Exception> exceptions = result.GetFailExceptions();

            exceptions.ShouldNotBeNull();
            exceptions.Count().ShouldEqual(0);
        }

        [Test]
        public async void Pipeline_With_ContinueOnError_Returns_Exceptions_On_All_Failures()
        {
            var pipelineNode = new PipelineNode<TestObjectA>
            {
                LocalOptions = new ExecutionOptions { ContinueOnFailure = true }
            };

            pipelineNode.AddChild(new FaultingTestNode());
            pipelineNode.AddChild(new FaultingTestNode());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            IEnumerable<Exception> exceptions = result.GetFailExceptions();

            exceptions.ShouldNotBeNull();
            exceptions.Count().ShouldEqual(2);
        }

        [Test]
        public async void Nested_Exception_Is_Included_In_Collection()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            var pipelineNode2 = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(pipelineNode2);

            pipelineNode2.AddChild(new FaultingTestNode());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            IEnumerable<Exception> exceptions = result.GetFailExceptions();

            exceptions.ShouldNotBeNull();
            exceptions.Count().ShouldEqual(1);
        }

        [Test]
        public async void Group_Run_With_Multiple_Failures_Returns_Failed_Statuses()
        {
            var pipelineNode = new GroupNode<TestObjectA>();

            var faultNode1 = new FaultingTestNode();
            var faultNode2 = new FaultingTestNode();

            pipelineNode.AddChild(faultNode1);
            pipelineNode.AddChild(faultNode2);

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            IEnumerable<Exception> exceptions = result.GetFailExceptions();

            exceptions.ShouldNotBeNull();
            exceptions.Count().ShouldBeGreaterThan(0);
        } 
    }
}