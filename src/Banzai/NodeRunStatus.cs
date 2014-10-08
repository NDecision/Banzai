namespace Banzai
{
    /// <summary>
    /// Indicates the current run status of the node.
    /// </summary>
    public enum NodeRunStatus
    {
        /// <summary>
        /// Node has not been run.
        /// </summary>
        NotRun = 0,

        /// <summary>
        /// Node is running.
        /// </summary>
        Running = 1,

        /// <summary>
        /// Node has completed running.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Node has faulted, meaning an exception was thrown.
        /// </summary>
        Faulted = 3
    }
}