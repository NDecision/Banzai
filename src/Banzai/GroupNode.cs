namespace Banzai
{
    /// <summary>
    /// This node runs all children potentially simultaneously using Async's WhenAll.
    /// This is a good choice for multiple i/o operations.  The node will not complete until all children complete.
    /// </summary>
    public interface IGroupNode<T> : IGroupNodeBase<T>
    {
    }

    /// <summary>
    /// This node runs all children potentially simultaneously using Async's WhenAll.
    /// This is a good choice for multiple i/o operations.  The node will not complete until all children complete.
    /// </summary>
    public sealed class GroupNode<T> : GroupNodeBase<T>, IGroupNode<T>
    {
    }
}