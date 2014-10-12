using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningGroupNode
    {
        [Test]
        public async void Successful_Group_Run_Status_Is_Succeeded()
        {
            var groupNode = new GroupNode<TestObjectA>();

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result =  await groupNode.ExecuteAsync(testObject);

            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Group_Run_With_Failure_Returns_Failed_Status()
        {
            var groupNode = new GroupNode<TestObjectA>();

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new FailingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Group_Run_With_Failure_And_ContinueOnError_Returns_SucceededWithErrors_Status()
        {
            var groupNode = new GroupNode<TestObjectA> {LocalOptions = new ExecutionOptions {ContinueOnFailure = true}};

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new FailingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Group_Run_With_Fault_Returns_Failed_Status()
        {
            var groupNode = new GroupNode<TestObjectA>();

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new FaultingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Group_Run_With_Fault_And_ContinueOnError_Returns_SucceededWithErrors_Status()
        {
            var groupNode = new GroupNode<TestObjectA> { LocalOptions = new ExecutionOptions { ContinueOnFailure = true } };

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new FaultingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.SucceededWithErrors);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Failed_Group_Run_Returns_Failed_Status()
        {
            var groupNode = new GroupNode<TestObjectA>();

            groupNode.AddChild(new FailingTestNodeA());
            groupNode.AddChild(new FailingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Faulted_Group_Run_Returns_Failed_Status()
        {
            var groupNode = new GroupNode<TestObjectA>();

            groupNode.AddChild(new FaultingTestNodeA());
            groupNode.AddChild(new FaultingTestNodeA());

            var testObject = new TestObjectA();
            NodeResult result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Successful_Group_Result_Matches_Expectations()
        {
            var groupNode = new GroupNode<TestObjectA>();

            groupNode.AddChild(new SimpleTestNodeA1());
            groupNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await groupNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            testObject.TestValueString.ShouldEqual("Completed");
            testObject.TestValueInt.ShouldEqual(100);
        }

    }
}