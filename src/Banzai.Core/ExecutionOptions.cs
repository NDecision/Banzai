namespace Banzai.Core
{
    public class ExecutionOptions
    {
        /// <summary>
        /// Indicates that pipeline processing should continue if this node errs.
        /// </summary>
        public bool ContinueOnError { get; set; }

        /// <summary>
        /// Indicates that pipeline processing should throw an exception when an error occurs instead oof adding it to the error collection.
        /// </summary>
        /// <remarks>
        /// This applies to the execution of the node only.  An error will always be thrown if ShouldExecute errs.
        /// </remarks>
        public bool ThrowOnError { get; set; }
    }
}