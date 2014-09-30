using System;
using System.Threading.Tasks;
using Banzai.Utility;

namespace Banzai
{
    public interface IFuncNodeSync<T> : INode<T>
    {
        Func<ExecutionContext<T>, bool> ShouldExecuteFunc { get; set; }

        Func<ExecutionContext<T>, NodeResultStatus> ExecuteFunc { get; set; }
    }

    public class FuncNodeSync<T> : Node<T>, IFuncNodeSync<T>
    {
        public Func<ExecutionContext<T>, bool> ShouldExecuteFunc { get; set; }

        public Func<ExecutionContext<T>, NodeResultStatus> ExecuteFunc { get; set; }

        public override Task<bool> ShouldExecute(ExecutionContext<T> context)
        {
            bool shouldExecute = true;

            if (ShouldExecuteFunc != null)
                shouldExecute = ShouldExecuteFunc(context);

            return Task.FromResult(shouldExecute);
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context)
        {
            Guard.AgainstNullProperty("PerformExecuteFunc", ExecuteFunc);

            return Task.FromResult(ExecuteFunc(context));
        }
    }
}