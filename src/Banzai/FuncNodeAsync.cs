using System;
using System.Threading.Tasks;
using Banzai.Utility;

namespace Banzai
{
    public interface IFuncNodeAsync<T> : INode<T>
    {
        Func<ExecutionContext<T>, Task<bool>> ShouldExecuteFunc { get; set; }

        Func<ExecutionContext<T>, Task<NodeResultStatus>> ExecuteFunc { get; set; }
    }


    public class FuncNodeAsync<T> : Node<T>, IFuncNodeAsync<T>
    {
        public Func<ExecutionContext<T>, Task<bool>> ShouldExecuteFunc { get; set; }

        public Func<ExecutionContext<T>, Task<NodeResultStatus>> ExecuteFunc { get; set; }

        public override async Task<bool> ShouldExecute(ExecutionContext<T> context)
        {
            if (ShouldExecuteFunc != null)
                return await ShouldExecuteFunc(context);

            return await base.ShouldExecute(context);
        }

        protected override async Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context)
        {
            Guard.AgainstNullProperty("PerformExecuteFunc", ExecuteFunc);

            return await ExecuteFunc(context);
        }
    }

  
}