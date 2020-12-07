namespace Banzai
{
    /// <summary>
    /// Defines a node in which the first matching ShouldExecute() node is executed.
    /// </summary>
    /// <typeparam name="T">Type on which the node operates.</typeparam>
    public interface IFirstMatchNodeBase<T> : IMultiNode<T>
    {
    }
}