using System;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningNode
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
            var testNode = new FaultingTestNodeA();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);
            result.Exception.ShouldNotBeNull();

            testNode.Status.ShouldEqual(NodeRunStatus.Faulted);
        }

        [Test]
        public async void Failed_Node_Run_Status_Is_Completed_With_Failed_Result()
        {
            var testNode = new FailingTestNodeA();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Failed);
        }

        [Test]
        public void Errant_Node_Run_With_ThrowOnError_True_Throws()
        {
            var testNode = new FaultingTestNodeA();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject) {GlobalOptions = {ThrowOnError = true}};

            Assert.Throws<Exception>(async () => await testNode.ExecuteAsync(context));
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
        public void Node_Specific_Options_Override_Global_Options()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            testNode.LocalOptions = new ExecutionOptions {ThrowOnError = true};

            testNode.GetEffectiveOptions(context.GlobalOptions).ThrowOnError.ShouldEqual(true);
        }

        [Test]
        public async void Global_Options_Are_Reflected_In_Effective_Options()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject) {GlobalOptions = {ThrowOnError = true}};

            var result = await testNode.ExecuteAsync(context);

            testNode.GetEffectiveOptions(context.GlobalOptions).ThrowOnError.ShouldEqual(true);
        }


    }



    
}