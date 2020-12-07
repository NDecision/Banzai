using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Banzai.Test
{
    
    public class WhenAddingInheritedSubjectNodes
    {
        [Fact]
        public async Task Can_Add_Base_Type_Nodes_To_Inherited_Type_Pipeline()
        {
            var testNode = new SimpleTestNodeA1();
            var testNode2 = new SimpleTestNodeASub1();

            var pipeline = new PipelineNode<TestObjectASub>();

            pipeline.AddChild(testNode);
            pipeline.AddChild(testNode2);

            var testObject = new TestObjectASub();

            var result = await pipeline.ExecuteAsync(testObject);

            testNode.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);
        }

        [Fact]
        public async Task Can_Add_Base_Type_Pipeline_Node_To_Inherited_Type_Pipeline()
        {
            var testNode = new SimpleTestNodeA1();

            var pipeline = new PipelineNode<TestObjectA>();
            pipeline.AddChild(testNode);

            var testNode2 = new SimpleTestNodeASub1();
            var pipelineSub = new PipelineNode<TestObjectASub>();

            pipelineSub.AddChild(testNode2);
            pipelineSub.AddChild(pipeline);
            
            var testObject = new TestObjectASub();

            var result = await pipeline.ExecuteAsync(testObject);

            testNode.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);
        }

        [Fact]
        public async Task ShouldExecuteFunc_On_Test_Object_Is_Evaluated()
        {
            var testNode = new SimpleTestNodeA1();
            var testNode2 = new SimpleTestNodeASub1();

            testNode2.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueDecimal == 1));

            var pipeline = new PipelineNode<TestObjectASub>();

            pipeline.AddChild(testNode);
            pipeline.AddChild(testNode2);

            var testObject = new TestObjectASub();

            var result = await pipeline.ExecuteAsync(testObject);

            testNode.Status.Should().Be(NodeRunStatus.Completed);
            testNode2.Status.Should().Be(NodeRunStatus.NotRun);
        }

        [Fact]
        public async Task Can_Add_Inherited_Func_Node_To_Pipleline()
        {
            var testNode = new FuncNode<TestObjectA>();

            testNode.AddShouldExecute(context => Task.FromResult(context.Subject.TestValueInt == 0));
            testNode.ExecutedFunc = context => { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

            var testObject = new TestObjectASub();

            var pipeline = new PipelineNode<TestObjectASub>();
            pipeline.AddChild(testNode);

            var result = await pipeline.ExecuteAsync(testObject);

            testNode.Status.Should().Be(NodeRunStatus.Completed);
            result.Status.Should().Be(NodeResultStatus.Succeeded);
        }

    }
}