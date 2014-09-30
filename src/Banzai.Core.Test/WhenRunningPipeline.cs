using NUnit.Framework;
using Should;

namespace Banzai.Core.Test
{

    [TestFixture]
    public class WhenRunningPipeline
    {
        [Test]
        public async void Successful_Pipeline_Run_Status_Is_Succeeded()
        {
            var groupNode = new PipelineNode<TestObjectA>();

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Initial_Failure_Returns_Failed_Status()
        {
            var groupNode = new PipelineNode<TestObjectA>();

            groupNode.AddChild(new FailingTestNode());
            groupNode.AddChild(new SimpleTestNodeA1());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Failure_After_Success_Returns_SucceededWithErrors_Status()
        {
            var groupNode = new PipelineNode<TestObjectA>();

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new FailingTestNode());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Initial_Failure_And_ContinueOnError_Returns_SucceededWithErrors_Status()
        {
            var groupNode = new PipelineNode<TestObjectA> { LocalOptions = new ExecutionOptions { ContinueOnError = true } };

            groupNode.AddChild(new FailingTestNode());
            groupNode.AddChild(new SimpleTestNodeA1());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Final_Failure_And_ContinueOnError_Returns_SucceededWithErrors_Status()
        {
            var groupNode = new PipelineNode<TestObjectA> {LocalOptions = new ExecutionOptions {ContinueOnError = true}};

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new FailingTestNode());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Initial_Fault_Returns_Failed_Status()
        {
            var groupNode = new PipelineNode<TestObjectA>();

            groupNode.AddChild(new FaultingTestNode());
            groupNode.AddChild(new SimpleTestNodeA1());
            
            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Fault_After_Success_Returns_SucceededWithErrors_Status()
        {
            var groupNode = new PipelineNode<TestObjectA>();
            
            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new FaultingTestNode());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Initial_Fault_And_ContinueOnError_Returns_SucceededWithErrors_Status()
        {
            var groupNode = new PipelineNode<TestObjectA> { LocalOptions = new ExecutionOptions { ContinueOnError = true } };

            groupNode.AddChild(new FaultingTestNode());
            groupNode.AddChild(new SimpleTestNodeA1());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Pipeline_Run_With_Final_Fault_And_ContinueOnError_Returns_SucceededWithErrors_Status()
        {
            var groupNode = new PipelineNode<TestObjectA> { LocalOptions = new ExecutionOptions { ContinueOnError = true } };

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new FaultingTestNode());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Failed_Pipeline_Run_Returns_Failed_Status()
        {
            var groupNode = new PipelineNode<TestObjectA>();

            groupNode.AddChild(new FailingTestNode());
            groupNode.AddChild(new FailingTestNode());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Faulted_Pipeline_Run_Returns_Failed_Status()
        {
            var groupNode = new PipelineNode<TestObjectA>();

            groupNode.AddChild(new FaultingTestNode());
            groupNode.AddChild(new FaultingTestNode());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Successful_Pipeline_Result_Matches_Expectations()
        {
            var groupNode = new PipelineNode<TestObjectA>();

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            testObject.TestValueString.ShouldEqual("Completed");
            testObject.TestValueInt.ShouldEqual(100);
        }

    }
}