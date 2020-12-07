using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Banzai.Test
{
    
    public class WhenRunningTransitionFuncNode
    {
        [Fact]
        public async Task Simple_TransitionSourceFunc_Succeeds()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new TransitionFuncNode<TestObjectA, TestObjectB>
            {
                ChildNode = new SimpleTestNodeB1(),
                TransitionSourceFuncAsync = ctxt => Task.FromResult(new TestObjectB())
            });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.Should().Be(NodeResultStatus.Succeeded);
            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Fact]
        public async Task Faulting_TransitionSourceFunc_Returns_Fail_Result()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new TransitionFuncNode<TestObjectA, TestObjectB>
            {
                ChildNode = new SimpleTestNodeB1(),
                TransitionSourceFuncAsync = ctxt => { throw new Exception(); }
            });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.Should().Be(NodeResultStatus.Failed);
            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Fact]
        public async Task Faulting_TransitionResultFunc_Returns_Fail_Result()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new TransitionFuncNode<TestObjectA, TestObjectB>
            {
                ChildNode = new SimpleTestNodeB1(),
                TransitionSourceFuncAsync = ctxt => Task.FromResult(new TestObjectB()),
                TransitionResultFuncAsync = (ctxt, res) => { throw new Exception(); }
            });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.Should().Be(NodeResultStatus.Failed);
            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Fact]
        public async Task Simple_TransitionSourceAsyncFunc_Succeeds()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new TransitionFuncNode<TestObjectA, TestObjectB>
            {
                ChildNode = new SimpleTestNodeB1(),
                TransitionSourceFuncAsync = ctxt => Task.FromResult(new TestObjectB())
            });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.Should().Be(NodeResultStatus.Succeeded);
            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Fact]
        public async Task Simple_TransitionResultFunc_Succeeds()
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
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.Should().Be(NodeResultStatus.Succeeded);
            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);
        }

        [Fact]
        public async Task Simple_TransitionResultAsyncFunc_Succeeds()
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
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.Should().Be(NodeResultStatus.Succeeded);
            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);
        }


        public class SimpleTransitionNode : TransitionNode<TestObjectA, TestObjectB>
        {
            protected override Task<TestObjectB> TransitionSourceAsync(IExecutionContext<TestObjectA> sourceContext)
            {
                return Task.FromResult(new TestObjectB());
            }
        }

    }
}