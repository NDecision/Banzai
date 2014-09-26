namespace Banzai.Core
{
    public class PipelineResult<T> : NodeResult<T>
    {
        public PipelineResult(T subject) : base(subject)
        {
        }
    }
}