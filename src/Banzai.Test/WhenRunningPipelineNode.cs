using System.Linq;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningPipelineNode
    {
        [Test]
        public async void Successful_Pipeline_Run_Status_Is_Succeeded()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Result_Should_Equal_Initial_Subject()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Subject.ShouldBeSameAs(testObject);
        }

        [Test]
        public async void Pipeline_Child_Result_Count_Equals_Child_Node_Count()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.ChildResults.Count().ShouldEqual(2);
        }

        [Test]
        public async void Pipeline_Result_Ids_Equal_Node_Ids()
        {
            var pipelineNode = new PipelineNode<TestObjectA>{Id="PipelineNode1", FlowId = "Flow1"};

            pipelineNode.AddChild(new SimpleTestNodeA1{FlowId = "Flow1"});
            pipelineNode.AddChild(new SimpleTestNodeA2{Id = "Child2", FlowId = "Flow1"});

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.FlowId.ShouldEqual("Flow1");
            result.Id.ShouldEqual("PipelineNode1");
            result.ChildResults.First().Id.ShouldEqual("Banzai.Test.SimpleTestNodeA1");
            result.ChildResults.First().FlowId.ShouldEqual("Flow1");

            var secondResult = result.ChildResults.FirstOrDefault(x => x.Id == "Child2");
            secondResult.ShouldNotBeNull();
            secondResult.FlowId.ShouldEqual("Flow1");
        }

        [Test]
        public async void Pipeline_Run_With_Initial_Failure_Returns_Failed_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new FailingTestNodeA());
            pipelineNode.AddChild(new SimpleTestNodeA1());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Failure_After_Success_Returns_Failed_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new FailingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }


        [Test]
        public async void Pipeline_Run_With_Initial_Failure_And_ContinueOnError_Returns_SucceededWithErrors_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA> { LocalOptions = new ExecutionOptions { ContinueOnFailure = true } };

            pipelineNode.AddChild(new FailingTestNodeA());
            pipelineNode.AddChild(new SimpleTestNodeA1());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Final_Failure_And_ContinueOnError_Returns_SucceededWithErrors_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA> {LocalOptions = new ExecutionOptions {ContinueOnFailure = true}};

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new FailingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Initial_Fault_Returns_Failed_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new FaultingTestNodeA());
            pipelineNode.AddChild(new SimpleTestNodeA1());
            
            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Fault_After_Success_Returns_Failed_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();
            
            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new FaultingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Initial_Fault_And_ContinueOnError_Returns_SucceededWithErrors_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA> { LocalOptions = new ExecutionOptions { ContinueOnFailure = true } };

            pipelineNode.AddChild(new FaultingTestNodeA());
            pipelineNode.AddChild(new SimpleTestNodeA1());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Final_Fault_And_ContinueOnError_Returns_SucceededWithErrors_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA> { LocalOptions = new ExecutionOptions { ContinueOnFailure = true } };

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new FaultingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Failed_Pipeline_Run_Returns_Failed_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new FailingTestNodeA());
            pipelineNode.AddChild(new FailingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Faulted_Pipeline_Run_Returns_Failed_Status()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new FaultingTestNodeA());
            pipelineNode.AddChild(new FaultingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Successful_Pipeline_Result_Matches_Expectations()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            testObject.TestValueString.ShouldEqual("Completed");
            testObject.TestValueInt.ShouldEqual(100);
        }

        [Test]
        public async void Nested_Pipeline_Results_Contain_All_Child_Results()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();
            var pipelineNode2 = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(pipelineNode2);
            pipelineNode2.AddChild(new SimpleTestNodeA2());
            pipelineNode2.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.ChildResults.Count().ShouldEqual(1);
            result.ChildResults.First().ChildResults.Count().ShouldEqual(2);
        }

    }
}