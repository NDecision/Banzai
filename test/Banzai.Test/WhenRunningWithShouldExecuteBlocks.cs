using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Banzai.Test
{
    
    public class WhenRunningWithShouldExecuteBlocks
    {
        [Fact]
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

        [Fact]
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