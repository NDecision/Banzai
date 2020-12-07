namespace Banzai
{
    /// <summary>
    /// This node runs children serially as defined by their current order (as added).
    /// This node is used for constructing pipelines. The node will not complete until all children complete or an error is encountered.
    /// </summary>
    public sealed class PipelineNode<T> : PipelineNodeBase<T>, IPipelineNode<T>
    {
    }
}