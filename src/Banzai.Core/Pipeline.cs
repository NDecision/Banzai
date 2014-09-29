﻿using System.Threading.Tasks;

namespace Banzai.Core
{
    public interface IPipeline<T> : IMultiNode<T>
    {

    }

    public class Pipeline<T> : MultiNode<T>, IPipeline<T>
    {
        protected override async Task<NodeResultStatus> ExecuteChildrenAsync(ExecutionContext<T> context)
        {
            NodeResult<T> result = null;

            foreach (var childNode in Children)
            {
                result = await childNode.ExecuteAsync(context);
            }

            if (result != null)
                return result.Status;

            return NodeResultStatus.NotRun;
        }
    }
}