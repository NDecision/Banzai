namespace Banzai.JavaScript
{
    /// <summary>
    /// Used as the return from the ShouldExecute JavaScript function to determine if a node should execute.
    /// </summary>
    public class ShouldExecuteResult
    {
        /// <summary>
        /// Should this node Execute.
        /// </summary>
        public bool ShouldExecute { get; set; } 
    }
}