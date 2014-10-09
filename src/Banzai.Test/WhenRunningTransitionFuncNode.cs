using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningTransitionFuncNode
    {
        [Test]
        public async void Simple_TransitionSourceFunc_Succeeds()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new TransitionFuncNode<TestObjectA,TestObjectB> {ChildNode = new SimpleTestNodeB1(), TransitionSourceFunc = ctxt => new TestObjectB()});
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Faulting_TransitionSourceFunc_Returns_Fail_Result()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new TransitionFuncNode<TestObjectA, TestObjectB>
            {
                ChildNode = new SimpleTestNodeB1(),
                TransitionSourceFunc = ctxt => { throw new Exception(); }
            });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Faulting_TransitionResultFunc_Returns_Fail_Result()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new TransitionFuncNode<TestObjectA, TestObjectB>
            {
                ChildNode = new SimpleTestNodeB1(),
                TransitionSourceFunc = ctxt => new TestObjectB(),
                TransitionResultFunc = (ctxt, res) => { throw new Exception(); }
            });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Simple_TransitionSourceAsyncFunc_Succeeds()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new TransitionFuncNode<TestObjectA, TestObjectB> { ChildNode = new SimpleTestNodeB1(), TransitionSourceFuncAsync = ctxt => Task.FromResult(new TestObjectB()) });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Simple_TransitionResultFunc_Succeeds()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new TransitionFuncNode<TestObjectA, TestObjectB>
            {
                ChildNode = new SimpleTestNodeB1(),
                TransitionSourceFunc = ctxt => new TestObjectB(),
                TransitionResultFunc = (ctxt, res) => ctxt.Subject
            });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Simple_TransitionResultAsyncFunc_Succeeds()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new TransitionFuncNode<TestObjectA, TestObjectB>
            {
                ChildNode = new SimpleTestNodeB1(),
                TransitionSourceFuncAsync = ctxt => Task.FromResult(new TestObjectB()),
                TransitionResultFuncAsync = (ctxt, res) => Task.FromResult(ctxt.Subject)
            });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }


        public class SimpleTransitionNode : TransitionNode<TestObjectA, TestObjectB>
        {
            protected override TestObjectB TransitionSource(ExecutionContext<TestObjectA> sourceContext)
            {
                return new TestObjectB();
            }
        }

    }
}