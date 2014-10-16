using System;
using System.Collections.Generic;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningWithDegreeOfParallelism
    {
        public IEnumerable<TestObjectA> GetTestObjects(int count = 100)
        {
            var testObjects = new List<TestObjectA>(count);
            for (int i = 0; i < count; i++)
            {
                testObjects.Add(new TestObjectA());
            }

            return testObjects;
        }

        [Test]
        public async void Successful_Node_Run_Status_Is_Completed()
        {
            var testNode = new SimpleTestNodeA1();

            var testObjectList = GetTestObjects(500);

            var result = await testNode.ExecuteManyAsync(testObjectList, new ExecutionOptions{ DegreeOfParallelism = 4 });

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.Exception.ShouldBeNull();
            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Successful_Node_Run_Status_Is_Completed_When_Fewer_Subjects_Than_DegreeOfParallelism()
        {
            var testNode = new SimpleTestNodeA1();

            var testObjectList = GetTestObjects(2);

            var result = await testNode.ExecuteManyAsync(testObjectList, new ExecutionOptions { DegreeOfParallelism = 4 });

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.Exception.ShouldBeNull();
            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Failed_Node_Run_Status_Is_Failed()
        {
            var testNode = new FailingTestNodeA();

            var testObjectList = GetTestObjects();

            var result = await testNode.ExecuteManyAsync(testObjectList, new ExecutionOptions { DegreeOfParallelism = 4 });

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            result.Exception.ShouldBeNull();
            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public void Faulted_Node_Throws_If_Throw_On_Error_True()
        {
            var testNode = new FaultingTestNodeA();

            var testObjectList = GetTestObjects();

            Assert.Throws<AggregateException>(async () => await testNode.ExecuteManyAsync(testObjectList, new ExecutionOptions { ThrowOnError = true, DegreeOfParallelism = 4 }));
        }

        [Test]
        public async void Faulted_Node_Run_Status_Is_Failed_If_Continue_On_Failure_True()
        {
            var testNode = new FaultingTestNodeA();

            var testObjectList = GetTestObjects();

            var result = await testNode.ExecuteManyAsync(testObjectList, new ExecutionOptions { ContinueOnFailure = true, DegreeOfParallelism = 4 });

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            result.Exception.ShouldNotBeNull();
            testNode.Status.ShouldEqual(NodeRunStatus.Faulted);
        }


       
    }
}