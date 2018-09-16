using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenPassingInheritedSubject
    {

        [Test]
        public async Task ExecutionContext_Based_On_Inherited_Type_Is_Passed_To_Execute()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectASub();

            var context = new ExecutionContext<TestObjectASub>(testObject);

            var result = await testNode.ExecuteAsync(context);

            testNode.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);
        }

        [Test]
        public async Task ExecutionContext_Based_On_Root_Type_Works_With_Inherited_Type()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectASub();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            testNode.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);
        }

        [Test]
        public async Task Subject_Of_Inherited_Type_Is_Passed_To_Execute()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectASub();

            var result = await testNode.ExecuteAsync(testObject);

            testNode.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);
        }

        [Test]
        public async Task Subject_Of_Inherited_Type_Works_With_Func_Node()
        {
            var node = new FuncNode<TestObjectA>();

            node.ShouldExecuteFunc = context => Task.FromResult(((TestObjectA)context.Subject).TestValueInt == 0);
            node.ExecutedFunc = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectASub();
            NodeResult result = await node.ExecuteAsync(testObject);

            node.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);
            result.GetSubjectAs<TestObjectA>().TestValueString.Should().Be("Completed");
        }

    }
}