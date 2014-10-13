using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenCancellingProcessing
    {
        [Test]
        public async void When_Cancelling_Pipeline_At_First_Node_Then_Status_Is_NotRun()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            var testNode1 = new SimpleTestNodeA1(true, false, true);
            var testNode2 = new SimpleTestNodeA2();

            pipelineNode.AddChild(testNode1);
            pipelineNode.AddChild(testNode2);

            var testObject = new TestObjectA();

            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.NotRun);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);

            testNode1.Status.ShouldEqual(NodeRunStatus.NotRun);
            testNode2.Status.ShouldEqual(NodeRunStatus.NotRun);
        }

        [Test]
        public async void When_Cancelling_Pipeline_At_Later_Node_Then_Status_Is_Success()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            var testNode1 = new SimpleTestNodeA1();
            var testNode2 = new SimpleTestNodeA1(true, false, true);
            var testNode3 = new SimpleTestNodeA2();

            pipelineNode.AddChild(testNode1);
            pipelineNode.AddChild(testNode2);
            pipelineNode.AddChild(testNode3);

            var testObject = new TestObjectA();

            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);

            testNode1.Status.ShouldEqual(NodeRunStatus.Completed);
            testNode2.Status.ShouldEqual(NodeRunStatus.NotRun);
            testNode3.Status.ShouldEqual(NodeRunStatus.NotRun);
        }

        [Test]
        public async void Parent_Pipeline_Cancels_Execution_When_Child_Pipeline_Node_Cancelled()
        {
            var testNode1 = new SimpleTestNodeA1();
            var testNode2 = new SimpleTestNodeA1(true, false, true);
            var testNode3 = new SimpleTestNodeA2();
            
            var innerPipelineNode = new PipelineNode<TestObjectA>();
            innerPipelineNode.AddChild(testNode1);
            innerPipelineNode.AddChild(testNode2);

            var pipelineNode = new PipelineNode<TestObjectA>();
            pipelineNode.AddChild(innerPipelineNode);
            pipelineNode.AddChild(testNode3);
            
            var testObject = new TestObjectA();

            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
    
            innerPipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
            testNode1.Status.ShouldEqual(NodeRunStatus.Completed);
            testNode2.Status.ShouldEqual(NodeRunStatus.NotRun);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
            testNode3.Status.ShouldEqual(NodeRunStatus.NotRun);
        }

    }
}