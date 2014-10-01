using System.Threading.Tasks;

namespace Banzai
{

    public interface IFirstMatchNode<T> : IMultiNode<T>
    {
    }

    public class FirstMatchNode<T> : MultiNode<T>, IFirstMatchNode<T>
    {

        protected override async Task<NodeResultStatus> ExecuteChildrenAsync(ExecutionContext<T> context)
        {
            NodeResult<T> result = null;
            
            foreach (var childNode in Children)
            {
                result = await childNode.ExecuteAsync(context);

                if (result.Status != NodeResultStatus.NotRun)
                    break;
            }

            if(result != null)
                return result.Status;

            return NodeResultStatus.NotRun;
        }

    }
}