﻿using System.Threading.Tasks;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningFuncNode
    {
        [Test]
        public async void Successful_FuncNodeSync_Values_Match_Expected()
        {
            var node = new FuncNode<TestObjectA>();

            node.ShouldExecuteFunc = context => Task.FromResult(context.Subject.TestValueInt == 0);
            node.ExecuteFunc = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await node.ExecuteAsync(testObject);

            node.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            result.Subject.TestValueString.ShouldEqual("Completed");
        }

        [Test]
        public async void FuncNodeSync_With_ShouldExecute_False_Shouldnt_Run()
        {
            var node = new FuncNode<TestObjectA>();

            node.ShouldExecuteFunc = context => Task.FromResult(context.Subject.TestValueInt == 5);
            node.ExecuteFunc = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await node.ExecuteAsync(testObject);

            node.Status.ShouldEqual(NodeRunStatus.NotRun);
            result.Status.ShouldEqual(NodeResultStatus.NotRun);
            result.Subject.TestValueString.ShouldBeNull();
        } 
    }
}