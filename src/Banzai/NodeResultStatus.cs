namespace Banzai
{
    /// <summary>
    /// Result status of a node.
    /// </summary>
    public enum NodeResultStatus
    {
        /// <summary>
        /// Node has not been run.
        /// </summary>
        NotRun = 0,

        /// <summary>
        /// Node is considered a success but errors were encountered.
        /// </summary>
        SucceededWithErrors = 1,

        /// <summary>
        /// Node ran successfully.
        /// </summary>
        Succeeded = 2,

        /// <summary>
        /// Node failed horribly.
        /// </summary>
        Failed = 3
    }
}