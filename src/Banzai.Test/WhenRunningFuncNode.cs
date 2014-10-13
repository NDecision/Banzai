using System.Threading.Tasks;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningFuncNode
    {
        [Test]
        public async void Successful_FuncNode_Values_Match_Expected()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueInt == 0));
            node.ExecutedFuncAsync = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectA();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.GetSubjectAs<TestObjectA>().TestValueString.ShouldEqual("Completed");
        }

        [Test]
        public async void FuncNode_With_ShouldExecute_False_Shouldnt_Run()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueInt == 5));
            node.ExecutedFuncAsync = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectA();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.ShouldEqual(NodeRunStatus.NotRun);
            result.Status.ShouldEqual(NodeResultStatus.NotRun);
            result.GetSubjectAs<TestObjectA>().TestValueString.ShouldBeNull();
        }

        [Test]
        public async void Successful_Sync_FuncNode_Values_Match_Expected()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueInt == 0));
            node.ExecutedFunc = context => { context.Subject.TestValueString = "Completed"; return NodeResultStatus.Succeeded; };

            var testObject = new TestObjectA();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.GetSubjectAs<TestObjectA>().TestValueString.ShouldEqual("Completed");
        }

        [Test]
        public async void FuncNode_Sync_With_ShouldExecute_False_Shouldnt_Run()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueInt == 5));
            node.ExecutedFunc = context => { context.Subject.TestValueString = "Completed"; return NodeResultStatus.Succeeded; };

            var testObject = new TestObjectA();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.ShouldEqual(NodeRunStatus.NotRun);
            result.Status.ShouldEqual(NodeResultStatus.NotRun);
            result.GetSubjectAs<TestObjectA>().TestValueString.ShouldBeNull();
        }

        [Test]
        public async void Can_Run_Func_Node_On_Inherited_Type()
        {
            var node = new FuncNode<TestObjectA>();

            node.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueInt == 0));
            node.ExecutedFuncAsync = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectASub();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.GetSubjectAs<TestObjectA>().TestValueString.ShouldEqual("Completed");
        }


    }
}