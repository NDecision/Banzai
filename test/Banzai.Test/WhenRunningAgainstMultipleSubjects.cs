using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningAgainstMultipleSubjects
    {
        [Test]
        public async Task Successful_Node_Run_Status_Is_Completed()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> {testObject1, testObject2};

            var result = await testNode.ExecuteManyAsync(testObjectList);

            result.Status.Should().Be(NodeResultStatus.Succeeded);
            result.Exception.Should().BeNull();
            testNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Test]
        public async Task Failed_Node_Run_Status_Is_Failed()
        {
            var testNode = new FailingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            var result = await testNode.ExecuteManyAsync(testObjectList);

            result.Status.Should().Be(NodeResultStatus.Failed);
            result.Exception.Should().BeNull();
            testNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Test]
        public void Faulted_Node_Throws_If_Throw_On_Error_True()
        {
            var testNode = new FaultingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            Assert.ThrowsAsync<AggregateException>(() => testNode.ExecuteManyAsync(testObjectList, new ExecutionOptions { ThrowOnError = true }));
        }

        [Test]
        public async Task Faulted_Node_Run_Status_Is_Failed_If_Continue_On_Failure_True()
        {
            var testNode = new FaultingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            var result = await testNode.ExecuteManyAsync(testObjectList, new ExecutionOptions{ContinueOnFailure = true});

            result.Status.Should().Be(NodeResultStatus.Failed);
            result.Exception.Should().NotBeNull();
            testNode.Status.Should().Be(NodeRunStatus.Faulted);
        }


        [Test]
        public async Task Successful_Sync_Node_Run_Status_Is_Completed()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            var result = await testNode.ExecuteManySeriallyAsync(testObjectList);

            result.Status.Should().Be(NodeResultStatus.Succeeded);
            result.Exception.Should().BeNull();
            testNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Test]
        public async Task Failed_Sync_Node_Run_Status_Is_Failed()
        {
            var testNode = new FailingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            var result = await testNode.ExecuteManySeriallyAsync(testObjectList);

            result.Status.Should().Be(NodeResultStatus.Failed);
            result.Exception.Should().BeNull();
            testNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Test]
        public void Faulted_Sync_Node_Throws_If_Throw_On_Error_True()
        {
            var testNode = new FaultingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            Assert.ThrowsAsync<Exception>(() => testNode.ExecuteManySeriallyAsync(testObjectList, new ExecutionOptions { ThrowOnError = true }));
        }

        [Test]
        public async Task Faulted_Sync_Node_Run_Status_Is_Failed_If_Continue_On_Failure_True()
        {
            var testNode = new FaultingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            var result = await testNode.ExecuteManySeriallyAsync(testObjectList, new ExecutionOptions { ContinueOnFailure = true });

            result.Status.Should().Be(NodeResultStatus.Failed);
            result.Exception.Should().NotBeNull();
            testNode.Status.Should().Be(NodeRunStatus.Faulted);
        }
    }
}