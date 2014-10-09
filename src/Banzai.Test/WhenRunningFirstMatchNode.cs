using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningFirstMatchNode
    {
        [Test]
        public async void Successful_FirstMatch_Node_Runs_First_Node_And_Not_Second_Node_When_Matched()
        {
            var matchNode = new FirstMatchNode<TestObjectA>();

            var firstNode = new SimpleTestNodeA1();
            matchNode.AddChild(firstNode);

            var secondNode = new SimpleTestNodeA2();
            matchNode.AddChild(secondNode);

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await matchNode.ExecuteAsync(testObject);

            matchNode.Status.ShouldEqual(NodeRunStatus.Completed);
            firstNode.Status.ShouldEqual(NodeRunStatus.Completed);
            secondNode.Status.ShouldEqual(NodeRunStatus.NotRun);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);

            testObject.TestValueString.ShouldEqual("Completed");
            testObject.TestValueInt.ShouldEqual(0);
        }

        [Test]
        public async void Successful_FirstMatch_Node_Runs_Second_Node_When_First_Not_Matched()
        {
            var matchNode = new FirstMatchNode<TestObjectA>();

            var firstNode = new SimpleTestNodeA1(false);
            matchNode.AddChild(firstNode);

            var secondNode = new SimpleTestNodeA2();
            matchNode.AddChild(secondNode);

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await matchNode.ExecuteAsync(testObject);

            matchNode.Status.ShouldEqual(NodeRunStatus.Completed);
            firstNode.Status.ShouldEqual(NodeRunStatus.NotRun);
            secondNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);

            testObject.TestValueString.ShouldBeNull();
            testObject.TestValueInt.ShouldEqual(100);
        }

        [Test]
        public async void FirstMatch_Node_Fails_When_Selected_Node_Fails()
        {
            var matchNode = new FirstMatchNode<TestObjectA>();

            var firstNode = new FailingTestNodeA();
            matchNode.AddChild(firstNode);

            var secondNode = new SimpleTestNodeA2();
            matchNode.AddChild(secondNode);

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await matchNode.ExecuteAsync(testObject);

            matchNode.Status.ShouldEqual(NodeRunStatus.Completed);
            firstNode.Status.ShouldEqual(NodeRunStatus.Completed);
            secondNode.Status.ShouldEqual(NodeRunStatus.NotRun);
            result.Status.ShouldEqual(NodeResultStatus.Failed);

            testObject.TestValueString.ShouldEqual("Failed");
            testObject.TestValueInt.ShouldEqual(0);
        }

        [Test]
        public async void FirstMatch_Node_Fails_When_Selected_Node_Faults()
        {
            var matchNode = new FirstMatchNode<TestObjectA>();

            var firstNode = new FaultingTestNodeA();
            matchNode.AddChild(firstNode);

            var secondNode = new SimpleTestNodeA2();
            matchNode.AddChild(secondNode);

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await matchNode.ExecuteAsync(testObject);

            matchNode.Status.ShouldEqual(NodeRunStatus.Completed);
            firstNode.Status.ShouldEqual(NodeRunStatus.Faulted);
            secondNode.Status.ShouldEqual(NodeRunStatus.NotRun);
            result.Status.ShouldEqual(NodeResultStatus.Failed);

            testObject.TestValueString.ShouldEqual("Faulted");
            testObject.TestValueInt.ShouldEqual(0);
        }
    }
}