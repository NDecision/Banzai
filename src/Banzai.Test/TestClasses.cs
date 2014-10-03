using System;
using System.Threading.Tasks;

namespace Banzai.Test
{
    public class SimpleTestNodeA1 : Node<TestObjectA>
    {
        private readonly bool _shouldExecute = true;

        public SimpleTestNodeA1()
        {
        }

        public SimpleTestNodeA1(bool shouldExecute)
        {
            _shouldExecute = shouldExecute;
        }

        public override Task<bool> ShouldExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            return Task.FromResult(_shouldExecute);
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.Subject.TestValueString = "Completed";

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public class SimpleTestNodeA2 : Node<TestObjectA>
    {
        private readonly bool _shouldExecute = true;

        public SimpleTestNodeA2()
        {
        }
        
        public SimpleTestNodeA2(bool shouldExecute)
        {
            _shouldExecute = shouldExecute;
        }

        public override Task<bool> ShouldExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            return Task.FromResult(_shouldExecute);
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.Subject.TestValueInt = 100;

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public class SubjectChangingNode1 : Node<TestObjectA>
    {
        private readonly bool _shouldExecute = true;

        public SubjectChangingNode1()
        {
        }

        public SubjectChangingNode1(bool shouldExecute)
        {
            _shouldExecute = shouldExecute;
        }

        public override Task<bool> ShouldExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            return Task.FromResult(_shouldExecute);
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.ChangeSubject(new TestObjectA()
            {
                TestValueString = "New Instance"
            });

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public class FaultingTestNode : Node<TestObjectA>
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.Subject.TestValueString = "Faulted";

            throw new Exception("Test Exception");
        }
    }

    public class FailingTestNode : Node<TestObjectA>
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.Subject.TestValueString = "Failed";

            return Task.FromResult(NodeResultStatus.Failed);
        }
    }

    public class TestObjectA
    {
        public string TestValueString { get; set; }

        public int TestValueInt { get; set; }
    }
}