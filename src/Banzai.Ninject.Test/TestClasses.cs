using System.Threading.Tasks;

namespace Banzai.Ninject.Test
{
    public interface ITestNode<T> : INode<T>
    {
    }

    public class TestNode : Node<object>, ITestNode<object>
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<object> context)
        {
            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public interface ITestNode2 : INode<object>
    {
    }

    public class TestNode2 : Node<object>, ITestNode2
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<object> context)
        {
            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public interface ITestNode3 : INode<object>
    {
    }

    public class TestNode3 : Node<object>, ITestNode3
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<object> context)
        {
            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public interface ITestNode4 : INode<object>
    {
    }

    public class TestNode4 : Node<object>, ITestNode4
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<object> context)
        {
            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public interface ITestPipelineNode1 : IPipelineNode<object>
    {
    }

    public class TestPipelineNode1 : PipelineNodeBase<object>, ITestPipelineNode1
    {
        protected override void OnBeforeExecute(IExecutionContext<object> context)
        {
            AddChild(NodeFactory.GetNode<ITestNode2>());
            AddChild(NodeFactory.GetNode<ITestNode3>());
            AddChild(NodeFactory.GetNode<ITestNode4>());
        }
    }

    public interface ITestPipelineNode2 : IPipelineNode<TestObjectA>
    {
    }

    public class TestPipelineNode2 : PipelineNodeBase<TestObjectA>, ITestPipelineNode2
    {
        protected override void OnBeforeExecute(IExecutionContext<TestObjectA> context)
        {
            AddChild(NodeFactory.GetNode<ITestTransitionNode1>());
        }
    }

    public interface ITestTransitionNode1 : ITransitionNode<TestObjectA, TestObjectB>
    {
    }

    public class TestTransitionNode1 : TransitionNode<TestObjectA, TestObjectB>, ITestTransitionNode1
    {
    }

    public class ShouldNotExecuteTestBlock : ShouldExecuteBlock<object>
    {
        public override bool ShouldExecute(IExecutionContext<object> context)
        {
            return false;
        }
    }


    public class TestObjectA
    {
        public string TestValueString { get; set; }

        public int TestValueInt { get; set; }
    }

    public class TestObjectASub : TestObjectA
    {
        public decimal TestValueDecimal;
    }

    public class TestObjectB
    {
        public bool TestValueBool { get; set; }

    }
}