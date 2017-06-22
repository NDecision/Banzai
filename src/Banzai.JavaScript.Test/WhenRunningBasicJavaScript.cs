using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Banzai.JavaScript.Test
{
    [TestFixture, Explicit]
    public class WhenRunningBasicJavaScript
    {

        [Test]
        public void Initial_Node_Run_Status_Is_NotRun()
        {
            var testNode = new JavaScriptNode<TestObjectA>();

            testNode.Status.Should().Be(NodeRunStatus.NotRun);
        }

        [Test]
        public async Task Successful_Node_Run_Matches_Expectations()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!'"
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.GetSubjectAs<TestObjectA>().TestValueString.Should().Be("Hello JavaScript!");

            result.Status.Should().Be(NodeResultStatus.Succeeded);
            testNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Test]
        public async Task Failed_Node_Run_Status_Is_Failed()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!'; result.IsSuccess = false;"
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.GetSubjectAs<TestObjectA>().TestValueString.Should().Be("Hello JavaScript!");

            testNode.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Failed);
        }

        [Test]
        public async Task Errant_Node_Run_Status_Is_Faulted()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!'; throw 'An error happened';"
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);
            result.Exception.Should().NotBeNull();

            result.Status.Should().Be(NodeResultStatus.Failed);

            testNode.Status.Should().Be(NodeRunStatus.Faulted);
        }


        [Test]
        public void Errant_Node_Run_With_ThrowOnError_True_Throws()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!'; throw 'An error happened';"
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject) { GlobalOptions = { ThrowOnError = true } };

            Assert.ThrowsAsync<Microsoft.ClearScript.ScriptEngineException>(async () => await testNode.ExecuteAsync(context));
        }


        [Test]
        public async Task Node_Is_Not_Run_If_ShouldExecute_False()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!';",
                ShouldExecuteFunc = c => Task.FromResult(false)
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.Status.Should().Be(NodeResultStatus.NotRun);

            context.Subject.TestValueString.Should().BeNull();
        }

        [Test]
        public async Task Node_Is_Not_Run_If_ShouldExecuteScript_False()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!';",
                ShouldExecuteScript = "result.ShouldExecute = false;"
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.Status.Should().Be(NodeResultStatus.NotRun);

            context.Subject.TestValueString.Should().BeNull();
        }

    }



    
}