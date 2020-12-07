using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Banzai.Test
{
    
    public class WhenRunningNode
    {

        [Fact]
        public void Initial_Node_Run_Status_Is_NotRun()
        {
            var testNode = new SimpleTestNodeA1();

            testNode.Status.Should().Be(NodeRunStatus.NotRun);
        }

        [Fact]
        public async Task Errant_Node_Run_Status_Is_Faulted()
        {
            var testNode = new FaultingTestNodeA();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);
            result.Exception.Should().NotBeNull();

            testNode.Status.Should().Be(NodeRunStatus.Faulted);
        }

        [Fact]
        public async Task Failed_Node_Run_Status_Is_Completed_With_Failed_Result()
        {
            var testNode = new FailingTestNodeA();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            testNode.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Failed);
        }

        [Fact]
        public void Errant_Node_Run_With_ThrowOnError_True_Throws()
        {
            var testNode = new FaultingTestNodeA();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject) {GlobalOptions = {ThrowOnError = true}};

            Assert.ThrowsAsync<Exception>(() => testNode.ExecuteAsync(context));
        }
        
        [Fact]
        public async Task Successful_Node_Run_Status_Is_Completed()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            testNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Fact]
        public async Task Successful_Node_Result_Matches_Expectations()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.Status.Should().Be(NodeResultStatus.Succeeded);
            result.Exception.Should().Be(null);
            result.Id.Should().Be(testNode.Id);
           
            context.Subject.TestValueString.Should().Be("Completed");
        }

        [Fact]
        public async Task Node_Is_Not_Run_If_ShouldExecute_False()
        {
            var testNode = new SimpleTestNodeA1(false);

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.Status.Should().Be(NodeResultStatus.NotRun);

            context.Subject.TestValueString.Should().BeNull();
        }

        [Fact]
        public void Node_Specific_Options_Override_Global_Options()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            testNode.LocalOptions = new ExecutionOptions {ThrowOnError = true};

            testNode.GetEffectiveOptions(context.GlobalOptions).ThrowOnError.Should().Be(true);
        }

        [Fact]
        public async Task Global_Options_Are_Reflected_In_Effective_Options()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject) {GlobalOptions = {ThrowOnError = true}};

            var result = await testNode.ExecuteAsync(context);

            testNode.GetEffectiveOptions(context.GlobalOptions).ThrowOnError.Should().Be(true);
        }


    }



    
}