namespace Banzai.JavaScript
{
    /// <summary>
    /// Represents the success or failure of a javascript node execution.
    /// </summary>
    public class NodeResult
    {
        /// <summary>
        /// Did the node succeed.  The default is true unless the node sets this to false.
        /// </summary>
        public bool IsSuccess { get; set; } 
    }
}