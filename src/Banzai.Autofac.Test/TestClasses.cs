using System.Threading.Tasks;

namespace Banzai.Autofac.Test
{
    public interface ITestNode<T> : INode<T> { }

    public class TestNode : Node<object>, ITestNode<object>
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<object> context)
        {
            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public interface ITestNode2 : INode<object> { }

    public class TestNode2 : Node<object>, ITestNode2
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<object> context)
        {
            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public interface ITestNode3 : INode<object> { }

    public class TestNode3 : Node<object>, ITestNode3
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<object> context)
        {
            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public interface ITestNode4 : INode<object> { }

    public class TestNode4 : Node<object>, ITestNode4
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<object> context)
        {
            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public interface ITestPipelineNode1 : IPipelineNode<object> { }

    public class TestPipelineNode1 : PipelineNodeBase<object>, ITestPipelineNode1
    {
        protected override void OnBeforeExecute(ExecutionContext<object> context)
        {
            AddChild(NodeFactory.GetNode<ITestNode2>());
            AddChild(NodeFactory.GetNode<ITestNode3>());
            AddChild(NodeFactory.GetNode<ITestNode4>());
        }
    }
}