using NUnit.Framework;
using Should;

namespace Banzai.JavaScript.Test
{
    [TestFixture("JavaScript")]
    public class WhenRunningBasicJavaScript
    {

        [Test]
        public void Initial_Node_Run_Status_Is_NotRun()
        {
            var testNode = new JavaScriptNode<TestObjectA>();

            testNode.Status.ShouldEqual(NodeRunStatus.NotRun);
        }

        [Test]
        public async void Successful_Node_Run_Matches_Expectations()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!'"
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.GetSubjectAs<TestObjectA>().TestValueString.ShouldEqual("Hello JavaScript!");

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Failed_Node_Run_Status_Is_Failed()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!'; result.IsSuccess = false;"
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.GetSubjectAs<TestObjectA>().TestValueString.ShouldEqual("Hello JavaScript!");

            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Failed);
        }

        [Test]
        public async void Errant_Node_Run_Status_Is_Faulted()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!'; throw 'An error happened';"
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);
            result.Exception.ShouldNotBeNull();

            result.Status.ShouldEqual(NodeResultStatus.Failed);

            testNode.Status.ShouldEqual(NodeRunStatus.Faulted);
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

            Assert.Throws<Microsoft.ClearScript.ScriptEngineException>(async () => await testNode.ExecuteAsync(context));
        }


        [Test]
        public async void Node_Is_Not_Run_If_ShouldExecute_False()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!';",
                ShouldExecuteFunc = c => false
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.Status.ShouldEqual(NodeResultStatus.NotRun);

            context.Subject.TestValueString.ShouldBeNull();
        }

        [Test]
        public async void Node_Is_Not_Run_If_ShouldExecuteScript_False()
        {
            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = "context.Subject.TestValueString = 'Hello JavaScript!';",
                ShouldExecuteScript = "result.ShouldExecute = false;"
            };

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.Status.ShouldEqual(NodeResultStatus.NotRun);

            context.Subject.TestValueString.ShouldBeNull();
        }

    }



    
}