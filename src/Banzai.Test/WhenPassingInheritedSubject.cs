using System.Threading.Tasks;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenPassingInheritedSubject
    {

        [Test]
        public async void ExecutionContext_Based_On_Inherited_Type_Is_Passed_To_Execute()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectASub();

            var context = new ExecutionContext<TestObjectASub>(testObject);

            var result = await testNode.ExecuteAsync(context);

            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
        }

        [Test]
        public async void ExecutionContext_Based_On_Root_Type_Works_With_Inherited_Type()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectASub();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
        }

        [Test]
        public async void Subject_Of_Inherited_Type_Is_Passed_To_Execute()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectASub();

            var result = await testNode.ExecuteAsync(testObject);

            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
        }

        [Test]
        public async void Subject_Of_Inherited_Type_Works_With_Func_Node()
        {
            var node = new FuncNode<TestObjectA>();

            node.ShouldExecuteFuncAsync = context => Task.FromResult(((TestObjectA)context.Subject).TestValueInt == 0);
            node.ExecutedFuncAsync = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectASub();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.GetSubjectAs<TestObjectA>().TestValueString.ShouldEqual("Completed");
        }

    }
}