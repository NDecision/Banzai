using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningFuncNode
    {
        [Test]
        public async Task Successful_FuncNode_Values_Match_Expected()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueInt == 0));
            node.ExecutedFunc = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectA();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);
            result.GetSubjectAs<TestObjectA>().TestValueString.Should().Be("Completed");
        }

        [Test]
        public async Task FuncNode_With_ShouldExecute_False_Shouldnt_Run()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueInt == 5));
            node.ExecutedFunc = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectA();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.Should().Be(NodeRunStatus.NotRun);
            result.Status.Should().Be(NodeResultStatus.NotRun);
            result.GetSubjectAs<TestObjectA>().TestValueString.Should().BeNull();
        }


        [Test]
        public async Task Can_Run_Func_Node_On_Inherited_Type()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueInt == 0));
            node.ExecutedFunc = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectASub();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);
            result.GetSubjectAs<TestObjectA>().TestValueString.Should().Be("Completed");
        }


    }
}