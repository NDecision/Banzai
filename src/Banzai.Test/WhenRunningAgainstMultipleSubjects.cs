using System;
using System.Collections.Generic;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningAgainstMultipleSubjects
    {
        [Test]
        public async void Successful_Node_Run_Status_Is_Completed()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> {testObject1, testObject2};

            var result = await testNode.ExecuteManyAsync(testObjectList);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.Exception.ShouldBeNull();
            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Failed_Node_Run_Status_Is_Failed()
        {
            var testNode = new FailingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            var result = await testNode.ExecuteManyAsync(testObjectList);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            result.Exception.ShouldBeNull();
            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public void Faulted_Node_Throws_If_Throw_On_Error_True()
        {
            var testNode = new FaultingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            Assert.Throws<AggregateException>(async () => await testNode.ExecuteManyAsync(testObjectList, new ExecutionOptions { ThrowOnError = true }));
        }

        [Test]
        public async void Faulted_Node_Run_Status_Is_Failed_If_Continue_On_Failure_True()
        {
            var testNode = new FaultingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            var result = await testNode.ExecuteManyAsync(testObjectList, new ExecutionOptions{ContinueOnFailure = true});

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            result.Exception.ShouldNotBeNull();
            testNode.Status.ShouldEqual(NodeRunStatus.Faulted);
        }


        [Test]
        public async void Successful_Sync_Node_Run_Status_Is_Completed()
        {
            var testNode = new SimpleTestNodeA1();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            var result = await testNode.ExecuteManySeriallyAsync(testObjectList);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.Exception.ShouldBeNull();
            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Failed_Sync_Node_Run_Status_Is_Failed()
        {
            var testNode = new FailingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            var result = await testNode.ExecuteManySeriallyAsync(testObjectList);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            result.Exception.ShouldBeNull();
            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public void Faulted_Sync_Node_Throws_If_Throw_On_Error_True()
        {
            var testNode = new FaultingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            Assert.Throws<Exception>(async () => await testNode.ExecuteManySeriallyAsync(testObjectList, new ExecutionOptions { ThrowOnError = true }));
        }

        [Test]
        public async void Faulted_Sync_Node_Run_Status_Is_Failed_If_Continue_On_Failure_True()
        {
            var testNode = new FaultingTestNodeA();

            var testObject1 = new TestObjectA();
            var testObject2 = new TestObjectA();

            var testObjectList = new List<TestObjectA> { testObject1, testObject2 };

            var result = await testNode.ExecuteManySeriallyAsync(testObjectList, new ExecutionOptions { ContinueOnFailure = true });

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            result.Exception.ShouldNotBeNull();
            testNode.Status.ShouldEqual(NodeRunStatus.Faulted);
        }
    }
}