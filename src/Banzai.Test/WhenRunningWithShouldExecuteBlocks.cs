using System.Threading.Tasks;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningWithShouldExecuteBlocks
    {
        [Test]
        public async void Node_With_ShouldExecuteBlock_Should_Run()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecuteBlock(new ShouldExecuteBlockA());
            node.ExecutedFuncAsync = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectA();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.GetSubjectAs<TestObjectA>().TestValueString.ShouldEqual("Completed");
        } 

        [Test]
        public async void Node_With_ShouldExecuteBlock_False_Shouldnt_Run()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecuteBlock(new ShouldNotExecuteBlockA());
            node.ExecutedFuncAsync = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectA();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.ShouldEqual(NodeRunStatus.NotRun);
            result.Status.ShouldEqual(NodeResultStatus.NotRun);
            result.GetSubjectAs<TestObjectA>().TestValueString.ShouldBeNull();
        } 
    }
}