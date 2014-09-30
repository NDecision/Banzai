using NUnit.Framework;
using Should;

namespace Banzai.Core.Test
{
    [TestFixture]
    public class When_Running_Basic_Node
    {

        [Test]
        public void Initial_Node_Run_Status_Is_NotRun()
        {
            var testNode = new SimpleTestNodeA1();

            testNode.Status.ShouldEqual(NodeRunStatus.NotRun);
        }

        [Test]
        public async void Errant_Node_Run_Status_Is_Faulted()
        {
            var testNode = new FaultingTestNode();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            testNode.Status.ShouldEqual(NodeRunStatus.Faulted);
        }

        [Test]
        public async void Successful_Node_Run_Status_Is_Completed()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Successful_Node_Result_Matches_Expectations()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.Exception.ShouldEqual(null);

            context.Subject.TestValueString.ShouldEqual("Completed");
        }

        [Test]
        public async void Node_Is_Not_Run_If_ShouldExecute_False()
        {
            var testNode = new SimpleTestNodeA1(false);

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.Status.ShouldEqual(NodeResultStatus.NotRun);

            context.Subject.TestValueString.ShouldBeNull();
        }

        [Test]
        public async void Node_Specific_Options_Override_Global_Options()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            testNode.LocalOptions = new ExecutionOptions {ThrowOnError = true};

            var result = await testNode.ExecuteAsync(context);

            context.EffectiveOptions.ThrowOnError.ShouldEqual(true);
        }

        [Test]
        public async void Global_Options_Are_Reflected_In_Effective_Options()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject) {GlobalOptions = {ThrowOnError = true}};

            var result = await testNode.ExecuteAsync(context);

            context.EffectiveOptions.ThrowOnError.ShouldEqual(true);
        }


    }



    
}