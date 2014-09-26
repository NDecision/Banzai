using System.Threading.Tasks;

namespace Banzai.Core
{
    public class Pipeline<T> : Node<T>, IPipeline<T>
    {
        public Pipeline(INode<T> rootNode)
        {
            RootNode = rootNode;
        }
        
        public INode<T> RootNode { get; private set; } 


        protected override Task<bool> PerformExecuteAsync(T subject)
        {
            throw new System.NotImplementedException();
        }
    }
}