using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningWithShouldExecuteBlocks
    {
        [Test]
        public async Task Node_With_ShouldExecuteBlock_Should_Run()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecuteBlock(new ShouldExecuteBlockA());
            node.ExecutedFunc = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectA();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);
            result.GetSubjectAs<TestObjectA>().TestValueString.Should().Be("Completed");
        } 

        [Test]
        public async Task Node_With_ShouldExecuteBlock_False_Shouldnt_Run()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecuteBlock(new ShouldNotExecuteBlockA());
            node.ExecutedFunc = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectA();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.Should().Be(NodeRunStatus.NotRun);
            result.Status.Should().Be(NodeResultStatus.NotRun);
            result.GetSubjectAs<TestObjectA>().TestValueString.Should().BeNull();
        } 
    }
}