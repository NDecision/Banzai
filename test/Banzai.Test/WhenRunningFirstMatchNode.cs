using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Banzai.Test
{
    
    public class WhenRunningFirstMatchNode
    {
        [Fact]
        public async Task Successful_FirstMatch_Node_Runs_First_Node_And_Not_Second_Node_When_Matched()
        {
            var matchNode = new FirstMatchNode<TestObjectA>();

            var firstNode = new SimpleTestNodeA1();
            matchNode.AddChild(firstNode);

            var secondNode = new SimpleTestNodeA2();
            matchNode.AddChild(secondNode);

            var testObject = new TestObjectA();
            NodeResult result = await matchNode.ExecuteAsync(testObject);

            matchNode.Status.Should().Be(NodeRunStatus.Completed);
            firstNode.Status.Should().Be(NodeRunStatus.Completed);
            secondNode.Status.Should().Be(NodeRunStatus.NotRun);
            result.Status.Should().Be(NodeResultStatus.Succeeded);

            testObject.TestValueString.Should().Be("Completed");
            testObject.TestValueInt.Should().Be(0);
        }

        [Fact]
        public async Task Successful_FirstMatch_Node_Runs_Second_Node_When_First_Not_Matched()
        {
            var matchNode = new FirstMatchNode<TestObjectA>();

            var firstNode = new SimpleTestNodeA1(false);
            matchNode.AddChild(firstNode);

            var secondNode = new SimpleTestNodeA2();
            matchNode.AddChild(secondNode);

            var testObject = new TestObjectA();
            NodeResult result = await matchNode.ExecuteAsync(testObject);

            matchNode.Status.Should().Be(NodeRunStatus.Completed);
            firstNode.Status.Should().Be(NodeRunStatus.NotRun);
            secondNode.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);

            testObject.TestValueString.Should().BeNull();
            testObject.TestValueInt.Should().Be(100);
        }

        [Fact]
        public async Task FirstMatch_Node_Fails_When_Selected_Node_Fails()
        {
            var matchNode = new FirstMatchNode<TestObjectA>();

            var firstNode = new FailingTestNodeA();
            matchNode.AddChild(firstNode);

            var secondNode = new SimpleTestNodeA2();
            matchNode.AddChild(secondNode);

            var testObject = new TestObjectA();
            NodeResult result = await matchNode.ExecuteAsync(testObject);

            matchNode.Status.Should().Be(NodeRunStatus.Completed);
            firstNode.Status.Should().Be(NodeRunStatus.Completed);
            secondNode.Status.Should().Be(NodeRunStatus.NotRun);
            result.Status.Should().Be(NodeResultStatus.Failed);

            testObject.TestValueString.Should().Be("Failed");
            testObject.TestValueInt.Should().Be(0);
        }

        [Fact]
        public async Task FirstMatch_Node_Fails_When_Selected_Node_Faults()
        {
            var matchNode = new FirstMatchNode<TestObjectA>();

            var firstNode = new FaultingTestNodeA();
            matchNode.AddChild(firstNode);

            var secondNode = new SimpleTestNodeA2();
            matchNode.AddChild(secondNode);

            var testObject = new TestObjectA();
            NodeResult result = await matchNode.ExecuteAsync(testObject);

            matchNode.Status.Should().Be(NodeRunStatus.Completed);
            firstNode.Status.Should().Be(NodeRunStatus.Faulted);
            secondNode.Status.Should().Be(NodeRunStatus.NotRun);
            result.Status.Should().Be(NodeResultStatus.Failed);

            testObject.TestValueString.Should().Be("Faulted");
            testObject.TestValueInt.Should().Be(0);
        }
    }
}