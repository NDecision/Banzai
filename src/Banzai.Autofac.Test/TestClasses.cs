using System.Threading.Tasks;

namespace Banzai.Autofac.Test
{
    public interface ITestNode<T> : INode<T> { }

    public class TestNode : Node<object>, ITestNode<object>
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<object> context)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface ITestNode2 : INode<object> { }

    public class TestNode2 : Node<object>, ITestNode2
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<object> context)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface ITestNode3 : INode<object> { }

    public class TestNode3 : Node<object>, ITestNode3
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<object> context)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface ITestNode4 : INode<object> { }

    public class TestNode4 : Node<object>, ITestNode4
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<object> context)
        {
            throw new System.NotImplementedException();
        }
    }
}